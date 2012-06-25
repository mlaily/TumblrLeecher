using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace TumblrLeecher.Api
{
	public sealed class Tumblr
	{
		private const string COMMON_URL_BASE = "http://api.tumblr.com/v2";
		public string ApiKey { get; set; }
		public string BlogHostName { get; set; }

		private Tumblr(string apiKey)
		{
			this.ApiKey = apiKey;
		}

		public Tumblr(string apiKey, string blogHostName)
			: this(apiKey)
		{
			this.BlogHostName = blogHostName;
		}

		private Response<T> DoRequest<T>(string url) where T : ITumblrParsable, new()
		{
			var request = System.Net.HttpWebRequest.Create(url);
			var response = request.GetResponse();
			Response<T> result = new Response<T>(response.GetResponseStream());
			return result;
		}

		private enum RequestTypes
		{
			Blog,
			User,
		}
		private string FormatRequestUrl(RequestTypes type)
		{
			switch (type)
			{
				case RequestTypes.Blog:
					return string.Format("{0}/blog/{1}", COMMON_URL_BASE, this.BlogHostName);
				case RequestTypes.User:
					return string.Format("{0}/user/{1}", COMMON_URL_BASE, this.BlogHostName);
				default:
					throw new NotImplementedException();
			}
		}

		public enum Filters
		{
			/// <summary>
			/// Plain text, no HTML
			/// </summary>
			Text,
			/// <summary>
			/// As entered by the user (no post-processing); if the user writes in Markdown, the Markdown will be returned rather than HTML
			/// </summary>
			Raw,
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type">
		/// The type of post to return. Specify one of the following:  text, quote, link, answer, video, audio, photo, chat.
		/// default to none i.e all posts.
		/// </param>
		public Response<PostCollection> RequestPosts(Post.Types type = Post.Types.None, int offset = 0, int limit = 20, long? id = null, string tag = null, Filters filter = Filters.Raw, bool reblogInfo = false, bool notesInfo = false)
		{
			StringBuilder queryParameters = new StringBuilder();
			if (offset != 0)
			{
				queryParameters.AppendFormat("&offset={0}", offset);
			}
			if (limit != 20)
			{
				queryParameters.AppendFormat("&limit={0}", limit);
			}
			if (id != null)
			{
				queryParameters.AppendFormat("&id={0}", id.Value);
			}
			if (!string.IsNullOrWhiteSpace(tag))
			{
				queryParameters.AppendFormat("&tag={0}", tag);
			}
			if (reblogInfo == true)
			{
				queryParameters.AppendFormat("&reblog_info={0}", reblogInfo);
			}
			if (notesInfo == true)
			{
				queryParameters.AppendFormat("&notes_info={0}", notesInfo);
			}
			queryParameters.AppendFormat("&filter={0}", filter.ToString().ToLowerInvariant());
			string url = string.Format("{0}/posts{1}?api_key={2}{3}",
				FormatRequestUrl(RequestTypes.Blog),
				(type == Post.Types.None ? "" : "/" + type.ToString().ToLowerInvariant()),
				this.ApiKey,
				queryParameters.ToString());

			//todo: handle id, and therefore, post instead of post collection
			return DoRequest<PostCollection>(url);
		}

		public Response<BlogInfo> RequestBlogInfos()
		{
			string url = string.Format("{0}/info?api_key={1}",
				FormatRequestUrl(RequestTypes.Blog),
				this.ApiKey);

			return DoRequest<BlogInfo>(url);
		}

		/// <summary>
		/// only method not requiring authentication in any form.
		/// </summary>
		/// <param name="size">Must be one of the values: 16, 24, 30, 40, 48, 64, 96, 128, 512</param>
		/// <returns></returns>
		public Avatar RequestAvatar(int size = 64)
		{
			if (!new int[] { 16, 24, 30, 40, 48, 64, 96, 128, 512 }.Contains(size))
			{
				throw new ArgumentException("out of range size. (16, 24, 30, 40, 48, 64, 96, 128, 512)");
			}

			string url = string.Format("{0}/avatar{1}",
				FormatRequestUrl(RequestTypes.Blog),
				(size == 64 ? "" : "/" + size.ToString()));

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			var response = request.GetResponse();
			return new Avatar(response);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using TumblrLeecher.Api.Converters;

namespace TumblrLeecher.Api
{
	public sealed class Tumblr
	{
		private const string COMMON_URL_BASE = "http://api.tumblr.com/v2";
		public string ApiKey { get; set; }
		public string BlogHostName { get; set; }

		private static readonly JsonConverter[] _converters = new JsonConverter[]
		{
			new ResponseConverter(),
			new BlogInfoConverter(),
			new PostCollectionConverter(),
			new PostConverter(),
		};

		private Tumblr(string apiKey)
		{
			this.ApiKey = apiKey;
		}

		public Tumblr(string apiKey, string blogHostName)
			: this(apiKey)
		{
			this.BlogHostName = blogHostName;
		}

		private HttpWebRequest CreateHttpRequest(HttpMethod httpMethod, string uri)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
			request.Method = Enum.GetName(typeof(HttpMethod), httpMethod);
			request.ServicePoint.Expect100Continue = false;
			return request;
		}

		private Response<T> DoRequest<T>(string url)
		{
			var request = CreateHttpRequest(HttpMethod.GET, url);
			string responseBody;
			var response = Utility.TryGetResponse(request, out responseBody);

			var result = JsonConvert.DeserializeObject<Response<T>>(responseBody, _converters);

			return result;
		}

		private string FormatRequestUrl(RequestType type)
		{
			switch (type)
			{
				case RequestType.Blog:
					return string.Format("{0}/blog/{1}", COMMON_URL_BASE, this.BlogHostName);
				case RequestType.User:
					return string.Format("{0}/user/{1}", COMMON_URL_BASE, this.BlogHostName);
				default:
					throw new NotImplementedException();
			}
		}

		private enum RequestType
		{
			Blog,
			User,
		}

		public enum Filter
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
		public Response<PostCollection> RequestPosts(Post.Types type = Post.Types.None, int offset = 0, int limit = 20, long? id = null, string tag = null, Filter filter = Filter.Raw, bool reblogInfo = false, bool notesInfo = false)
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
				FormatRequestUrl(RequestType.Blog),
				(type == Post.Types.None ? "" : "/" + type.ToString().ToLowerInvariant()),
				this.ApiKey,
				queryParameters.ToString());

			//todo: handle id, and therefore, post instead of post collection
			return DoRequest<PostCollection>(url);
		}

		public Response<BlogInfo> RequestBlogInfos()
		{
			string url = string.Format("{0}/info?api_key={1}",
				FormatRequestUrl(RequestType.Blog),
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
				throw new ArgumentException("size is out of range. accepted values: (16, 24, 30, 40, 48, 64, 96, 128, 512)");
			}

			string url = string.Format("{0}/avatar{1}",
				FormatRequestUrl(RequestType.Blog),
				(size == 64 ? "" : "/" + size.ToString()));

			HttpWebRequest request = CreateHttpRequest(HttpMethod.GET, url);
			var response = request.GetResponse();
			return new Avatar(response);
		}
	}

	internal enum HttpMethod
	{
		GET,
		POST,
	}

}

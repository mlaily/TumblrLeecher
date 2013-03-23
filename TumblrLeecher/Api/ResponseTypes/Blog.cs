using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class BlogInfo : ITumblrParsable
	{
		/// <summary>
		/// The display title of the blog
		/// </summary>
		public string Title { get; protected set; }
		/// <summary>
		/// The total number of posts to this blog
		/// </summary>
		public int Posts { get; protected set; }
		/// <summary>
		/// The short blog name that appears before tumblr.com in a standard blog hostname (and before the domain in a custom blog hostname)
		/// </summary>
		public string Name { get; protected set; }
		public string Url { get; protected set; }
		public DateTime Updated { get; protected set; }
		/// <summary>
		/// You guessed it! The blog's description
		/// </summary>
		public string Description { get; protected set; }
		/// <summary>
		/// Indicates whether the blog allows questions
		/// </summary>
		public bool Ask { get; protected set; }
		/// <summary>
		/// Indicates whether the blog allows anonymous questions
		/// </summary>
		public bool AskAnon { get; protected set; }

		public int Likes { get; protected set; }

		/// <summary>
		/// http://developers.tumblr.com/post/35360559145/changelog-for-the-week-of-11-09-12
		/// </summary>
		public bool ShareLikes { get; protected set; }

		public bool Parse(JsonReader reader)
		{
			reader.Read();//startObject
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "blog"://special case, if we are parsing a blog info response
						Parse(reader);
						return true;
					case "title":
						this.Title = reader.ReadAsString();
						break;
					case "posts":
						this.Posts = reader.ReadAsInt32().Value;
						break;
					case "name":
						this.Name = reader.ReadAsString();
						break;
					case "url":
						this.Url = reader.ReadAsString();
						break;
					case "updated":
						this.Updated = Utility.TimestampToDateTime(reader.ReadAsInt32().Value);
						break;
					case "description":
						this.Description = reader.ReadAsString();
						break;
					case "ask":
						this.Ask = bool.Parse(reader.ReadAsString());
						break;
					case "ask_anon":
						this.AskAnon = bool.Parse(reader.ReadAsString());
						break;
					case "likes":
						this.Likes = reader.ReadAsInt32().Value;
						break;
					case "share_likes":
						this.ShareLikes = bool.Parse(reader.ReadAsString());
						break;
					default:
						throw new Exception();
				}
			}
			return true;
		}
	}
}

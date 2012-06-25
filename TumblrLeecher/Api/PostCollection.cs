using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	/// <summary>
	/// represent the datas received in the json response (blog + post array)
	/// </summary>
	public class PostCollection : System.Collections.ObjectModel.Collection<Post>, ITumblrParsable
	{
		public BlogInfo Blog { get; protected set; }
		/// <summary>
		/// The total number of post available for this request, useful for paginating through results
		/// </summary>
		public int TotalPosts { get; protected set; }

		public bool Parse(JsonReader reader)
		{
			reader.Read();//startObject
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "blog":
						this.Blog = new BlogInfo();
						this.Blog.Parse(reader);
						break;
					case "posts":
						ParsePosts(reader);
						break;
					case "total_posts":
						this.TotalPosts = reader.ReadAsInt32().Value;
						break;
					default:
						throw new Exception("unexpected value.");
				}
			}
			return true;
		}

		private bool ParsePosts(JsonReader reader)
		{
			reader.Read();//startArray
			bool result = false;
			do
			{
				Post newPost = Post.PostFactory(reader);
				//fail if endArray
				result = (newPost != null);
				if (result)
				{
					this.Add(newPost);
				}
			} while (result);
			return true;
		}

	}
}

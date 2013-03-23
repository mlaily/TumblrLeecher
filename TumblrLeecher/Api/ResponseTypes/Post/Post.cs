using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public abstract class Post 
	{
		/// <summary>
		/// The post's unique ID
		/// </summary>
		public long Id { get; internal set; }
		public string PostUrl { get; internal set; }
		public string Slug { get; internal set; }
		public PostType Type { get; internal set; }
		/// <summary>
		/// The GMT date and time of the post, as a string
		/// </summary>
		public string Date { get; internal set; }
		public DateTime Timestamp { get; internal set; }
		public PostFormat Format { get; internal set; }
		public string ReblogKey { get; internal set; }
		public List<string> Tags { get; internal set; }
		public List<string> Highlighted { get; internal set; }
		public List<string> FeaturedInTag { get; internal set; }
		public int NoteCount { get; internal set; }
		/// <summary>
		/// The URL for the source of the content (for quotes, reblogs, etc.)
		/// </summary>
		public string SourceUrl { get; internal set; }
		/// <summary>
		/// The title of the source site
		/// </summary>
		public string SourceTitle { get; internal set; }
		/// <summary>
		/// Indicates whether the post was created via mobile/email publishing
		/// </summary>
		public bool Mobile { get; internal set; }
		/// <summary>
		/// Indicates whether the post was created via the Tumblr bookmarklet
		/// </summary>
		public bool Bookmarklet { get; internal set; }

		public PostState State { get; internal set; }

		public string ShortUrl { get; internal set; }

		/// <summary>
		/// when implemented, should switch on the currentPropertyName and read and initialize own properties accordingly.
		/// should return true if datas have been read, false otherwise.
		/// </summary>
		/// <param name="currentPropertyName"></param>
		/// <param name="reader"></param>
		/// <returns></returns>
		protected abstract bool LocalSwitch(string currentPropertyName, JsonReader reader);

		public bool Parse(JsonReader reader)
		{
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "timestamp":
						this.Timestamp = Utility.TimestampToDateTime(reader.ReadAsInt32().Value);
						break;
					case "date":
						this.Date = reader.ReadAsString();
						break;
					case "format":
						this.Format = (PostFormat)Enum.Parse(typeof(PostFormat), reader.ReadAsString(), true);
						break;
					case "reblog_key":
						this.ReblogKey = reader.ReadAsString();
						break;
					case "tags":
						this.Tags = new List<string>();
						reader.Read();//startArray
						while (reader.Read() && reader.TokenType != JsonToken.EndArray)
						{
							this.Tags.Add(reader.Value.ToString());
						}
						break;
					case "highlighted":
						this.Highlighted = new List<string>();
						reader.Read();//startArray
						while (reader.Read() && reader.TokenType != JsonToken.EndArray)
						{
							this.Highlighted.Add(reader.Value.ToString());
						}
						break;
					case "featured_in_tag":
						this.FeaturedInTag = new List<string>();
						reader.Read();//startArray
						while (reader.Read() && reader.TokenType != JsonToken.EndArray)
						{
							this.FeaturedInTag.Add(reader.Value.ToString());
						}
						break;
					case "bookmarklet":
						this.Bookmarklet = bool.Parse(reader.ReadAsString());
						break;
					case "mobile":
						this.Mobile = bool.Parse(reader.ReadAsString());
						break;
					case "source_url":
						this.SourceUrl = reader.ReadAsString();
						break;
					case "source_title":
						this.SourceTitle = reader.ReadAsString();
						break;
					case "note_count":
						this.NoteCount = reader.ReadAsInt32().Value;
						break;
					case "state":
						this.State = (PostState)Enum.Parse(typeof(PostState), reader.ReadAsString(), true);
						break;
					case "short_url":
						this.ShortUrl = reader.ReadAsString();
						break;
					default:
						//call the implementation from derived class
						string value = reader.Value.ToString();
						bool result = LocalSwitch(value, reader);
						if (!result)
						{
							throw new Exception(string.Format("unexpected value \"{0}\"", value));
						}
						break;
				}
			}

			return true;
		}

		/// <summary>
		/// because we don't know the type of the post before we encounter the "type" field, we need this factory method...
		/// return null on error (e.g. end of array)
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static Post PostFactory(JsonReader reader)
		{
			Post newPost;
			//we assume/hope only simple fields (no array/object) will come before the "type" field...
			Dictionary<string, string> postDatas = new Dictionary<string, string>();
			reader.Read();//startObject
			if (reader.TokenType == JsonToken.EndArray)
			{
				return null;
			}
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					default:
						postDatas.Add(reader.Value.ToString(), reader.ReadAsString());
						break;
					case "type":
						PostType type = (PostType)Enum.Parse(typeof(PostType), reader.ReadAsString(), true);
						switch (type)
						{
							case PostType.Text:
								newPost = new TextPost();
								break;
							case PostType.Quote:
								newPost = new QuotePost();
								break;
							case PostType.Link:
								newPost = new LinkPost();
								break;
							case PostType.Answer:
								newPost = new AnswerPost();
								break;
							case PostType.Video:
								newPost = new VideoPost();
								break;
							case PostType.Audio:
								newPost = new AudioPost();
								break;
							case PostType.Photo:
								newPost = new PhotoPost();
								break;
							case PostType.Chat:
								newPost = new ChatPost();
								break;
							default:
								throw new Exception();
						}
						newPost.Type = type;
						newPost.InitializeFromDictionary(postDatas);
						//we continue the parsing
						newPost.Parse(reader);
						return newPost;
				}
			}
			throw new Exception("we should not be here");
		}

		private bool InitializeFromDictionary(Dictionary<string, string> commonValues)
		{
			foreach (var pair in commonValues)
			{
				switch (pair.Key)
				{
					case "blog_name"://don't care
						break;
					case "id":
						this.Id = long.Parse(pair.Value);
						break;
					case "post_url":
						this.PostUrl = pair.Value;
						break;
					case "slug":
						this.Slug = pair.Value;
						break;
					default:
						throw new Exception("unknown value");
				}
			}
			return true;
		}

	}

	public enum PostType
	{
		None,
		Text,
		Quote,
		Link,
		Answer,
		Video,
		Audio,
		Photo,
		Chat,
	}

	public enum PostFormat
	{
		Html,
		Markdown,
	}

	public enum PostState
	{
		Published,
		Queued,
		Draft,
		Private,
	}
}

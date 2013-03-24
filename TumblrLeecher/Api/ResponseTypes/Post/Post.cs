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
		public long NoteCount { get; internal set; }
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

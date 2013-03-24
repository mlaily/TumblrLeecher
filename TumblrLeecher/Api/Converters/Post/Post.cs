using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private AnswerPost ParseBasePost(JObject jObject, HashSet<string> checkedProperties)
		{
			AnswerPost newPost = new AnswerPost();
			JToken current;
			//not used.
			CheckProperty(jObject, "blog_name", checkedProperties, out current);
			//parse the properties common to all post types.
			if (CheckProperty(jObject, "id", checkedProperties, out current))
			{
				newPost.Id = (long)current;
			}
			if (CheckProperty(jObject, "post_url", checkedProperties, out current))
			{
				newPost.PostUrl = (string)current;
			}
			if (CheckProperty(jObject, "slug", checkedProperties, out current))
			{
				newPost.Slug = (string)current;
			}

			if (CheckProperty(jObject, "timestamp", checkedProperties, out current))
			{
				newPost.Timestamp = Utility.TimestampToDateTime((long)current);
			}
			if (CheckProperty(jObject, "date", checkedProperties, out current))
			{
				newPost.Date = (string)current;
			}
			if (CheckProperty(jObject, "format", checkedProperties, out current))
			{
				newPost.Format = (PostFormat)Enum.Parse(typeof(PostFormat), (string)current, true);
			}
			if (CheckProperty(jObject, "reblog_key", checkedProperties, out current))
			{
				newPost.ReblogKey = (string)current;
			}
			if (CheckProperty(jObject, "tags", checkedProperties, out current))
			{
				var children = current.Children();
				newPost.Tags = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "highlighted", checkedProperties, out current))
			{
				var children = current.Children();
				newPost.Highlighted = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "featured_in_tag", checkedProperties, out current))
			{
				var children = current.Children();
				newPost.FeaturedInTag = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "bookmarklet", checkedProperties, out current))
			{
				newPost.Bookmarklet = (bool)current;
			}
			if (CheckProperty(jObject, "source_url", checkedProperties, out current))
			{
				newPost.SourceUrl = (string)current;
			}
			if (CheckProperty(jObject, "source_title", checkedProperties, out current))
			{
				newPost.SourceTitle = (string)current;
			}
			if (CheckProperty(jObject, "note_count", checkedProperties, out current))
			{
				newPost.NoteCount = (long)current;
			}
			if (CheckProperty(jObject, "state", checkedProperties, out current))
			{
				newPost.State = (PostState)Enum.Parse(typeof(PostState), (string)current, true);
			}
			if (CheckProperty(jObject, "short_url", checkedProperties, out current))
			{
				newPost.ShortUrl = (string)current;
			}
			if (CheckProperty(jObject, "mobile", checkedProperties, out current))
			{
				newPost.Mobile = (bool)current;
			}
			return newPost;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private void ParseBasePost(JObject jObject, HashSet<string> checkedProperties, Post toPopulate)
		{
			JToken current;
			//not used.
			CheckProperty(jObject, "blog_name", checkedProperties, out current);
			//parse the properties common to all post types.
			if (CheckProperty(jObject, "id", checkedProperties, out current))
			{
				toPopulate.Id = (long)current;
			}
			if (CheckProperty(jObject, "post_url", checkedProperties, out current))
			{
				toPopulate.PostUrl = (string)current;
			}
			if (CheckProperty(jObject, "slug", checkedProperties, out current))
			{
				toPopulate.Slug = (string)current;
			}

			if (CheckProperty(jObject, "timestamp", checkedProperties, out current))
			{
				toPopulate.Timestamp = Utility.TimestampToDateTime((long)current);
			}
			if (CheckProperty(jObject, "date", checkedProperties, out current))
			{
				toPopulate.Date = (string)current;
			}
			if (CheckProperty(jObject, "format", checkedProperties, out current))
			{
				toPopulate.Format = (PostFormat)Enum.Parse(typeof(PostFormat), (string)current, true);
			}
			if (CheckProperty(jObject, "reblog_key", checkedProperties, out current))
			{
				toPopulate.ReblogKey = (string)current;
			}
			if (CheckProperty(jObject, "tags", checkedProperties, out current))
			{
				var children = current.Children();
				toPopulate.Tags = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "highlighted", checkedProperties, out current))
			{
				var children = current.Children();
				toPopulate.Highlighted = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "featured_in_tag", checkedProperties, out current))
			{
				var children = current.Children();
				toPopulate.FeaturedInTag = children.Select(x => (string)x).ToList();
			}
			if (CheckProperty(jObject, "bookmarklet", checkedProperties, out current))
			{
				toPopulate.Bookmarklet = (bool)current;
			}
			if (CheckProperty(jObject, "source_url", checkedProperties, out current))
			{
				toPopulate.SourceUrl = (string)current;
			}
			if (CheckProperty(jObject, "source_title", checkedProperties, out current))
			{
				toPopulate.SourceTitle = (string)current;
			}
			if (CheckProperty(jObject, "note_count", checkedProperties, out current))
			{
				toPopulate.NoteCount = (long)current;
			}
			if (CheckProperty(jObject, "state", checkedProperties, out current))
			{
				toPopulate.State = (PostState)Enum.Parse(typeof(PostState), (string)current, true);
			}
			if (CheckProperty(jObject, "short_url", checkedProperties, out current))
			{
				toPopulate.ShortUrl = (string)current;
			}
			if (CheckProperty(jObject, "mobile", checkedProperties, out current))
			{
				toPopulate.Mobile = (bool)current;
			}
		}

	}
}

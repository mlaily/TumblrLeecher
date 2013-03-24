using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private LinkPost ParseLinkPost(JObject jObject, HashSet<string> checkedProperties)
		{
			LinkPost newPost = new LinkPost();
			JToken current;
			if (CheckProperty(jObject, "title", checkedProperties, out current))
			{
				newPost.Title = (string)current;
			}
			if (CheckProperty(jObject, "url", checkedProperties, out current))
			{
				newPost.Url = (string)current;
			}
			if (CheckProperty(jObject, "description", checkedProperties, out current))
			{
				newPost.Description = (string)current;
			}
			return newPost;
		}

	}
}

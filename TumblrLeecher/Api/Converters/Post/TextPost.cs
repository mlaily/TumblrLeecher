using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private TextPost ParseTextPost(JObject jObject, HashSet<string> checkedProperties)
		{
			TextPost newPost = new TextPost();
			JToken current;
			if (CheckProperty(jObject, "title", checkedProperties, out current))
			{
				newPost.Title = (string)current;
			}
			if (CheckProperty(jObject, "body", checkedProperties, out current))
			{
				newPost.Body = (string)current;
			}
			return newPost;
		}

	}
}

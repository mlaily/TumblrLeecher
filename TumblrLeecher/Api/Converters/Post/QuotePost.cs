using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private QuotePost ParseQuotePost(JObject jObject, HashSet<string> checkedProperties)
		{
			QuotePost newPost = new QuotePost();
			JToken current;
			if (CheckProperty(jObject, "text", checkedProperties, out current))
			{
				newPost.Text = (string)current;
			}
			if (CheckProperty(jObject, "source", checkedProperties, out current))
			{
				newPost.Source = (string)current;
			}
			return newPost;
		}

	}
}

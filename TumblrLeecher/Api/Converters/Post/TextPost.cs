using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private AnswerPost ParseTextPost(JObject jObject, HashSet<string> checkedProperties)
		{
			AnswerPost newPost = new AnswerPost();
			JToken current;
			if (CheckProperty(jObject, "type", checkedProperties, out current))
			{

			}
			return newPost;
		}

	}
}

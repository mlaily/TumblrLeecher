using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private AnswerPost ParseAnswerPost(JObject jObject, HashSet<string> checkedProperties)
		{
			AnswerPost newPost = new AnswerPost();
			JToken current;
			if (CheckProperty(jObject, "asking_name", checkedProperties, out current))
			{
				newPost.AskingName = (string)current;
			}
			if (CheckProperty(jObject, "asking_url", checkedProperties, out current))
			{
				newPost.AskingUrl = (string)current;
			}
			if (CheckProperty(jObject, "question", checkedProperties, out current))
			{
				newPost.Question = (string)current;
			}
			if (CheckProperty(jObject, "answer", checkedProperties, out current))
			{
				newPost.Answer = (string)current;
			}
			return newPost;
		}

	}
}

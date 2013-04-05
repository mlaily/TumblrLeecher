using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private ChatPost ParseChatPost(JObject jObject, HashSet<string> checkedProperties)
		{
			ChatPost newPost = new ChatPost();
			JToken current;
			if (CheckProperty(jObject, "title", checkedProperties, out current))
			{
				newPost.Title = (string)current;
			}
			if (CheckProperty(jObject, "body", checkedProperties, out current))
			{
				newPost.Body = (string)current;
			}
			if (CheckProperty(jObject, "dialogue", checkedProperties, out current))
			{
				newPost.Dialogue = new List<Dialogue>();
				var children = current.Children();
				foreach (JObject child in children)
				{
					newPost.Dialogue.Add(ParseDialogue(child));
				}
			}
			return newPost;
		}

		private Dialogue ParseDialogue(JObject jObject)
		{
			Dialogue newDialogue = new Dialogue();
			foreach (var property in jObject.Properties())
			{
				switch (property.Name)
				{
					case "name":
						newDialogue.Name = (string)property.Value;
						break;
					case "label":
						newDialogue.Label = (string)property.Value;
						break;
					case "phrase":
						newDialogue.Phrase = (string)property.Value;
						break;
					default:
						throw new NotImplementedException("unexpected value in a dialogue object:\n" + property.ToString());
				}
			}
			return newDialogue;
		}

	}
}

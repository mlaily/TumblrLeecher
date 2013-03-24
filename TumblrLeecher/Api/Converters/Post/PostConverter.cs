using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(Post).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			var result = Build(jObject);
			return result;
		}

		private Post Build(JObject jObject)
		{
			HashSet<string> checkedProperties = new HashSet<string>();
			JToken current;
			Post newPost;
			PostType postType;
			//initialize the post instance according to its type.
			if (CheckProperty(jObject, "type", checkedProperties, out current))
			{
				postType = (PostType)Enum.Parse(typeof(PostType), (string)current, true);
				switch (postType)
				{
					case PostType.Text:
						newPost = ParseTextPost(jObject, checkedProperties);
						break;
					case PostType.Quote:
						newPost = ParseQuotePost(jObject, checkedProperties);
						break;
					case PostType.Link:
						newPost = ParseLinkPost(jObject, checkedProperties);
						break;
					case PostType.Answer:
						newPost = ParseAnswerPost(jObject, checkedProperties);
						break;
					case PostType.Video:
						newPost = ParseVideoPost(jObject, checkedProperties);
						break;
					case PostType.Audio:
						newPost = ParseAudioPost(jObject, checkedProperties);
						break;
					case PostType.Photo:
						newPost = ParsePhotoPost(jObject, checkedProperties);
						break;
					case PostType.Chat:
						newPost = ParseChatPost(jObject, checkedProperties);
						break;
					default:
						throw new NotImplementedException("unexpected post type.");
				}
			}
			else
			{
				throw new NotImplementedException("property \"type\" not found in post.");
			}

			ParseBasePost(jObject, checkedProperties);

			//check for unknown properties and throw an exception if any is found.
			foreach (var property in jObject.Properties())
			{
				if (!checkedProperties.Contains(property.Name))
				{
					throw new Exception(string.Format("Warning: The property \"{0}\" was not handled. Post type: {1}.\nProperty value:\n{2}",
						property.Name, postType, property.Value));
				}
			}

			return newPost;
		}

		private bool CheckProperty(JObject jObject, string name, HashSet<string> checkedProperties, out JToken current)
		{
			checkedProperties.Add(name);
			current = jObject[name];
			return current != null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

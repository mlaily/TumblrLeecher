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
						newPost = new TextPost();
						break;
					case PostType.Quote:
						newPost = new QuotePost();
						break;
					case PostType.Link:
						newPost = new LinkPost();
						break;
					case PostType.Answer:
						newPost = ParseAnswerPost(jObject, checkedProperties);
						break;
					case PostType.Video:
						newPost = new VideoPost();
						break;
					case PostType.Audio:
						newPost = new AudioPost();
						break;
					case PostType.Photo:
						newPost = new PhotoPost();
						break;
					case PostType.Chat:
						newPost = new ChatPost();
						break;
					default:
						throw new NotImplementedException("unexpected post type.");
				}
			}
			else
			{
				throw new NotImplementedException("property \"type\" not found in post.");
			}

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
			//not used.
			CheckProperty(jObject, "blog_name", checkedProperties, out current);

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

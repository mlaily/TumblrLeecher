using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal class PostCollectionConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(PostCollection).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			var result = Build(jObject, serializer);
			return result;
		}

		private PostCollection Build(JObject jObject, JsonSerializer serializer)
		{
			PostCollection result = new PostCollection();
			var posts = jObject["posts"];
			JToken current;

			if ((current = posts["total_posts"]) != null)
			{
				result.TotalPosts = (long)current;
			}
			if ((current = posts["blog"]) != null)
			{
				result.Blog = serializer.Deserialize<BlogInfo>(jObject.CreateReader());
			}
			if ((current = posts["posts"]) != null)
			{
				var newPost = serializer.Deserialize<Post>(jObject.CreateReader());
			}
			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

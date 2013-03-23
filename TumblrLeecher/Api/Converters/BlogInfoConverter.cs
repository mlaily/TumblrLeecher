using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal class BlogInfoConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(BlogInfo).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			var result = Build(jObject);
			return result;
		}

		private Post Build(JObject jObject)
		{
			var property = jObject["property"];
			JToken current;
			//if ((current = property["name"]) != null)
			//{
			//    node.Name = (string)current;
			//}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

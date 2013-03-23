using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal class ResponseConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsGenericType)
			{
				return false;
			}
			Type genericDefinition = objectType.GetGenericTypeDefinition();
			if (genericDefinition != null && typeof(Response<>).IsAssignableFrom(genericDefinition))
			{
				return GetGenericArgument(genericDefinition) != null;
			}
			return false;
		}

		private Type GetGenericArgument(Type responseType)
		{
			var genericArguments = responseType.GetGenericArguments();
			if (genericArguments != null && genericArguments.Length == 1)
			{
				return genericArguments[0];
			}
			else
			{
				throw new ArgumentException("The provided parameter must be a Response<T>", "responseType");
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);

			Type responseType = GetGenericArgument(objectType);
			if (typeof(PostCollection) == responseType)
			{
				return ParseResponse<PostCollection>(jObject, serializer);
			}
			else if (typeof(BlogInfo) == responseType)
			{
				return ParseResponse<BlogInfo>(jObject, serializer);
			}
			else
			{
				throw new NotImplementedException("There is no converter for the requested response.");
			}
		}

		private Response<T> ParseResponse<T>(JObject jObject, JsonSerializer serializer)
		{
			var result = new Response<T>();
			var meta = jObject["meta"];
			var response = jObject["response"];
			JToken current;

			if ((current = meta["status"]) != null)
			{
				result.MetaStatus = (long)current;
			}
			if ((current = meta["msg"]) != null)
			{
				result.MetaMessage = (string)current;
			}
			result.Content = serializer.Deserialize<T>(response.CreateReader());

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

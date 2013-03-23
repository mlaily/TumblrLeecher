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

		private BlogInfo Build(JObject jObject)
		{
			BlogInfo result = new BlogInfo();
			var blog = jObject["blog"];
			JToken current;

			if ((current = blog["title"]) != null)
			{
				result.Title = (string)current;
			}
			if ((current = blog["posts"]) != null)
			{
				result.Posts = (long)current;
			}
			if ((current = blog["name"]) != null)
			{
				result.Name = (string)current;
			}
			if ((current = blog["url"]) != null)
			{
				result.Url = (string)current;
			}
			if ((current = blog["updated"]) != null)
			{
				result.Updated = Utility.TimestampToDateTime((long)current);
			}
			if ((current = blog["description"]) != null)
			{
				result.Description = (string)current;
			}
			if ((current = blog["ask"]) != null)
			{
				result.Ask = (bool)current;
			}
			if ((current = blog["ask_anon"]) != null)
			{
				result.AskAnon = (bool)current;
			}
			if ((current = blog["likes"]) != null)
			{
				result.Likes = (long)current;
			}
			if ((current = blog["share_likes"]) != null)
			{
				result.ShareLikes = (bool)current;
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace TumblrLeecher.Api
{
	/// <summary>
	/// represent the response object, in the json tumblr send us.
	/// include the two meta properties.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Response<T> where T : ITumblrParsable, new()
	{
		public int MetaStatus { get; protected set; }
		public string MetaMessage { get; protected set; }

		public T Content { get; protected set; }

		public Response(string rawJson)
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(rawJson));
			Parse(reader);
		}

		public Response(Stream stream)
		{
			using (var txtReader = new StreamReader(stream))
			{
				using (var jsonReader = new Newtonsoft.Json.JsonTextReader(txtReader))
				{
					Parse(jsonReader);
				}
			}
		}

		protected void Parse(JsonReader reader)
		{
			reader.Read();//startObject
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "meta":
						ParseMeta(reader);
						break;
					case "response":
						this.Content = new T();
						this.Content.Parse(reader);
						break;
					default:
						throw new Exception("unexpected value.");
				}
			}
		}

		protected void ParseMeta(JsonReader reader)
		{
			reader.Read();//startObject
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "status":
						this.MetaStatus = reader.ReadAsInt32().Value;
						break;
					case "msg":
						this.MetaMessage = reader.ReadAsString();
						break;
					default:
						throw new Exception();
				}
			}
		}

	}
}

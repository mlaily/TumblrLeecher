using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class ChatPost : Post
	{
		public string Title { get; protected set; }
		public string Body { get; protected set; }
		public List<Dialogue> Dialogue { get; protected set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "title":
					this.Title = reader.ReadAsString();
					break;
				case "body":
					this.Body = reader.ReadAsString();
					break;
				case "dialogue":
					ParseDialogues(reader);
					break;
				default:
					return false;
			}
			return true;
		}

		private bool ParseDialogues(JsonReader reader)
		{
			this.Dialogue = new List<Api.Dialogue>();
			reader.Read();//startArray
			bool result = false;
			do
			{
				Dialogue dialogue = new Api.Dialogue();
				//fail if endArray
				result = dialogue.Parse(reader);
				if (result)
				{
					this.Dialogue.Add(dialogue);
				}
			} while (result);
			return true;
		}
	}

	public class Dialogue : ITumblrParsable
	{
		public string Name { get; protected set; }
		public string Label { get; protected set; }
		public string Phrase { get; protected set; }

		public bool Parse(JsonReader reader)
		{
			reader.Read();//startObject
			if (reader.TokenType == JsonToken.EndArray)
			{
				return false;
			}
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "name":
						this.Name = reader.ReadAsString();
						break;
					case "label":
						this.Label = reader.ReadAsString();
						break;
					case "phrase":
						this.Phrase = reader.ReadAsString();
						break;
					default:
						throw new Exception("unexpected value");
				}
			}
			return true;
		}
	}
}

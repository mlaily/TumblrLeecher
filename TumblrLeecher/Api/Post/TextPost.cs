using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class TextPost : Post
	{
		public string Title { get; protected set; }
		public string Body { get; protected set; }

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
				default:
					return false;
			}
			return true;
		}
	}
}

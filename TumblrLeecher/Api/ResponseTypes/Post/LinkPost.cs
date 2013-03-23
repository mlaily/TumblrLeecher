using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class LinkPost : Post
	{
		public string Title { get; protected set; }
		public string Url { get; protected set; }
		public string Description { get; protected set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "title":
					this.Title = reader.ReadAsString();
					break;
				case "url":
					this.Url = reader.ReadAsString();
					break;
				case "description":
					this.Description = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}

	}
}

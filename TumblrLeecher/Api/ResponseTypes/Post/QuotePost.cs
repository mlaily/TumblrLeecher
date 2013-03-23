using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class QuotePost : Post
	{
		/// <summary>
		/// The text of the quote (can be modified by the user when posting)
		/// </summary>
		public string Text { get; protected set; }
		/// <summary>
		/// Full HTML for the source of the quote
		/// Example: <a href="...">Steve Jobs</a>
		/// </summary>
		public string Source { get; protected set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "text":
					this.Text = reader.ReadAsString();
					break;
				case "source":
					this.Source = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}
	}
}

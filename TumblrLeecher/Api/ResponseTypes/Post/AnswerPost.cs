using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class AnswerPost : Post
	{
		/// <summary>
		/// The blog name of the user asking the question
		/// </summary>
		public string AskingName { get; protected set; }
		/// <summary>
		/// The blog URL of the user asking the question
		/// </summary>
		public string AskingUrl { get; protected set; }
		public string Question { get; protected set; }
		public string Answer { get; protected set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "asking_name":
					this.AskingName = reader.ReadAsString();
					break;
				case "asking_url":
					this.AskingName = reader.ReadAsString();
					break;
				case "question":
					this.AskingName = reader.ReadAsString();
					break;
				case "answer":
					this.AskingName = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}
	}
}

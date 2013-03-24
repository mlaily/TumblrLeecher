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
		public string AskingName { get; internal set; }
		/// <summary>
		/// The blog URL of the user asking the question
		/// </summary>
		public string AskingUrl { get; internal set; }
		public string Question { get; internal set; }
		public string Answer { get; internal set; }
	}
}

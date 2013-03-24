using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class ChatPost : Post
	{
		public string Title { get; internal set; }
		public string Body { get; internal set; }
		public List<Dialogue> Dialogue { get; internal set; }
	}

	public class Dialogue 
	{
		public string Name { get; internal set; }
		public string Label { get; internal set; }
		public string Phrase { get; internal set; }
	}
}

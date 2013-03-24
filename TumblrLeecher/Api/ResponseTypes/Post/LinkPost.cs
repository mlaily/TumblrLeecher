using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class LinkPost : Post
	{
		public string Title { get; internal set; }
		public string Url { get; internal set; }
		public string Description { get; internal set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class TextPost : Post
	{
		public string Title { get; internal set; }
		public string Body { get; internal set; }
	}
}

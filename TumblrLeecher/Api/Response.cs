using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace TumblrLeecher.Api
{
	/// <summary>
	/// represent the response object, in the json tumblr sends us.
	/// include the two meta properties.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Response<T>
	{
		public long MetaStatus { get; internal set; }
		public string MetaMessage { get; internal set; }

		public T Content { get; internal set; }

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TumblrLeecher.Api
{
	/// <summary>
	/// represent the datas received in the json response (blog + post array)
	/// </summary>
	public class PostCollection : Collection<Post>
	{
		public BlogInfo Blog { get; internal set; }
		/// <summary>
		/// The total number of post available for this request, useful for paginating through results
		/// </summary>
		public long TotalPosts { get; internal set; }
	}
}

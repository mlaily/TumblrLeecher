using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class VideoPost : Post
	{

		public string Caption { get; internal set; }
		public List<Player> Player { get; internal set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public string PermalinkUrl { get; internal set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public string ThumbnailUrl { get; internal set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public decimal ThumbnailWidth { get; internal set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public decimal ThumbnailHeight { get; internal set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public bool Html5Capable { get; internal set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string VideoUrl { get; internal set; }

		public decimal Duration { get; internal set; }
	}

	public class Player
	{
		/// <summary>
		/// width of video player, in pixels
		/// </summary>
		public long Width { get; internal set; }
		/// <summary>
		/// HTML for embedding the video player
		/// </summary>
		public string EmbedCode { get; internal set; }
	}
}

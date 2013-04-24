using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class AudioPost : Post
	{
		public string Caption { get; internal set; }
		/// <summary>
		/// HTML for embedding the audio player
		/// </summary>
		public string Player { get; internal set; }
		/// <summary>
		/// Number of times the audio post has been played
		/// </summary>
		public long Plays { get; internal set; }
		/// <summary>
		/// Location of the audio file's ID3 album art image
		/// </summary>
		public string AlbumArt { get; internal set; }
		public string Artist { get; internal set; }
		public string Album { get; internal set; }
		public string TrackName { get; internal set; }
		public long TrackNumber { get; internal set; }
		public long Year { get; internal set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string AudioUrl { get; internal set; }

		/// <summary>
		/// undocumented
		/// ex: "track":"7 of 11"
		/// </summary>
		public string Track { get; internal set; }

		public string Embed { get; internal set; }
	}
}

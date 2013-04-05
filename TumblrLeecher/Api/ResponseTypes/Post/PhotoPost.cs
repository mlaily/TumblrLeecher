using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TumblrLeecher.Api;

namespace TumblrLeecher.Api
{
	public class PhotoPost : Post
	{
		//Multi-photo Photo posts, called Photosets, will send return multiple photo objects in the photos array.
		public List<Photo> Photos { get; internal set; }
		/// <summary>
		/// The user-supplied caption
		/// </summary>
		public string Caption { get; internal set; }
		/// <summary>
		/// The width of the photo or photoset
		/// </summary>
		public long Width { get; internal set; }
		/// <summary>
		/// The height of the photo or photoset
		/// </summary>
		public long Height { get; internal set; }

		/// <summary>
		/// undocumented
		/// http://groups.google.com/group/tumblr-themes/tree/browse_frm/month/2012-03/bccb783366fbf5fb?rnum=11&_done=%2Fgroup%2Ftumblr-themes%2Fbrowse_frm%2Fmonth%2F2012-03%3F
		/// </summary>
		public string PhotoSetLayout { get; internal set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string LinkUrl { get; internal set; }

		public string ImagePermalink { get; internal set; }
	}

	public class Photo
	{
		/// <summary>
		/// user supplied caption for the individual photo (Photosets only)
		/// </summary>
		public string Caption { get; internal set; }
		public List<PhotoSize> AltSizes { get; internal set; }
		public PhotoSize OriginalSize { get; internal set; }
		public PhotoExif Exif { get; set; }
	}

	public class PhotoSize
	{
		public long Width { get; internal set; }
		public long Height { get; internal set; }
		public string Url { get; internal set; }
	}

	public class PhotoExif
	{
		public string Camera { get; set; }
		public long ISO { get; set; }
		public string Aperture { get; set; }
		public string Exposure { get; set; }
		public string FocalLength { get; set; }
	}
}

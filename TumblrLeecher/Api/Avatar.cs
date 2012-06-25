using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace TumblrLeecher.Api
{
	public class Avatar
	{
		public string AvatarUrl { get; protected set; }
		public System.Drawing.Bitmap Image { get; protected set; }

		public Avatar(WebResponse response)
		{
			this.AvatarUrl = response.ResponseUri.ToString();
			this.Image = new System.Drawing.Bitmap(response.GetResponseStream());
		}
	}
}

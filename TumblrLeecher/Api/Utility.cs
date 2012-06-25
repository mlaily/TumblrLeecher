using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TumblrLeecher.Api
{
	public static class Utility
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1);
		public static DateTime TimestampToDateTime(int timestamp)
		{
			return EPOCH + new TimeSpan(0, 0, timestamp);
		}
	}
}

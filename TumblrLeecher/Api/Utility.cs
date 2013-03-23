using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace TumblrLeecher.Api
{
	internal static class Utility
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1);
		internal static DateTime TimestampToDateTime(int timestamp)
		{
			return EPOCH + new TimeSpan(0, 0, timestamp);
		}

		internal static HttpWebResponse TryGetResponse(HttpWebRequest request, out string responseBody)
		{
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
				using (var sr = new StreamReader(response.GetResponseStream()))
				{
					responseBody = sr.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					request.Abort();
					throw new TimeoutException(ex.Message, ex);
				}
				else
				{
					using (ex.Response)
					using (var sr = new StreamReader(ex.Response.GetResponseStream()))
					{
						responseBody = sr.ReadToEnd();
					}
					throw;
				}
			}
			catch (Exception)
			{
				throw;
			}
			return response;
		}
	}
}

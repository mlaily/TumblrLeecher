using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public interface ITumblrParsable
	{
		/// <summary>
		/// Must return false on error (e.g: endArray)
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		bool Parse(JsonReader reader);
	}
}

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
		public List<Photo> Photos { get; protected set; }
		/// <summary>
		/// The user-supplied caption
		/// </summary>
		public string Caption { get; protected set; }
		/// <summary>
		/// The width of the photo or photoset
		/// </summary>
		public int Width { get; protected set; }
		/// <summary>
		/// The height of the photo or photoset
		/// </summary>
		public int Height { get; protected set; }

		/// <summary>
		/// undocumented
		/// http://groups.google.com/group/tumblr-themes/tree/browse_frm/month/2012-03/bccb783366fbf5fb?rnum=11&_done=%2Fgroup%2Ftumblr-themes%2Fbrowse_frm%2Fmonth%2F2012-03%3F
		/// </summary>
		public string PhotoSetLayout { get; set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string LinkUrl { get; protected set; }

		public string ImagePermalink { get; set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "photos":
					ParsePhotos(reader);
					break;
				case "caption":
					this.Caption = reader.ReadAsString();
					break;
				case "width":
					this.Width = reader.ReadAsInt32().Value;
					break;
				case "height":
					this.Height = reader.ReadAsInt32().Value;
					break;
				case "link_url":
					this.LinkUrl = reader.ReadAsString();
					break;
				case "photoset_layout":
					this.PhotoSetLayout = reader.ReadAsString();
					break;
				case "image_permalink":
					this.ImagePermalink = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}

		private bool ParsePhotos(JsonReader reader)
		{
			this.Photos = new List<Photo>();
			reader.Read();//startArray
			bool result = false;
			do
			{
				Photo photo = new Photo();
				//fail if endArray
				result = photo.Parse(reader);
				if (result)
				{
					this.Photos.Add(photo);
				}
			} while (result);
			return true;
		}
	}

	public class Photo : ITumblrParsable
	{
		/// <summary>
		/// user supplied caption for the individual photo (Photosets only)
		/// </summary>
		public string Caption { get; protected set; }
		public List<PhotoSize> AltSizes { get; protected set; }
		public PhotoSize OriginalSize { get; protected set; }


		public bool Parse(JsonReader reader)
		{
			reader.Read();//startObject
			if (reader.TokenType == JsonToken.EndArray)
			{
				return false;
			}
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "caption":
						this.Caption = reader.ReadAsString();
						break;
					case "alt_sizes":
						ParseAltSizes(reader);
						break;
					case "original_size":
						this.OriginalSize = new PhotoSize();
						this.OriginalSize.Parse(reader);
						break;
					default:
						throw new Exception("unexpected value");
				}
			}
			return true;
		}

		private bool ParseAltSizes(JsonReader reader)
		{
			this.AltSizes = new List<PhotoSize>();
			reader.Read();//startArray
			bool result = false;
			do
			{
				PhotoSize size = new PhotoSize();
				//fail if endArray
				result = size.Parse(reader);
				if (result)
				{
					this.AltSizes.Add(size);
				}
			} while (result);
			return true;
		}
	}

	public class PhotoSize : ITumblrParsable
	{
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		public string Url { get; protected set; }

		public bool Parse(JsonReader reader)
		{
			reader.Read();//startObject
			if (reader.TokenType == JsonToken.EndArray)
			{
				return false;
			}
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				switch (reader.Value.ToString())
				{
					case "width":
						this.Width = reader.ReadAsInt32().Value;
						break;
					case "height":
						this.Height = reader.ReadAsInt32().Value;
						break;
					case "url":
						this.Url = reader.ReadAsString();
						break;
					default:
						throw new Exception("unexpected value");
				}
			}
			return true;
		}
	}
}

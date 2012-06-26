using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class VideoPost : Post
	{

		public string Caption { get; protected set; }
		public List<Player> Player { get; protected set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public string PermalinkUrl { get; protected set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public string ThumbnailUrl { get; protected set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public decimal ThumbnailWidth { get; protected set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public decimal ThumbnailHeight { get; protected set; }
		/// <summary>
		/// undocumented
		/// </summary>
		public bool Html5Capable { get; protected set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string VideoUrl { get; set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "caption":
					this.Caption = reader.ReadAsString();
					break;
				case "player":
					ParsePlayers(reader);
					break;
				case "permalink_url":
					this.PermalinkUrl = reader.ReadAsString();
					break;
				case "thumbnail_url":
					this.ThumbnailUrl = reader.ReadAsString();
					break;
				case "thumbnail_width":
					this.ThumbnailWidth = reader.ReadAsDecimal().Value;
					break;
				case "thumbnail_height":
					this.ThumbnailHeight = reader.ReadAsDecimal().Value;
					break;
				case "html5_capable":
					this.Html5Capable = bool.Parse(reader.ReadAsString());
					break;
				case "video_url":
					this.VideoUrl = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}

		private bool ParsePlayers(JsonReader reader)
		{
			this.Player = new List<Player>();
			reader.Read();//startArray
			bool result = false;
			do
			{
				Player player = new Player();
				//fail if endArray
				result = player.Parse(reader);
				if (result)
				{
					this.Player.Add(player);
				}
			} while (result);
			return true;
		}
	}

	public class Player : ITumblrParsable
	{
		/// <summary>
		/// width of video player, in pixels
		/// </summary>
		public int Width { get; protected set; }
		/// <summary>
		/// HTML for embedding the video player
		/// </summary>
		public string EmbedCode { get; protected set; }

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
					case "embed_code":
						this.EmbedCode = reader.ReadAsString();
						break;
					default:
						throw new Exception("unexpected value");
				}
			}
			return true;
		}
	}
}

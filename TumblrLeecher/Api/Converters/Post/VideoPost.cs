using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private VideoPost ParseVideoPost(JObject jObject, HashSet<string> checkedProperties)
		{
			VideoPost newPost = new VideoPost();
			JToken current;
			if (CheckProperty(jObject, "caption", checkedProperties, out current))
			{
				newPost.Caption = (string)current;
			}
			if (CheckProperty(jObject, "permalink_url", checkedProperties, out current))
			{
				newPost.PermalinkUrl = (string)current;
			}
			if (CheckProperty(jObject, "thumbnail_url", checkedProperties, out current))
			{
				newPost.ThumbnailUrl = (string)current;
			}
			if (CheckProperty(jObject, "thumbnail_width", checkedProperties, out current))
			{
				newPost.ThumbnailWidth = (decimal)current;
			}
			if (CheckProperty(jObject, "thumbnail_height", checkedProperties, out current))
			{
				newPost.ThumbnailHeight = (decimal)current;
			}
			if (CheckProperty(jObject, "html5_capable", checkedProperties, out current))
			{
				newPost.Html5Capable = (bool)current;
			}
			if (CheckProperty(jObject, "video_url", checkedProperties, out current))
			{
				newPost.VideoUrl = (string)current;
			}
			if (CheckProperty(jObject, "duration", checkedProperties, out current))
			{
				newPost.Duration = (decimal)current;
			}
			if (CheckProperty(jObject, "player", checkedProperties, out current))
			{
				newPost.Player = new List<Player>();
				var children = current.Children();
				foreach (JObject child in children)
				{
					newPost.Player.Add(ParsePlayer(child));
				}
			}
			return newPost;
		}

		private Player ParsePlayer(JObject jObject)
		{
			Player newPlayer = new Player();
			foreach (var property in jObject.Properties())
			{
				switch (property.Name)
				{
					case "width":
						newPlayer.Width = (long)property.Value;
						break;
					case "embed_code":
						newPlayer.EmbedCode = (string)property.Value;
						break;
					default:
						throw new NotImplementedException("unexpected value in a player object:\n" + property.ToString());
				}
			}
			return newPlayer;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private AudioPost ParseAudioPost(JObject jObject, HashSet<string> checkedProperties)
		{
			AudioPost newPost = new AudioPost();
			JToken current;
			if (CheckProperty(jObject, "caption", checkedProperties, out current))
			{
				newPost.Caption = (string)current;
			}
			if (CheckProperty(jObject, "player", checkedProperties, out current))
			{
				newPost.Player = (string)current;
			}
			if (CheckProperty(jObject, "plays", checkedProperties, out current))
			{
				newPost.Plays = (long?)current ?? 0;
			}
			if (CheckProperty(jObject, "album_art", checkedProperties, out current))
			{
				newPost.AlbumArt = (string)current;
			}
			if (CheckProperty(jObject, "artist", checkedProperties, out current))
			{
				newPost.Artist = (string)current;
			}
			if (CheckProperty(jObject, "album", checkedProperties, out current))
			{
				newPost.Album = (string)current;
			}
			if (CheckProperty(jObject, "track_name", checkedProperties, out current))
			{
				newPost.TrackName = (string)current;
			}
			if (CheckProperty(jObject, "track_number", checkedProperties, out current))
			{
				newPost.TrackNumber = (long)current;
			}
			if (CheckProperty(jObject, "year", checkedProperties, out current))
			{
				newPost.Year = (long)current;
			}
			if (CheckProperty(jObject, "audio_url", checkedProperties, out current))
			{
				newPost.AudioUrl = (string)current;
			}
			if (CheckProperty(jObject, "track", checkedProperties, out current))
			{
				newPost.Track = (string)current;
			}
			if (CheckProperty(jObject, "embed", checkedProperties, out current))
			{
				newPost.Embed = (string)current;
			}
			return newPost;
		}

	}
}

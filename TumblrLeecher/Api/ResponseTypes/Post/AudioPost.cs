﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TumblrLeecher.Api
{
	public class AudioPost : Post
	{
		public string Caption { get; protected set; }
		/// <summary>
		/// HTML for embedding the audio player
		/// </summary>
		public string Player { get; protected set; }
		/// <summary>
		/// Number of times the audio post has been played
		/// </summary>
		public int Plays { get; protected set; }
		/// <summary>
		/// Location of the audio file's ID3 album art image
		/// </summary>
		public string AlbumArt { get; protected set; }
		public string Artist { get; protected set; }
		public string Album { get; protected set; }
		public string TrackName { get; protected set; }
		public int TrackNumber { get; protected set; }
		public int Year { get; protected set; }

		/// <summary>
		/// undocumented
		/// </summary>
		public string AudioUrl { get; set; }

		/// <summary>
		/// undocumented
		/// ex: "track":"7 of 11"
		/// </summary>
		public string Track { get; set; }

		protected override bool LocalSwitch(string currentPropertyName, JsonReader reader)
		{
			switch (currentPropertyName)
			{
				case "caption":
					this.Caption = reader.ReadAsString();
					break;
				case "player":
					this.Player = reader.ReadAsString();
					break;
				case "plays":
					int? value;
					this.Plays = (value = reader.ReadAsInt32()) == null ? 0 : value.Value;
					break;
				case "album_art":
					this.AlbumArt = reader.ReadAsString();
					break;
				case "artist":
					this.Artist = reader.ReadAsString();
					break;
				case "album":
					this.Album = reader.ReadAsString();
					break;
				case "track_name":
					this.TrackName = reader.ReadAsString();
					break;
				case "track_number":
					this.TrackNumber = reader.ReadAsInt32().Value;
					break;
				case "year":
					this.Year = reader.ReadAsInt32().Value;
					break;
				case "audio_url":
					this.AudioUrl = reader.ReadAsString();
					break;
				case "track":
					this.Track = reader.ReadAsString();
					break;
				default:
					return false;
			}
			return true;
		}
	}
}
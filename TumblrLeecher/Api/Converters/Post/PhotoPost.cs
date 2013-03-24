using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TumblrLeecher.Api.Converters
{
	internal partial class PostConverter
	{

		private PhotoPost ParsePhotoPost(JObject jObject, HashSet<string> checkedProperties)
		{
			PhotoPost newPost = new PhotoPost();
			JToken current;
			if (CheckProperty(jObject, "caption", checkedProperties, out current))
			{
				newPost.Caption = (string)current;
			}
			if (CheckProperty(jObject, "width", checkedProperties, out current))
			{
				newPost.Width = (long)current;
			}
			if (CheckProperty(jObject, "height", checkedProperties, out current))
			{
				newPost.Height = (long)current;
			}
			if (CheckProperty(jObject, "link_url", checkedProperties, out current))
			{
				newPost.LinkUrl = (string)current;
			}
			if (CheckProperty(jObject, "photoset_layout", checkedProperties, out current))
			{
				newPost.PhotoSetLayout = (string)current;
			}
			if (CheckProperty(jObject, "image_permalink", checkedProperties, out current))
			{
				newPost.ImagePermalink = (string)current;
			}
			if (CheckProperty(jObject, "photos", checkedProperties, out current))
			{
				newPost.Photos = new List<Photo>();
				var children = current.Children();
				foreach (JObject child in children)
				{
					newPost.Photos.Add(ParsePhoto(child));
				}
			}
			return newPost;
		}

		private Photo ParsePhoto(JObject jObject)
		{
			Photo newPhoto = new Photo();
			foreach (var property in jObject.Properties())
			{
				switch (property.Name)
				{
					case "caption":
						newPhoto.Caption = (string)property.Value;
						break;
					case "original_size":
						newPhoto.OriginalSize = ParsePhotoSize((JObject)property.Value);
						break;
					case "alt_sizes":
						newPhoto.AltSizes = new List<PhotoSize>();
						var children = property.Value.Children();
						foreach (JObject child in children)
						{
							newPhoto.AltSizes.Add(ParsePhotoSize(child));
						}
						break;
					default:
						throw new NotImplementedException("unexpected value in a photo object.");
				}
			}
			return newPhoto;
		}

		private PhotoSize ParsePhotoSize(JObject jObject)
		{
			PhotoSize newPhotoSize = new PhotoSize();
			foreach (var property in jObject.Properties())
			{
				switch (property.Name)
				{
					case "width":
						newPhotoSize.Width = (long)property.Value;
						break;
					case "height":
						newPhotoSize.Height = (long)property.Value;
						break;
					case "url":
						newPhotoSize.Url = (string)property.Value;
						break;
					default:
						throw new NotImplementedException("unexpected value in a photo size object.");
				}
			}
			return newPhotoSize;
		}

	}
}

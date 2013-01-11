using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace TumblrLeecher
{

	class Program
	{
		static void Main(string[] args)
		{
			//Api.Tumblr tumblr = new Api.Tumblr("PyezS3Q4Smivb24d9SzZGYSuhMNPQUhMsVetMC9ksuGPkK1BTt", "mrdiv.tumblr.com");


			//var photoPost = ((Api.PhotoPost)tumblr.RequestPosts(Api.Post.Types.Photo).Content[0]);

			//var photo = photoPost.Photos.First();

			//dynamic d = CloneObject(photo);
			//d.LocalPath = "...";
			//d.Image = new System.Drawing.Bitmap("...");

			//using (var writer = new System.IO.StreamWriter(@"C:\serialized.txt"))
			//{
			//    JsonSerializer.Create(new JsonSerializerSettings() { Formatting = Formatting.Indented })
			//        .Serialize(writer, d);
			//}


			//System.Net.HttpWebRequest r = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://www.tumblr.com/photo/1280/nerdasaurusrex/108665072/1/Cxuawx2rGnjjpjv2hXPIw47j");
			//var x = r.GetResponse();
			//var stream = x.GetResponseStream();
			//byte[] buffer = new byte[1024];
			//int read = 0;
			//using (var fs = new System.IO.FileStream(@"C:\image.jpg", System.IO.FileMode.Create))
			//{
			//    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
			//    {
			//        fs.Write(buffer, 0, read);
			//    }
			//}

			BackupEverything("noirlac.tumblr.com", @"D:\noirlac.tumblr.com\");
		}

		/// <summary>
		/// allow to create a modifiable dynamic object containing all the public properties of the given object.
		/// the reference properties are the same.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static dynamic CloneObject(object obj)
		{
			dynamic d = new System.Dynamic.ExpandoObject();
			var typeofObject = obj.GetType();
			var members = typeofObject.GetProperties();
			var assignableD = (d as IDictionary<string, object>);
			foreach (var property in members)
			{
				assignableD[property.Name] = property.GetValue(obj, null);
			}
			return d;
		}

		public static string Download(string url, string basePath, System.IO.FileMode openMode = System.IO.FileMode.Create, int timeout = 10000)
		{
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
			request.Timeout = timeout;
			request.ReadWriteTimeout = timeout;
			System.Net.WebResponse response;
			int counter = 0;
			int sleepTime = 1000;
		getResponse:
			counter++;
			try
			{
				response = request.GetResponse();
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					System.Threading.Thread.Sleep(sleepTime);
					sleepTime *= 2;
					request.Abort();
					goto getResponse;//uglyyyyyy
				}
				else
				{
					string responseString;
					using (var sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
					{
						responseString = sr.ReadToEnd();
					}
					//debug
					return string.Format("ERROR: {0}", ((HttpWebResponse)ex.Response).StatusCode.ToString());
					//debug
					throw new Exception(((HttpWebResponse)ex.Response).StatusCode.ToString());
				}
			}
			catch (Exception)
			{
				throw;
			}

			string localPath = System.IO.Path.Combine(basePath, response.ResponseUri.Segments.Last());
			if (System.IO.File.Exists(localPath))
			{
				//todo: recursive auto file name (add 1, 2, n... after file name)
				//useful for video preview, but for a photo we can return the existing file
				response.Close();
				return localPath;
				//throw new Exception(localPath + " already exists!");
			}
			byte[] buffer = new byte[1024];
			int read = 0;
			using (var fs = new System.IO.FileStream(localPath, openMode))
			{
				while ((read = response.GetResponseStream().Read(buffer, 0, buffer.Length)) > 0)
				{
					fs.Write(buffer, 0, read);
				}
			}
			return localPath;
		}

		//todo: incremental backup from existing base
		//todo: download theme and render the blog as original
		public static void BackupEverything(string blogHostName, string folderPath)
		{
			Api.Tumblr tumblr = new Api.Tumblr("PyezS3Q4Smivb24d9SzZGYSuhMNPQUhMsVetMC9ksuGPkK1BTt", blogHostName);

			int offset = 0;
			int total = tumblr.RequestBlogInfos().Content.Posts;//one extra request doesn't hurt...
			List<Api.PostCollection> rawPostCollections = new List<Api.PostCollection>();
			const int PAGE_SIZE = 20;//max
			do
			{
				//backup all the things!
				var response = tumblr.RequestPosts(Api.Post.Types.None, offset, PAGE_SIZE, null, null, Api.Tumblr.Filters.Raw, true, true);
				rawPostCollections.Add(response.Content);
				offset += PAGE_SIZE;
			} while (offset < total);

			var dummy = tumblr.RequestPosts(Api.Post.Types.None, 0, 0);//ask 0 but return 20
			Api.PostCollection all = dummy.Content;
			all.Clear();//this api sucks
			foreach (var item in (from postCollection in rawPostCollections from post in postCollection select post))
			{
				all.Add(item);
			}

			List<dynamic> allDynamic = new List<dynamic>();
			string localPath;
			foreach (var post in all)
			{
				switch (post.Type)
				{
					default:
						throw new NotImplementedException();
					case TumblrLeecher.Api.Post.Types.Text:
					case TumblrLeecher.Api.Post.Types.Quote:
					case TumblrLeecher.Api.Post.Types.Link:
					case TumblrLeecher.Api.Post.Types.Answer:
					case TumblrLeecher.Api.Post.Types.Chat:
					//nothing to download
					case TumblrLeecher.Api.Post.Types.Audio:
						//todo: parse player and download music?
						allDynamic.Add(CloneObject(post));
						break;
					case TumblrLeecher.Api.Post.Types.Video:
						var videoPost = post as Api.VideoPost;
						dynamic videoPostDynamic = CloneObject(videoPost);
						if (videoPost.ThumbnailUrl != null)
						{
							localPath = Download(videoPost.ThumbnailUrl, folderPath);
							videoPostDynamic.ThumbnailLocalPath = localPath;
						}
						allDynamic.Add(videoPostDynamic);
						//todo: download actual video?
						break;
					case TumblrLeecher.Api.Post.Types.Photo:
						var photoPost = post as Api.PhotoPost;
						dynamic photoPostDynamic = CloneObject(photoPost);
						photoPostDynamic.Photos = new List<dynamic>();
						foreach (var photo in photoPost.Photos)
						{
							dynamic photoDynamic = CloneObject(photo);
							photoDynamic.OriginalSize = CloneObject(photo.OriginalSize);
							localPath = null;
							try
							{
								localPath = Download(photo.OriginalSize.Url, folderPath);
							}
							catch (Exception ex)
							{
								//403 happen sometimes...
								photoDynamic.OriginalSize.DownloadException = ex;
								photo.AltSizes.Sort(new PhotoSizeComparer());
								foreach (var item in photo.AltSizes)
								{
									try
									{
										//try to download the next size
										localPath = Download(item.Url, folderPath);
										break;
									}
									catch (Exception)
									{
										continue;
									}
								}

							}
							photoDynamic.OriginalSize.LocalPath = localPath;
							//todo: remove useless entries (keep larger size only)
							//photoDynamic.AltSizes = new List<dynamic>();
							//foreach (var item in photo.AltSizes)
							//{
							//    if (item.Width == photo.OriginalSize.Width && item.Height == photo.OriginalSize.Height && item.Url == photo.OriginalSize.Url)
							//    {
							//        //original
							//        continue;
							//    }
							//    dynamic altSizeDynamic = CloneObject(item);
							//    localPath = Download(item.Url, folderPath);
							//    altSizeDynamic.LocalPath = localPath;
							//    photoDynamic.AltSizes.Add(altSizeDynamic);
							//}
							photoPostDynamic.Photos.Add(photoDynamic);
						}
						allDynamic.Add(photoPostDynamic);
						break;
				}
			}

			using (var tw = new System.IO.StreamWriter(System.IO.Path.Combine(folderPath, "serialized.txt")))
			{
				var anon = new { Posts = allDynamic, Blog = all.Blog, TotalPosts = all.TotalPosts };
				Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented }).Serialize(tw, anon);
			}

		}


		//TODO one day: unit tests
		//Api.Tumblr tumblr = new Api.Tumblr("PyezS3Q4Smivb24d9SzZGYSuhMNPQUhMsVetMC9ksuGPkK1BTt", "nerdasaurusrex.tumblr.com");

		//            //tumblr.RequestPosts();

		//            string blogInfo =
		//@"{""meta"":{""status"":200,""msg"":""OK""},""response"":{""blog"":{""title"":""hello"",""posts"":95,""name"":""nerdasaurusrex"",""url"":""http:\/\/nerdasaurusrex.tumblr.com\/"",""updated"":1242492193,""description"":""<script type=\""text\/javascript\"">\r\nvar gaJsHost = ((\""https:\"" == document.location.protocol) ? \""https:\/\/ssl.\"" : \""http:\/\/www.\"");\r\ndocument.write(unescape(\""%3Cscript src='\"" + gaJsHost + \""google-analytics.com\/ga.js' type='text\/javascript'%3E%3C\/script%3E\""));\r\n<\/script>\r\n<script type=\""text\/javascript\"">\r\nvar pageTracker = _gat._getTracker(\""UA-6194486-1\"");\r\npageTracker._trackPageview();\r\n<\/script>"",""ask"":false}}}";

		//            Api.Response<Api.BlogInfo> infos = new Api.Response<Api.BlogInfo>(blogInfo);

		//            string postsData =
		//@"{""meta"":{""status"":200,""msg"":""OK""},""response"":{""blog"":{""title"":""hello"",""posts"":95,""name"":""nerdasaurusrex"",""url"":""http:\/\/nerdasaurusrex.tumblr.com\/"",""updated"":1242492193,""description"":""<script type=\""text\/javascript\"">\r\nvar gaJsHost = ((\""https:\"" == document.location.protocol) ? \""https:\/\/ssl.\"" : \""http:\/\/www.\"");\r\ndocument.write(unescape(\""%3Cscript src='\"" + gaJsHost + \""google-analytics.com\/ga.js' type='text\/javascript'%3E%3C\/script%3E\""));\r\n<\/script>\r\n<script type=\""text\/javascript\"">\r\nvar pageTracker = _gat._getTracker(\""UA-6194486-1\"");\r\npageTracker._trackPageview();\r\n<\/script>"",""ask"":false},""posts"":[{""blog_name"":""nerdasaurusrex"",""id"":108665072,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/108665072\/unicornology-merricat-via-milkdrop"",""slug"":""unicornology-merricat-via-milkdrop"",""type"":""photo"",""date"":""2009-05-16 16:43:19 GMT"",""timestamp"":1242492199,""format"":""html"",""reblog_key"":""9iA9Hg3D"",""tags"":[],""highlighted"":[],""note_count"":72,""source_url"":""http:\/\/ptasiemleczko.tumblr.com\/post\/108530927"",""source_title"":""ptasiemleczko"",""caption"":""<p><a href=\""http:\/\/unicornology.tumblr.com\/post\/108632228\/merricat-via-milkdrop\"">unicornology<\/a>:<\/p>\n<blockquote><a href=\""http:\/\/merricat.tumblr.com\/post\/108531127\/via-milkdrop\"">merricat<\/a>:(via <a href=\""http:\/\/milkdrop.tumblr.com\/\"">milkdrop<\/a>)<\/blockquote>"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":721,""height"":980,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/108665072\/1\/Cxuawx2rGnjjpjv2hXPIw47j""},{""width"":500,""height"":680,""url"":""http:\/\/25.media.tumblr.com\/Cxuawx2rGnjjpjv2hXPIw47jo1_500.jpg""},{""width"":400,""height"":544,""url"":""http:\/\/25.media.tumblr.com\/Cxuawx2rGnjjpjv2hXPIw47jo1_400.jpg""},{""width"":250,""height"":340,""url"":""http:\/\/24.media.tumblr.com\/Cxuawx2rGnjjpjv2hXPIw47jo1_250.jpg""},{""width"":100,""height"":136,""url"":""http:\/\/25.media.tumblr.com\/Cxuawx2rGnjjpjv2hXPIw47jo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/Cxuawx2rGnjjpjv2hXPIw47jo1_75sq.jpg""}],""original_size"":{""width"":721,""height"":980,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/108665072\/1\/Cxuawx2rGnjjpjv2hXPIw47j""}}]},{""blog_name"":""nerdasaurusrex"",""id"":71907476,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/71907476\/thedailywhat-clip-save-of-the-day-the"",""slug"":""thedailywhat-clip-save-of-the-day-the"",""type"":""photo"",""date"":""2009-01-20 22:10:55 GMT"",""timestamp"":1232489455,""format"":""html"",""reblog_key"":""92cym2aM"",""tags"":[],""highlighted"":[],""note_count"":175,""caption"":""<p><a href=\""http:\/\/thedw.us\/post\/71899585\/clip-save-of-the-day-the-problem-solving\"">thedailywhat<\/a>:<\/p>\n<blockquote>\n<p><b>Clip + Save of the Day:<\/b> The Problem Solving Flowchart.<\/p>\n<p>[<a href=\""http:\/\/digg.com\/comedy\/A_flow_chart_for_every_problematic_situation\"">via<\/a>.]<\/p>\n<\/blockquote>"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":450,""height"":496,""url"":""http:\/\/24.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_500.jpg""},{""width"":400,""height"":441,""url"":""http:\/\/24.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_400.jpg""},{""width"":250,""height"":276,""url"":""http:\/\/25.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_250.jpg""},{""width"":100,""height"":110,""url"":""http:\/\/24.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_75sq.jpg""}],""original_size"":{""width"":450,""height"":496,""url"":""http:\/\/24.media.tumblr.com\/b9vfl4b63iympe5dRV6q5qvdo1_500.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":70780263,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70780263"",""slug"":"""",""type"":""photo"",""date"":""2009-01-15 23:56:45 GMT"",""timestamp"":1232063805,""format"":""html"",""reblog_key"":""YS36RwcH"",""tags"":[],""highlighted"":[],""note_count"":33,""caption"":"""",""link_url"":""http:\/\/www.flickr.com\/photos\/zarajay\/3199461121\/"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":1391,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/70780263\/1\/6v2mR1qOiirml57g9iRgbSve""},{""width"":500,""height"":543,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiirml57g9iRgbSveo1_500.jpg""},{""width"":400,""height"":435,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOiirml57g9iRgbSveo1_400.jpg""},{""width"":250,""height"":272,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOiirml57g9iRgbSveo1_250.jpg""},{""width"":100,""height"":109,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOiirml57g9iRgbSveo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiirml57g9iRgbSveo1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":1391,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/70780263\/1\/6v2mR1qOiirml57g9iRgbSve""}}]},{""blog_name"":""nerdasaurusrex"",""id"":70779613,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70779613\/cold-should-be-warm"",""slug"":""cold-should-be-warm"",""type"":""text"",""date"":""2009-01-15 23:52:56 GMT"",""timestamp"":1232063576,""format"":""html"",""reblog_key"":""CndyMYaT"",""tags"":[],""highlighted"":[],""note_count"":0,""title"":""Cold should.. be.. warm"",""body"":""<p>I woke up this morning and it was -32C. Without windchill. WITHOUT wind chill. Didn&#8217;t check that.. but it was definitely atleast -40C.<\/p>\n<p>The greatest thing was I had to sit outside waiting in that weather for half an hour.<\/p>\n<p>My toes still tingle.<\/p>""},{""blog_name"":""nerdasaurusrex"",""id"":70577199,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70577199\/teacakes-drawingbycandlelight-is-this-the"",""slug"":""teacakes-drawingbycandlelight-is-this-the"",""type"":""photo"",""date"":""2009-01-15 03:06:57 GMT"",""timestamp"":1231988817,""format"":""html"",""reblog_key"":""aTKd5xSJ"",""tags"":[],""highlighted"":[],""note_count"":75,""source_url"":""http:\/\/weholdthesky.tumblr.com\/post\/70562552\/i-would-like-a-wall-like-this"",""source_title"":""weholdthesky"",""caption"":""<p><a href=\""http:\/\/teacakes.tumblr.com\/post\/70566475\/drawingbycandlelight-is-this-the-polaroid-a-day\"">teacakes<\/a>:<\/p>\n<blockquote><a href=\""http:\/\/drawingbycandlelight.tumblr.com\/post\/70565549\/speedofair\"">drawingbycandlelight<\/a><br\/>is this the polaroid a day lady?<\/blockquote>"",""link_url"":""http:\/\/weheartit.com\/entry\/287885"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":500,""height"":375,""url"":""http:\/\/24.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_500.jpg""},{""width"":400,""height"":300,""url"":""http:\/\/25.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_400.jpg""},{""width"":250,""height"":188,""url"":""http:\/\/24.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_250.jpg""},{""width"":100,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_75sq.jpg""}],""original_size"":{""width"":500,""height"":375,""url"":""http:\/\/24.media.tumblr.com\/UA8PmDbQJiqb4l3iPvDCk7Ppo1_500.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":70574467,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70574467\/teacakes-theslyestfox"",""slug"":""teacakes-theslyestfox"",""type"":""photo"",""date"":""2009-01-15 02:53:54 GMT"",""timestamp"":1231988034,""format"":""html"",""reblog_key"":""a7IglGk9"",""tags"":[],""highlighted"":[],""note_count"":23,""source_url"":""http:\/\/shirleyclifford.tumblr.com\/post\/69922548"",""source_title"":""shirleyclifford"",""caption"":""<p><a href=\""http:\/\/teacakes.tumblr.com\/post\/70569444\/theslyestfox\"">teacakes<\/a>:<\/p>\n<blockquote><a href=\""http:\/\/theslyestfox.tumblr.com\/\"">theslyestfox<\/a><\/blockquote>"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":500,""height"":375,""url"":""http:\/\/25.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_500.jpg""},{""width"":400,""height"":300,""url"":""http:\/\/25.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_400.jpg""},{""width"":250,""height"":188,""url"":""http:\/\/24.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_250.jpg""},{""width"":100,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_75sq.jpg""}],""original_size"":{""width"":500,""height"":375,""url"":""http:\/\/25.media.tumblr.com\/mXP8tdVd3imeim7sByY5dL8So1_500.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":70574193,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70574193\/people-who-talk-about-their-dreams-are-actually"",""slug"":""people-who-talk-about-their-dreams-are-actually"",""type"":""quote"",""date"":""2009-01-15 02:52:29 GMT"",""timestamp"":1231987949,""format"":""html"",""reblog_key"":""VGBvnTXW"",""tags"":[],""highlighted"":[],""note_count"":279,""source_url"":""http:\/\/annealed.tumblr.com\/post\/70569132\/people-who-talk-about-their-dreams-are-actually"",""source_title"":""annealed"",""text"":""People who talk about their dreams are actually trying to tell you things about themselves they\u2019d never admit in normal conversation."",""source"":""Chuck Klosterman (via <a href=\""http:\/\/ignoranceisabliss.com\/\"">sooshi<\/a>) (via <a href=\""http:\/\/thebackdoor.tumblr.com\/\"">thebackdoor<\/a>)""},{""blog_name"":""nerdasaurusrex"",""id"":70573474,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/70573474\/teacakes-theprosaic-she-has-a-freaky-ass-face"",""slug"":""teacakes-theprosaic-she-has-a-freaky-ass-face"",""type"":""photo"",""date"":""2009-01-15 02:48:44 GMT"",""timestamp"":1231987724,""format"":""html"",""reblog_key"":""VA3sNWOE"",""tags"":[],""highlighted"":[],""note_count"":21,""source_url"":""http:\/\/zackwolk.tumblr.com\/post\/70536949\/the-american-american-beauty"",""source_title"":""zackwolk"",""caption"":""<p><a href=\""http:\/\/teacakes.tumblr.com\/post\/70572959\/theprosaic-she-has-a-freaky-ass-face-wtf-this-is\"">teacakes<\/a>:<\/p>\n<blockquote><a href=\""http:\/\/theprosaic.tumblr.com\/post\/70572773\"">theprosaic<\/a><br\/>she has a freaky ass face. wtf this is sad.<\/blockquote>"",""link_url"":""http:\/\/www.raph.com\/3dartists\/artgallery\/6605.jpg"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1200,""height"":1200,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/70573474\/1\/P8IXBolt7iq5p4m2kpEh7jGh""},{""width"":500,""height"":500,""url"":""http:\/\/24.media.tumblr.com\/P8IXBolt7iq5p4m2kpEh7jGho1_500.jpg""},{""width"":400,""height"":400,""url"":""http:\/\/24.media.tumblr.com\/P8IXBolt7iq5p4m2kpEh7jGho1_400.jpg""},{""width"":250,""height"":250,""url"":""http:\/\/24.media.tumblr.com\/P8IXBolt7iq5p4m2kpEh7jGho1_250.jpg""},{""width"":100,""height"":100,""url"":""http:\/\/25.media.tumblr.com\/P8IXBolt7iq5p4m2kpEh7jGho1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/P8IXBolt7iq5p4m2kpEh7jGho1_75sq.jpg""}],""original_size"":{""width"":1200,""height"":1200,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/70573474\/1\/P8IXBolt7iq5p4m2kpEh7jGh""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68347589,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68347589\/teacakes-theenormousemptiness"",""slug"":""teacakes-theenormousemptiness"",""type"":""photo"",""date"":""2009-01-04 21:18:45 GMT"",""timestamp"":1231103925,""format"":""html"",""reblog_key"":""dSkfl3Ws"",""tags"":[],""highlighted"":[],""note_count"":175,""source_url"":""http:\/\/pathofcinders.tumblr.com\/post\/68066729\/via-nathanhunter-files-wordpress-com"",""source_title"":""pathofcinders"",""caption"":""<p><a href=\""http:\/\/teacakes.tumblr.com\/post\/68338409\/theenormousemptiness\"">teacakes<\/a>:<\/p>\n<blockquote><a href=\""http:\/\/theenormousemptiness.tumblr.com\/post\/68215299\"">theenormousemptiness<\/a><\/blockquote>"",""link_url"":""http:\/\/nathanhunter.files.wordpress.com\/2008\/06\/1213491202049.gif"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":500,""height"":225,""url"":""http:\/\/25.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_500.gif""},{""width"":400,""height"":180,""url"":""http:\/\/24.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_400.gif""},{""width"":250,""height"":113,""url"":""http:\/\/25.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_250.gif""},{""width"":100,""height"":45,""url"":""http:\/\/24.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_100.gif""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_75sq.gif""}],""original_size"":{""width"":500,""height"":225,""url"":""http:\/\/25.media.tumblr.com\/ijdilvQZri9a5070CYVG6LHLo1_500.gif""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68152693,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68152693"",""slug"":"""",""type"":""photo"",""date"":""2009-01-03 17:23:33 GMT"",""timestamp"":1231003413,""format"":""html"",""reblog_key"":""7oQYFrKH"",""tags"":[],""highlighted"":[],""note_count"":21,""caption"":"""",""link_url"":""httphttp:\/\/www.flickr.com\/photos\/zarajay\/3159761153\/"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68152693\/1\/6v2mR1qOiia398z47m29rp1m""},{""width"":500,""height"":333,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiia398z47m29rp1mo1_500.jpg""},{""width"":400,""height"":267,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOiia398z47m29rp1mo1_400.jpg""},{""width"":250,""height"":167,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiia398z47m29rp1mo1_250.jpg""},{""width"":100,""height"":67,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiia398z47m29rp1mo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOiia398z47m29rp1mo1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68152693\/1\/6v2mR1qOiia398z47m29rp1m""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68082018,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68082018\/i-want-to-do-something"",""slug"":""i-want-to-do-something"",""type"":""text"",""date"":""2009-01-03 05:58:22 GMT"",""timestamp"":1230962302,""format"":""html"",""reblog_key"":""QEaxzqI3"",""tags"":[],""highlighted"":[],""note_count"":42,""title"":""I want to do something."",""body"":""<p><a href=\""http:\/\/everythingapplies.tumblr.com\/post\/68074832\/i-want-to-do-something\"">everythingapplies<\/a>:<\/p>\n<blockquote>I want to grab a bunch of free spirited people and dance in a parking lot.<br\/>Go out to the middle of nowhere and start a bonfire and just let go, dammit. We could share stories and talents and words of wisdom. We can sing and jump and scream and yell. Fuck, nothing happens here. Everyone\u2019s too uptight and going through every day with the same boring useless rigid schedule. <br\/>Good morning, let\u2019s go to work\/school, let\u2019s come home and watch tv, goodnight, good morning, watching our lives waste away, one day at a time.<br\/>I\u2019ll bring the guitar, you bring the jello, you bring the boom box, you bring some firewood, and you, way in the back, bring your dancing shoes.<\/blockquote>""},{""blog_name"":""nerdasaurusrex"",""id"":68081524,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68081524"",""slug"":"""",""type"":""photo"",""date"":""2009-01-03 05:54:41 GMT"",""timestamp"":1230962081,""format"":""html"",""reblog_key"":""kD9Cm2eO"",""tags"":[],""highlighted"":[],""note_count"":16,""caption"":"""",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":924,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68081524\/1\/6v2mR1qOii9encnsTgYUNBYL""},{""width"":500,""height"":361,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9encnsTgYUNBYLo1_500.jpg""},{""width"":400,""height"":289,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9encnsTgYUNBYLo1_400.jpg""},{""width"":250,""height"":180,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9encnsTgYUNBYLo1_250.jpg""},{""width"":100,""height"":72,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9encnsTgYUNBYLo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9encnsTgYUNBYLo1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":924,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68081524\/1\/6v2mR1qOii9encnsTgYUNBYL""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68081085,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68081085"",""slug"":"""",""type"":""photo"",""date"":""2009-01-03 05:50:49 GMT"",""timestamp"":1230961849,""format"":""html"",""reblog_key"":""tYYTGgvc"",""tags"":[],""highlighted"":[],""note_count"":7,""caption"":"""",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68081085\/1\/6v2mR1qOii9eie30WkKFNsVt""},{""width"":500,""height"":333,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9eie30WkKFNsVto1_500.jpg""},{""width"":400,""height"":267,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9eie30WkKFNsVto1_400.jpg""},{""width"":250,""height"":167,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9eie30WkKFNsVto1_250.jpg""},{""width"":100,""height"":67,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9eie30WkKFNsVto1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9eie30WkKFNsVto1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68081085\/1\/6v2mR1qOii9eie30WkKFNsVt""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68080868,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68080868"",""slug"":"""",""type"":""photo"",""date"":""2009-01-03 05:49:16 GMT"",""timestamp"":1230961756,""format"":""html"",""reblog_key"":""NSQuOP2I"",""tags"":[],""highlighted"":[],""note_count"":8,""caption"":"""",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":1059,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68080868\/1\/6v2mR1qOii9egebcWZGDJnCo""},{""width"":500,""height"":414,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9egebcWZGDJnCoo1_500.jpg""},{""width"":400,""height"":331,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9egebcWZGDJnCoo1_400.jpg""},{""width"":250,""height"":207,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9egebcWZGDJnCoo1_250.jpg""},{""width"":100,""height"":83,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9egebcWZGDJnCoo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9egebcWZGDJnCoo1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":1059,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68080868\/1\/6v2mR1qOii9egebcWZGDJnCo""}}]},{""blog_name"":""nerdasaurusrex"",""id"":68080401,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/68080401"",""slug"":"""",""type"":""photo"",""date"":""2009-01-03 05:45:22 GMT"",""timestamp"":1230961522,""format"":""html"",""reblog_key"":""l7FHQw9o"",""tags"":[],""highlighted"":[],""note_count"":13,""caption"":"""",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68080401\/1\/6v2mR1qOii9ebdeze2yTII5a""},{""width"":500,""height"":333,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9ebdeze2yTII5ao1_500.jpg""},{""width"":400,""height"":267,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9ebdeze2yTII5ao1_400.jpg""},{""width"":250,""height"":167,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOii9ebdeze2yTII5ao1_250.jpg""},{""width"":100,""height"":67,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9ebdeze2yTII5ao1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOii9ebdeze2yTII5ao1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":853,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/68080401\/1\/6v2mR1qOii9ebdeze2yTII5a""}}]},{""blog_name"":""nerdasaurusrex"",""id"":67240263,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/67240263\/via-stewardesses"",""slug"":""via-stewardesses"",""type"":""photo"",""date"":""2008-12-29 03:46:14 GMT"",""timestamp"":1230522374,""format"":""html"",""reblog_key"":""gddEEZ0H"",""tags"":[],""highlighted"":[],""note_count"":47,""caption"":""<p>(via <a href=\""http:\/\/stewardesses.tumblr.com\/\"">stewardesses<\/a>)<\/p>"",""link_url"":""http:\/\/flickr.com\/photos\/momoblack\/3140137936\/"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":500,""height"":405,""url"":""http:\/\/25.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_500.jpg""},{""width"":400,""height"":324,""url"":""http:\/\/24.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_400.jpg""},{""width"":250,""height"":203,""url"":""http:\/\/25.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_250.jpg""},{""width"":100,""height"":81,""url"":""http:\/\/25.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/25.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_75sq.jpg""}],""original_size"":{""width"":500,""height"":405,""url"":""http:\/\/25.media.tumblr.com\/Shyp1SHY0i220n94d5zaJIggo1_500.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":65673653,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/65673653\/thebackdoor-via-theenormousemptiness-excuse"",""slug"":""thebackdoor-via-theenormousemptiness-excuse"",""type"":""photo"",""date"":""2008-12-19 04:06:43 GMT"",""timestamp"":1229659603,""format"":""html"",""reblog_key"":""dlkx1Axv"",""tags"":[],""highlighted"":[],""caption"":""<p><a href=\""http:\/\/thebackdoor.tumblr.com\/post\/65489664\/via-theenormousemptiness-excuse-me-i-happen-to\"">thebackdoor<\/a>:<\/p>\n<blockquote>\n<p>(via <a href=\""http:\/\/theenormousemptiness.tumblr.com\/\"">theenormousemptiness<\/a>)<\/p>\n<p>excuse me i happen to think cosmopolitan is very informative!<\/p>\n<p>i think the last one i looked at had the subtitle of:<\/p>\n<p>\u201d WHERE\u2019S HIS G-SPOT?\u201d<\/p>\n<p>i mean come on! that is imperative to know for human existance\u2026.<\/p>\n<\/blockquote>"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":387,""height"":500,""url"":""http:\/\/24.media.tumblr.com\/YMq5Yf4Nrhmg1m4u7ejhj9Mzo1_400.jpg""},{""width"":250,""height"":323,""url"":""http:\/\/25.media.tumblr.com\/YMq5Yf4Nrhmg1m4u7ejhj9Mzo1_250.jpg""},{""width"":100,""height"":129,""url"":""http:\/\/25.media.tumblr.com\/YMq5Yf4Nrhmg1m4u7ejhj9Mzo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/YMq5Yf4Nrhmg1m4u7ejhj9Mzo1_75sq.jpg""}],""original_size"":{""width"":387,""height"":500,""url"":""http:\/\/24.media.tumblr.com\/YMq5Yf4Nrhmg1m4u7ejhj9Mzo1_400.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":65633202,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/65633202\/hamandheroin-the-website-is-down-by-josh"",""slug"":""hamandheroin-the-website-is-down-by-josh"",""type"":""video"",""date"":""2008-12-18 23:16:24 GMT"",""timestamp"":1229642184,""format"":""html"",""reblog_key"":""zw1PcH0O"",""tags"":[],""highlighted"":[],""note_count"":1,""caption"":""<p><a href=\""http:\/\/www.hamandheroin.com\/post\/65610862\/the-website-is-down-by-josh-weinberg-if-youre\"">hamandheroin<\/a>:<\/p>\n<blockquote>\n<p><b>The Website Is Down<\/b> by\u00a0Josh Weinberg<\/p>\n<p>If you\u2019re into computer humor (and really, who isn\u2019t?), it really pays off to watch this all the way through.\u00a0 Even if it\u2019s just for the line, <i>\u201cYou can\u2019t \u2018Arrange by Penis!\u201d<\/i><\/p>\n<\/blockquote>"",""permalink_url"":""http:\/\/www.youtube.com\/watch?v=BcQ7RkyBoBc"",""thumbnail_url"":""http:\/\/img.youtube.com\/vi\/BcQ7RkyBoBc\/hqdefault.jpg"",""thumbnail_width"":480,""thumbnail_height"":360,""html5_capable"":true,""player"":[{""width"":250,""embed_code"":""<iframe width=\""248\"" height=\""186\"" src=\""http:\/\/www.youtube.com\/embed\/BcQ7RkyBoBc?wmode=transparent&autohide=1&egm=0&hd=1&iv_load_policy=3&modestbranding=1&rel=0&showinfo=0&showsearch=0\"" frameborder=\""0\"" allowfullscreen><\/iframe>""},{""width"":400,""embed_code"":""<iframe width=\""400\"" height=\""300\"" src=\""http:\/\/www.youtube.com\/embed\/BcQ7RkyBoBc?wmode=transparent&autohide=1&egm=0&hd=1&iv_load_policy=3&modestbranding=1&rel=0&showinfo=0&showsearch=0\"" frameborder=\""0\"" allowfullscreen><\/iframe>""},{""width"":500,""embed_code"":""<iframe width=\""500\"" height=\""375\"" src=\""http:\/\/www.youtube.com\/embed\/BcQ7RkyBoBc?wmode=transparent&autohide=1&egm=0&hd=1&iv_load_policy=3&modestbranding=1&rel=0&showinfo=0&showsearch=0\"" frameborder=\""0\"" allowfullscreen><\/iframe>""}]},{""blog_name"":""nerdasaurusrex"",""id"":65465900,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/65465900\/via-rainier"",""slug"":""via-rainier"",""type"":""photo"",""date"":""2008-12-18 02:11:23 GMT"",""timestamp"":1229566283,""format"":""html"",""reblog_key"":""QTr56GE1"",""tags"":[],""highlighted"":[],""note_count"":3,""caption"":""<p>(via <a href=\""http:\/\/rainier.tumblr.com\/\"">rainier<\/a>)<\/p>"",""link_url"":""http:\/\/www.flickr.com\/photos\/jamieswang\/3096550978\/"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":500,""height"":375,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_500.jpg""},{""width"":400,""height"":300,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_400.jpg""},{""width"":250,""height"":188,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_250.jpg""},{""width"":100,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_75sq.jpg""}],""original_size"":{""width"":500,""height"":375,""url"":""http:\/\/24.media.tumblr.com\/hgxzejjachmb1rj2l92fJJpZo1_500.jpg""}}]},{""blog_name"":""nerdasaurusrex"",""id"":65039571,""post_url"":""http:\/\/nerdasaurusrex.tumblr.com\/post\/65039571\/i-remember"",""slug"":""i-remember"",""type"":""photo"",""date"":""2008-12-15 22:31:13 GMT"",""timestamp"":1229380273,""format"":""html"",""reblog_key"":""00POeVP4"",""tags"":[],""highlighted"":[],""note_count"":2,""caption"":""<p>i remember<\/p>"",""link_url"":""http:\/\/www.flickr.com\/photos\/zarajay\/3111798970\/"",""photos"":[{""caption"":"""",""alt_sizes"":[{""width"":1280,""height"":841,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/65039571\/1\/6v2mR1qOihj8vq3bua8g4Q9i""},{""width"":500,""height"":329,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOihj8vq3bua8g4Q9io1_500.jpg""},{""width"":400,""height"":263,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOihj8vq3bua8g4Q9io1_400.jpg""},{""width"":250,""height"":164,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOihj8vq3bua8g4Q9io1_250.jpg""},{""width"":100,""height"":66,""url"":""http:\/\/25.media.tumblr.com\/6v2mR1qOihj8vq3bua8g4Q9io1_100.jpg""},{""width"":75,""height"":75,""url"":""http:\/\/24.media.tumblr.com\/6v2mR1qOihj8vq3bua8g4Q9io1_75sq.jpg""}],""original_size"":{""width"":1280,""height"":841,""url"":""http:\/\/www.tumblr.com\/photo\/1280\/nerdasaurusrex\/65039571\/1\/6v2mR1qOihj8vq3bua8g4Q9i""}}]}],""total_posts"":95}}";

		//            Api.Response<Api.PostCollection> posts = new Api.Response<Api.PostCollection>(postsData);

		//var x = tumblr.RequestPosts();
		//var y = tumblr.RequestBlogInfos();
		//var z = tumblr.RequestAvatar();

		//var x = (from value in Enum.GetValues(typeof(Api.Post.Types)).OfType<Api.Post.Types>() select tumblr.RequestPosts(value)).ToArray();
	}

	class PhotoSizeComparer : Comparer<Api.PhotoSize>
	{

		public override int Compare(Api.PhotoSize x, Api.PhotoSize y)
		{
			long sizeX = x.Width * x.Height;
			long sizeY = y.Width * y.Height;
			if (sizeX < sizeY)
			{
				return 1;
			}
			else if (sizeX > sizeY)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}
}

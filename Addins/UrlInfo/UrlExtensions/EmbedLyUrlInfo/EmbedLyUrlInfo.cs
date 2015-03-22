using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.Collections.Specialized;
using MoistureBot.ExtensionPoints;
using MoistureBot;
using Json;
using System.Web;

namespace MoistureBot
{

	public class EmbedLyUrlInfo: IReceiveUrl
	{

		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		protected String apiKey = null;

		#region IReceiveUrl implementation

		public string ReplyToUrl(Uri uri)
		{

			var apiUrl = CreateEmbedlyUri(uri);

			using(WebClient client = new WebClient())
			{
				var jsonStr = client.DownloadString(apiUrl);

				UrlResponse response = JsonParser.Deserialize<UrlResponse>(jsonStr);

				// reply only to video urls
				if (response.Type.Equals("video"))
					return response.Title;
				else
					return null;
			}
				
		}

		#endregion

		private string CreateEmbedlyUri(Uri uri)
		{
			var url = "http://api.embed.ly/1/oembed?url=" + HttpUtility.UrlEncode(uri.ToString());

			var apiKey = Config.GetSetting("embed.ly","api_key");

			if (String.IsNullOrEmpty(apiKey))
			{
				// creates setting to ini file
				Config.SetSetting("embed.ly","api_key",""); 
			}
			else
			{
				// add key to query
				url += "&key=" + apiKey; 
			}

			return url;
		}

	}

	public class UrlResponse
	{
		public string Title { get; set; }
		public string Type { get; set; }
	}
}

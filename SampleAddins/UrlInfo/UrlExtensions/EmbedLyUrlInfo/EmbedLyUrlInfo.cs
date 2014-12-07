using System;
using System.Text.RegularExpressions;
using System.Linq;
using Mono.Addins;
using System.Net;
using System.Collections.Specialized;
using MoistureBot.ExtensionPoints;
using MoistureBot;
using Json;
using System.Web;

[assembly:Addin("EmbedlyUrlInfo", "1.0")]
[assembly:AddinDependency("UrlInfo", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Embedly url info reply.")]
[assembly:AddinName("EmbedlyUrlinfo")]
[assembly:AddinUrl("")]
namespace UrlInfo
{

	[Extension(typeof(IReceiveUrl))]
	public class EmbedlyUrlInfo: IReceiveUrl
	{

		#region IReceiveUrl implementation

		public string ReplyToUrl(Uri uri)
		{
				
			// TODO: filter video sites only

			var apiUrl = "http://api.embed.ly/1/oembed?url=" + HttpUtility.UrlEncode(uri.ToString());

			using(WebClient client = new WebClient())
			{
				var jsonStr = client.DownloadString(apiUrl);

				UrlResponse response = JsonParser.Deserialize<UrlResponse>(jsonStr);

				if (response.Type.Equals("video"))
					return response.Title;

			}

		}

		#endregion

	}

	public class UrlResponse
	{
		public string Title { get; set; }
		public string Type { get; set; }
	}
}

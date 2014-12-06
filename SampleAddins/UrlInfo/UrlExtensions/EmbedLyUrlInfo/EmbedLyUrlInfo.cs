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
				
			var apiUrl = "http://api.embed.ly/1/oembed?url=" + HttpUtility.UrlEncode(uri.ToString());

			using(WebClient client = new WebClient())
			{
				var response = client.DownloadString(apiUrl);
				return JsonParser.Deserialize<UrlResponse>(response).Title;
			}

		}

		#endregion

	}

	public class UrlResponse
	{
		public string Title { get; set; }
	}
}

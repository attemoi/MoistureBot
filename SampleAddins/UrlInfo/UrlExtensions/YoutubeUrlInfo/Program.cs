using System;
using System.Text.RegularExpressions;
using System.Linq;
using Mono.Addins;
using System.Web;
using System.Collections.Specialized;

[assembly:Addin("YoutubeUrlinfo", "1.0")]
[assembly:AddinDependency("UrlInfo", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Youtube url info reply.")]
[assembly:AddinName("YoutubeUrlinfo")]
[assembly:AddinUrl("")]
namespace UrlInfo
{

	[Extension(typeof(IReceiveUrl))]
	public class Moikkaaja: IReceiveUrl
	{
		#region IReceiveUrl implementation

		public string replyToUrl(Uri uri)
		{

			if (!uri.Host.ToLower().Equals("www.youtube.com")) 
				return null;

			String videoId = HttpUtility.ParseQueryString(uri.Query).Get("v");
			if (!String.IsNullOrEmpty(videoId)) 
			{
				String infoString = new System.Net.WebClient().DownloadString("http://youtube.com/get_video_info?video_id=" + videoId);
				int startIndex = infoString.IndexOf("&title=") + 7;
				int endIndex = infoString.IndexOf("&",startIndex + 1);
				String title = infoString.Substring(startIndex, endIndex-startIndex);
				return '"' + HttpUtility.UrlDecode(title) + '"';
			}

			return null;

		}

		#endregion

	}
}

using System;
using System.Text.RegularExpressions;
using System.Linq;
using Mono.Addins;
using System.Web;
using System.Collections.Specialized;
using Embedly;
using Embedly.OEmbed;
using MoistureBot.ExtensionPoints;
using MoistureBot;

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
	
		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		protected String apiKey = null;

		#region IReceiveUrl implementation

		public string replyToUrl(Uri uri)
		{
			// Get api key from config
			if (String.IsNullOrEmpty(apiKey))
				apiKey = Config.GetSetting("embed.ly","api_key");

			// Create setting to ini file
			if (String.IsNullOrEmpty(apiKey))
				Config.SetSetting("embed.ly","api_key","");
				
			var client = new Client(apiKey);

			if (client.IsUrlSupported(uri))
			{

				var provider = client.GetProvider(uri);
				if (provider.Type == ProviderType.Video)
				{
					var result = client.GetOEmbed(uri);
					var link = result.Response.AsLink;
					return link.Title;
				}
			}

			return null;

		}

		#endregion

	}
}

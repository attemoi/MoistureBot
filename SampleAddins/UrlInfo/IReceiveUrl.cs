using System;
using Mono.Addins;

namespace UrlInfo
{

	[TypeExtensionPoint]
	public interface IReceiveUrl
	{
		/// <summary>
		/// Called when the bot receives an url in any chat.
		/// </summary>
		/// <returns>Bot reply to the url message. Null for no reply.</returns>
		/// <param name="receivedUrl">Received URL.</param>
		String replyToUrl(Uri receivedUrl);
	}
}


using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using System.Xml;
using System.Collections.Generic;
using System.Threading;

[assembly:Addin("GameInviteReply", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Reply to game invites.")]
[assembly:AddinName("GameInviteReply")]
[assembly:AddinUrl("")]

[assembly:ImportAddinFile ("GameInviteReply.xml")]

namespace MoistureBot
{
	[Extension(typeof(IReceiveGameLobbyInvites))]
	public class GameInviteReply
		: IReceiveGameLobbyInvites
	{

		static Random rnd = new Random();

		IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		ILogger Logger = MoistureBotComponentProvider.GetLogger();

		public void InviteReceived(GameLobbyInvite invite)
		{

			List<List<string>> replies = new List<List<string>>();

			using (XmlTextReader reader = new XmlTextReader("addins/GameInviteReply.xml"))
			{
			
				// Parse the file and display each of the nodes.
				reader.ReadToFollowing("replies");

				while (reader.ReadToFollowing("reply"))
				{
					var messages = new List<string>();

					if (reader.ReadToDescendant("message"))
					{
						do
						{
							messages.Add(reader.ReadElementContentAsString());

						} while (reader.ReadToNextSibling("message"));
					}

					replies.Add(messages);

				}


			}

			// We should now have a list of possible replies

			int r = rnd.Next(replies.Count);

			foreach (string message in replies[r])
			{
				Bot.SendChatMessage(message, invite.InviterId);
				Thread.Sleep(1000);
			}
		}

	}
}
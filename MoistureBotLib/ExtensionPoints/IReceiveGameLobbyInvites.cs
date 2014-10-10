using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{
	[TypeExtensionPoint]
	public interface IReceiveGameLobbyInvites
	{
		/// <summary>
		/// Called when the bot has received an invite to a game lobby.
		/// </summary>
		/// <param name="message">Received invite</param>
		void InviteReceived(GameLobbyInvite invite);
	}
		
}


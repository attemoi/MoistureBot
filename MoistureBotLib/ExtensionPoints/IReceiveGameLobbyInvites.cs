using System;
using Mono.Addins;

namespace MoistureBot
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


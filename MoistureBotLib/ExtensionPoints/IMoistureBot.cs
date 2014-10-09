using System;
using System.Collections.Generic;
using MoistureBot.Config;
using Mono.Addins;

namespace MoistureBot
{

	[TypeExtensionPoint]
	public interface IMoistureBot
	{

		string UserName { get; }

		string PersonaName { get; set; }

		/// <summary>
		/// Determines whether bot is connected to Steam.
		/// </summary>
		/// <returns><c>true</c> if this instance is connected; otherwise, <c>false</c>.</returns>
		Boolean IsConnected();

		/// <summary>
		/// Connects and signs in to Steam.
		/// </summary>
		/// <param name="username">Steam username</param>
		/// <param name="password">Steam password</param>
		void Connect(string username, string password);

		/// <summary>
		/// Disconnects from Steam.
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Joins a chat room.
		/// </summary>
		/// <param name="id">Chat room id (steamID64)</param>
		void JoinChatRoom(ulong id);

		/// <summary>
		/// Sends chat message to friend.
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="userId">User id</param>
		void SendChatMessage(String message, ulong userId);

		/// <summary>
		/// Sends chat message to room.
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="chatRoomId">Chat room id.</param>
		void SendChatRoomMessage(String message, ulong chatRoomId);

		/// <summary>
		/// Gets user name based from id.
		/// </summary>
		/// <returns>User profile name</returns>
		/// <param name="id">User id (steamID64)</param>
		string GetUserName(ulong id);

/// 

		/// <summary>
		/// Disconnect and terminate the bot.
		/// </summary>
		void Terminate();

		/// <summary>
		/// Kicks a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void KickChatMember(ulong roomId, ulong userId);

		/// <summary>
		/// Bans a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void BanChatMember(ulong roomId, ulong userId);

		/// <summary>
		/// Unbans a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void UnbanChatMember(ulong roomId, ulong userId);

		/// <summary>
		/// Chantes the user's online status.
		/// </summary>
		/// <returns>online status</returns>
		OnlineStatus GetOnlineStatus();

		/// <summary>
		/// Changes the user's online status.
		/// </summary>
		void SetOnlineStatus(OnlineStatus state);

		/// <summary>
		/// Sets the online status. Allowed values: "online", "offline", 
		/// "away", "busy", "looking_to_play", "looking_to_trade", "snooze"
		/// </summary>
		/// <param name="state">State.</param>
		void SetOnlineStatus(string status);

	}
}


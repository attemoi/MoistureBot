﻿using System;
using System.Collections.Generic;

namespace MoistureBot
{
	public interface IMoistureBot
	{

		string User { get; }

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

		/// <summary>
		/// Gets the active chat rooms.
		/// </summary>
		/// <returns>The active chat rooms.</returns>
		List<ulong> GetActiveChatRooms();

		/// <summary>
		/// Disconnect and terminate the bot.
		/// </summary>
		void Terminate ();

		/// <summary>
		/// Kicks a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void KickChatMember (ulong roomId, ulong userId);

		/// <summary>
		/// Bans a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void BanChatMember (ulong roomId, ulong userId);

		/// <summary>
		/// Unbans a user from a chat room.
		/// </summary>
		/// <param name="roomId">Room id (steamID64)</param>
		/// <param name="userId">User id (steamID64)</param></param>
		void UnbanChatMember (ulong roomId, ulong userId);

		/// <summary>
		/// Gets the persona state of the user (Online/Offline/Away etc).
		/// </summary>
		/// <returns>The current persona state</returns>
		PersonaState GetPersonaState ();

		/// <summary>
		/// Sets the persona state of the user (Online/Offline/Away etc).
		/// </summary>
		void SetPersonaState(PersonaState state);

	}
}


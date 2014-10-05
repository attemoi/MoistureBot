using System;
using System.Collections.Generic;

namespace moisturebot
{
	public interface IMoistureBot
	{

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
		/// <param name="id">Chat room id (example value: 111222333444555666)</param>
		void JoinChat(ulong id);

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
		/// <param name="id">User id</param>
		string GetUserName(ulong id);

		/// <summary>
		/// Gets the active chat rooms.
		/// </summary>
		/// <returns>The active chat rooms.</returns>
		List<ulong> GetActiveChatRooms();

		/// <summary>
		/// Blocks the current thread until the bot has connected
		/// or failed to connect.
		/// </summary>
		void BlockUntilConnected ();

		/// <summary>
		/// Blocks the current thread until the bot has disconnected
		/// or failed to connect.
		/// </summary>
		void BlockUntilDisconnected();

		/// <summary>
		/// Gets the favorite friends as dictionary containing name and id.
		/// </summary>
		/// <returns>The favorite friends.</returns>
		Dictionary<string, string> GetFavoriteFriends ();

	}
}


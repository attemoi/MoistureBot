using System;
using System.Collections.Generic;
using Mono.Addins;
using MoistureBot.Config;

namespace MoistureBot.ExtensionPoints
{

	/// <summary>
	/// This is an extension point for a single instance addin intended for internal use. 
	/// This should not be extended by external addins.
	/// </summary>
	[TypeExtensionPoint]
	public interface IConfig
	{
		/// <summary>
		/// Creates a new configuration file.
		/// </summary>
		void CreateConfig();

		/// <summary>
		/// Determines wheter the configuration file exists.
		/// </summary>
		/// <returns><c>true</c>, if file exists, <c>false</c> otherwise.</returns>
		bool ConfigExists();

		/// <summary>
		/// Gets the favorite users.
		/// </summary>
		/// <returns>A dictionary with the users' keys and steamID64s</returns>
		Dictionary<string, ulong> GetFavoriteUsers();

		/// <summary>
		/// Gets the id of a favorite user.
		/// </summary>
		/// <returns>The steamID64 of a favourite user if found, null otherwise</returns>
		/// <param name="key">Favorite user's key</param>
		string GetFavoriteUserId(string key);

		/// <summary>
		/// Gets the favorite chat rooms.
		/// </summary>
		/// <returns>>A dictionary with the rooms' keys and steamID64s</returns>
		Dictionary<string, ulong> GetFavoriteChatRooms();

		/// <summary>
		/// Gets the id of a favorite chat room.
		/// </summary>
		/// <returns>The steamID64 of a favourite chat room if found, null otherwise</returns>
		/// <param name="key">Favorite chat room's key</param>
		string GetFavoriteChatRoomId(string key);

		/// <summary>
		/// Adds a new user to favorites.
		/// </summary>
		/// <returns><c>true</c>, if favorite user was added, <c>false</c> otherwise (user already added).</returns>
		/// <param name="key">Favorite key</param>
		/// <param name="userId">Favorite steam64ID</param>
		bool AddFavoriteUser(string key, ulong userId);

		/// <summary>
		/// Adds a new chat room to favorites.
		/// </summary>
		/// <returns><c>true</c>, if favorite chat room was added, <c>false</c> otherwise (room already added).</returns>
		/// <param name="key">Favorite key</param>
		/// <param name="chatRoomId">Chat room steam64ID</param>
		bool AddFavoriteChatRoom(string key, ulong chatRoomId);

		/// <summary>
		/// Removes a user from favorites.
		/// </summary>
		/// <returns><c>true</c>, if favorite user was removed, <c>false</c> otherwise (key not found).</returns>
		/// <param name="key">Favorite user's key</param>
		bool RemoveFavoriteUser(string key);

		/// <summary>
		/// Removes all favorite users.
		/// </summary>
		void RemoveAllFavoriteUsers();

		/// <summary>
		/// Removes a chat room from favorites.
		/// </summary>
		/// <returns><c>true</c>, if favorite chat room was removed, <c>false</c> otherwise (key not found).</returns>
		/// <param name="key">Key.</param>
		bool RemoveFavoriteChatRoom(string key);

		/// <summary>
		/// Removes all favorite chat rooms.
		/// </summary>
		void RemoveAllFavoriteChatRooms();

		/// <summary>
		/// Configures a value in the ini file. Adds a new setting if it doesn't exist.
		/// </summary>
		/// <param name="section">Setting section</param>
		/// <param name="key">Setting key</param>
		/// <param name="value">Setting value</param>
		void SetSetting(string section, string key, string value);

		/// <summary>
		/// Configures a value in the ini file. Adds a new setting if it doesn't exist.
		/// </summary>
		/// <param name="setting">Setting to update</param>
		/// <param name="value">new value</param>
		void SetSetting(ConfigSetting setting, string value);

		/// <summary>
		/// Gets a setting's value from the ini file.
		/// </summary>
		/// <returns>The setting value</returns>
		/// <param name="setting">Setting.</param>
		string GetSetting(ConfigSetting setting);

		/// <summary>
		/// Configures a value in the ini file. Adds a new setting if it doesn't exist.
		/// </summary>
		/// <param name="section">Setting section</param>
		/// <param name="key">Setting key</param>
		/// <param name="value">Setting value</param>
		string GetSetting(string section, string key);

	}
}


using System;
using System.Collections.Generic;

namespace MoistureBot.Config
{
	public interface IConfig
	{
		void CreateConfig();
		bool ConfigExists();

		Dictionary<string, ulong> GetFavoriteUsers();
		string GetFavoriteUserId(string key);

		Dictionary<string, ulong> GetFavoriteChatRooms();
		string GetFavoriteChatRoomId(string key);

		bool AddFavoriteUser (string key, ulong userId);
		bool AddFavoriteChatRoom (string key, ulong chatRoomId);

		bool RemoveFavoriteUser (string key);
		void RemoveAllFavoriteUsers ();

		bool RemoveFavoriteChatRoom (string key);
		void RemoveAllFavoriteChatRooms ();

		void SetSetting (ConfigSetting setting, string value);
		string GetSetting (ConfigSetting setting);

	}
}


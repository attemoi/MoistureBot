using System;
using System.Collections.Generic;

namespace moisturebot
{
	public interface IConfig
	{
		void CreateConfig();
		bool ConfigExists();

		Dictionary<string, ulong> GetFavoriteUsers();
		Dictionary<string, ulong> GetFavoriteChatRooms();

		void AddFavoriteUser (string key, ulong userId);
		void AddFavoriteChatRoom (string key, ulong chatRoomId);

		void RemoveFavoriteUser (string key);
		void RemoveAllFavoriteUsers ();

		void RemoveFavoriteChatRoom (string key);
		void RemoveAllFavoriteChatRooms ();

		void SetValue (string section, string key, string value);
		string GetValue (string section, string key);

	}
}


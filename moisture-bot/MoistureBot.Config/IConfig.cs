using System;
using System.Collections.Generic;

namespace moisturebot
{
	public interface IConfig
	{
		void CreateConfig();
		bool ConfigExists();

		Dictionary<string, ulong> GetFavoriteFriends();
		Dictionary<string, ulong> GetFavoriteChatRooms();

		void AddFavoriteFriend (string key, ulong userId);
		void AddFavoriteChatRoom (string key, ulong chatRoomId);

		void RemoveFavoriteFriend (string key);
		void RemoveFavoriteChatRoom (string key);

		void SetValue (string section, string key, string value);
		void GetValue (string section, string key);

	}
}


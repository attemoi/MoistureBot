using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using System.IO;

namespace moisturebot
{
	public class BotConfig : IConfig
	{

		private const string FILENAME = "moisture-bot.ini";
		private System.Text.Encoding ENCODING = System.Text.Encoding.UTF8;

		private static readonly object iniLock = new object();

		public BotConfig ()
		{

		}

		private FileIniDataParser getParser() {
			var parser = new FileIniDataParser();
			return parser;
		}

		private void writeData(IniData data, FileIniDataParser parser) {
			lock(iniLock)
			{
				// TODO try catch, log errors
				parser.WriteFile (FILENAME, data, ENCODING);
			}
		}

		private IniData readData(FileIniDataParser parser) {
			lock(iniLock)
			{
				// TODO try catch, log errors
				return parser.ReadFile (FILENAME, ENCODING);
			}
		}

		#region IConfig implementation

		public void CreateConfig ()
		{
			if (ConfigExists())
				return;

			writeData(new IniData (), getParser ());
		}

		public void ResetConfig ()
		{
			if (!ConfigExists())
				CreateConfig ();

			var parser = getParser ();
			var data = new IniData ();
			parser.WriteFile (FILENAME, data, ENCODING);
		}


		public bool ConfigExists ()
		{
			return File.Exists (FILENAME);
		}

		public Dictionary<string, ulong> GetFavoriteFriends ()
		{
			throw new NotImplementedException ();
		}

		public Dictionary<string, ulong> GetFavoriteChatRooms ()
		{
			throw new NotImplementedException ();
		}

		public void AddFavoriteFriend (string key, ulong userId)
		{
			throw new NotImplementedException ();
		}

		public void AddFavoriteChatRoom (string key, ulong chatRoomId)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFavoriteFriend (string key)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFavoriteChatRoom (string key)
		{
			throw new NotImplementedException ();
		}

		public void SetValue (string section, string key, string value)
		{
			throw new NotImplementedException ();
		}

		public void GetValue (string section, string key)
		{
			throw new NotImplementedException ();
		}

		#endregion

	}
}


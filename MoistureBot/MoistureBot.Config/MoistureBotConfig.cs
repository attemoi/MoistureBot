using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Linq;

namespace MoistureBot.Config
{
	public class MoistureBotConfig : IConfig
	{

		private const string FILENAME = "MoistureBot.ini";
		private System.Text.Encoding ENCODING = System.Text.Encoding.UTF8;

		private static readonly object iniLock = new object();

		public MoistureBotConfig ()
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
			var data = new IniData ();
			data.Sections.AddSection (ConfigSections.BOT_SETTINGS);
			data.Sections.AddSection (ConfigSections.FAVORITE_ROOMS);
			data.Sections.AddSection (ConfigSections.FAVORITE_USERS);
			writeData(data, getParser());
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

		public Dictionary<string, ulong> GetFavoriteUsers ()
		{
			IniData data = readData (getParser ());
			KeyDataCollection users = data [ConfigSections.FAVORITE_USERS];

			return IniCollectionToDictionary (users);
		}

		public Dictionary<string, ulong> GetFavoriteChatRooms ()
		{
			IniData data = readData (getParser ());
			KeyDataCollection rooms = data [ConfigSections.FAVORITE_ROOMS];

			return IniCollectionToDictionary (rooms);
		}

		private Dictionary<string, ulong> IniCollectionToDictionary(KeyDataCollection collection) {

			Dictionary<string, ulong> dict = new Dictionary<string, ulong> ();

			if (collection == null)
				return dict;
				
			foreach (KeyData kd in collection) {
				try {
					string key = kd.KeyName;
					ulong id = UInt64.Parse(kd.Value);
					dict.Add( key, id);
				} catch {
					// TODO log
				}
			}
			return dict;
		}

		public void AddFavoriteUser (string key, ulong userId)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[ConfigSections.FAVORITE_USERS].AddKey (key, userId.ToString ());
			writeData( data, parser );
		}

		public void AddFavoriteChatRoom (string key, ulong chatRoomId)
		{
			var parser = getParser ();
			var data = readData (parser) ;
			data[ConfigSections.FAVORITE_ROOMS].AddKey (key, chatRoomId.ToString ());
			writeData (data, parser);
		}

		public void RemoveFavoriteUser (string key)
		{
			var parser = getParser ();
			var data = readData (parser) ;
			data[ConfigSections.FAVORITE_USERS].RemoveKey (key);
			writeData(data, parser);
		}

		public void RemoveFavoriteChatRoom (string key)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[ConfigSections.FAVORITE_ROOMS].RemoveKey (key);
			writeData(data, parser);
		}

		public void SetValue (string section, string key, string value)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[section][key] = value;
			writeData(data, parser);

		}

		public string GetValue (string section, string key)
		{
			return readData (getParser ()) [section][key];
		}
			
		public void RemoveAllFavoriteUsers ()
		{
			var parser = getParser ();
			var data = readData (parser);
			data.Sections.SetSectionData (ConfigSections.FAVORITE_USERS, new SectionData (ConfigSections.FAVORITE_USERS));
			writeData(data, parser);
		}

		public void RemoveAllFavoriteChatRooms ()
		{
			var parser = getParser ();
			var data = readData (parser);
			data.Sections.SetSectionData (ConfigSections.FAVORITE_ROOMS, new SectionData (ConfigSections.FAVORITE_ROOMS));
				writeData(data, parser);
		}

		public string GetFavoriteUserId (string key)
		{
			try {
				return readData (getParser ()) [ConfigSections.FAVORITE_USERS][key];
			} catch {
				return null;
			}
		}

		public string GetFavoriteChatRoomId (string key)
		{
			try {
				return readData (getParser ()) [ConfigSections.FAVORITE_ROOMS][key];
			} catch {
				return null;
			}
		}

		#endregion

	}
}


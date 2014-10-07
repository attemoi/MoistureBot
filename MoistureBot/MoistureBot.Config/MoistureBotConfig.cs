using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MoistureBot.Config
{
	public class MoistureBotConfig : IConfig
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static string SECTION_FAVORITE_USERS = "favorite_users";
		private static string SECTION_FAVORITE_ROOMS = "favorite_rooms";

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
				try {
					parser.WriteFile (FILENAME, data, ENCODING);
				} catch (Exception e) {
					log.Error ("Failed to write data", e);
				}
			}
		}

		private IniData readData(FileIniDataParser parser) {
			lock(iniLock)
			{
				try {
					return parser.ReadFile (FILENAME, ENCODING);
				} catch (Exception e) {
					log.Error ("Failed to read data", e);
				}

				return null;
			}
		}

		private string GetSection(ConfigSetting settings) {
			return EnumUtils.GetValue<SectionAttribute> (settings);
		}

		private string GetKey(ConfigSetting settings) {
			return EnumUtils.GetValue<KeyAttribute> (settings);
		}

		#region IConfig implementation

		public void CreateConfig ()
		{

			if (ConfigExists ()) {
				log.Debug ("Config already exists");
				return;
			}

			log.Debug ("Config file not found, creating... ");

			var data = new IniData ();
			data.Sections.AddSection (SECTION_FAVORITE_ROOMS);
			data.Sections.AddSection (SECTION_FAVORITE_USERS);
			writeData(data, getParser());

			SetSetting (
				ConfigSetting.STATUS, 
				EnumUtils.GetValue<StringAttribute> (OnlineStatus.ONLINE));
		}

		public void ResetConfig ()
		{
			log.Debug ("Resetting config " + FILENAME);

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
			KeyDataCollection users = data [SECTION_FAVORITE_USERS];

			return IniCollectionToDictionary (users);
		}

		public Dictionary<string, ulong> GetFavoriteChatRooms ()
		{
			IniData data = readData (getParser ());
			KeyDataCollection rooms = data [SECTION_FAVORITE_ROOMS];

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
					log.Error ("Failed to parse favorite with key " + kd.Value);
				}
			}
			return dict;
		}
			
		public bool AddFavoriteUser (string key, ulong userId)
		{
			log.Debug ("Adding favorite user: key '" + key + "' + id: '" + userId + "'");

			var parser = getParser ();
			var data = readData (parser);

			CreateSectionIfNotExists (data, SECTION_FAVORITE_USERS);

			if (data [SECTION_FAVORITE_USERS].ContainsKey (key))
				return false;

			data[SECTION_FAVORITE_USERS].AddKey (key, userId.ToString ());
			writeData (data, parser);
			return true;

		}

		public bool AddFavoriteChatRoom (string key, ulong chatRoomId)
		{
			log.Debug ("Adding favorite room: key '" + key + "' + id: '" + chatRoomId + "'");

			var parser = getParser ();
			var data = readData (parser);

			CreateSectionIfNotExists (data, SECTION_FAVORITE_ROOMS);

			if (data [SECTION_FAVORITE_ROOMS].ContainsKey (key))
				return false;

			data[SECTION_FAVORITE_ROOMS].AddKey (key, chatRoomId.ToString ());
			writeData (data, parser);
			return true;
		}

		public bool RemoveFavoriteUser (string key)
		{
			log.Debug ("Removing favorite user with key '" + key + "'");

			var parser = getParser ();
			var data = readData (parser) ;

			CreateSectionIfNotExists (data, SECTION_FAVORITE_USERS);

			if (!data [SECTION_FAVORITE_USERS].ContainsKey (key))
				return false;

			data[SECTION_FAVORITE_USERS].RemoveKey (key);
			writeData (data, parser);
			return true;
		}

		public bool RemoveFavoriteChatRoom (string key)
		{
			var parser = getParser ();
			var data = readData (parser);

			CreateSectionIfNotExists (data, SECTION_FAVORITE_ROOMS);

			if (!data [SECTION_FAVORITE_ROOMS].ContainsKey (key))
				return false;

			data[SECTION_FAVORITE_ROOMS].RemoveKey (key);
			writeData (data, parser);
			return true;
		}

		public void SetSetting (ConfigSetting setting, string value)
		{

			var key = GetKey (setting);
			var section = GetSection (setting);
			log.Debug("Changing ini setting '" + key + "' in section '" + section + "'");

			var parser = getParser ();
			var data = readData (parser);

			CreateSectionAndKeyIfNotExists (data, section, key);

			log.Debug ("Setting ini setting value to '" + value + "'");
			data[section][key] = value;
			writeData(data, parser);
		}

		private void CreateSectionIfNotExists(IniData data, string section) {
			if (!data.Sections.ContainsSection (section)) {
				log.Debug ("Section '" + section + "' doesn't exist, creating new...");
				data.Sections.AddSection(section);
			}
		}

		private void CreateSectionAndKeyIfNotExists(IniData data, string section, string key) {

			CreateSectionIfNotExists (data, section);

			if (!data [section].ContainsKey (key)) {
				log.Debug ("Key '" + key + "' doesn't exist, creating new...");
				data [section].AddKey (key);
			}
		}
			
		public string GetSetting (ConfigSetting setting)
		{
			var key = GetKey (setting);
			var section = GetSection (setting);
			log.Debug ("Reading ini setting '" + key + "' in section '" + section + "'");
			var value = readData (getParser ()) [GetSection(setting)][GetKey(setting)];
			log.Debug ("Found value '" + value + "'");
			return value;
		}
			
		public void RemoveAllFavoriteUsers ()
		{
			log.Debug ("Removing all favorite users...");

			var parser = getParser ();
			var data = readData (parser);
			data.Sections.RemoveSection (SECTION_FAVORITE_USERS);
			data.Sections.AddSection (SECTION_FAVORITE_USERS);
			writeData(data, parser);
		}

		public void RemoveAllFavoriteChatRooms ()
		{
			log.Debug ("Removing all favorite chatrooms...");

			var parser = getParser ();
			var data = readData (parser);
			data.Sections.RemoveSection (SECTION_FAVORITE_ROOMS);
			data.Sections.AddSection (SECTION_FAVORITE_ROOMS);
			writeData(data, parser);
		}

		public string GetFavoriteUserId (string key)
		{
			try {
				return readData (getParser ()) [SECTION_FAVORITE_USERS][key];
			} catch {
				return null;
			}
		}

		public string GetFavoriteChatRoomId (string key)
		{
			try {
				return readData (getParser ()) [SECTION_FAVORITE_ROOMS][key];
			} catch {
				return null;
			}
		}
			
		#endregion

	}
}


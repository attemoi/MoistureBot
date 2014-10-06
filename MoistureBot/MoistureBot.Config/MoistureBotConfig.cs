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
			return StringEnum.GetValue<SectionAttribute> (settings);
		}

		private string GetKey(ConfigSetting settings) {
			return StringEnum.GetValue<KeyAttribute> (settings);
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
				StringEnum.GetValue<StringAttribute> (OnlineStatus.ONLINE));
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

		public void AddFavoriteUser (string key, ulong userId)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[SECTION_FAVORITE_USERS].AddKey (key, userId.ToString ());
			writeData( data, parser );
		}

		public void AddFavoriteChatRoom (string key, ulong chatRoomId)
		{
			var parser = getParser ();
			var data = readData (parser) ;
			data[SECTION_FAVORITE_USERS].AddKey (key, chatRoomId.ToString ());
			writeData (data, parser);
		}

		public void RemoveFavoriteUser (string key)
		{
			var parser = getParser ();
			var data = readData (parser) ;
			data[SECTION_FAVORITE_USERS].RemoveKey (key);
			writeData(data, parser);
		}

		public void RemoveFavoriteChatRoom (string key)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[SECTION_FAVORITE_ROOMS].RemoveKey (key);
			writeData(data, parser);
		}

		public void SetSetting (ConfigSetting setting, string value)
		{
			var parser = getParser ();
			var data = readData (parser);
			data[GetSection(setting)][GetKey(setting)] = value;
			writeData(data, parser);
		}

		public string GetSetting (ConfigSetting setting)
		{
			return readData (getParser ()) [GetSection(setting)][GetKey(setting)];
		}

		// TODO: limit generics
		private string GetKey(Enum value)
		{
			string output = null;
			Type type = value.GetType();

			FieldInfo fi = type.GetField(value.ToString());
			KeyAttribute[] attrs =
				fi.GetCustomAttributes(typeof(KeyAttribute),
					false) as KeyAttribute[];
			if (attrs.Length > 0)
			{
				output = attrs[0].Value;
			}

			return output;
		}
			
		public void RemoveAllFavoriteUsers ()
		{
			var parser = getParser ();
			var data = readData (parser);
			data.Sections.SetSectionData (SECTION_FAVORITE_USERS, new SectionData (SECTION_FAVORITE_ROOMS));
			writeData(data, parser);
		}

		public void RemoveAllFavoriteChatRooms ()
		{
			var parser = getParser ();
			var data = readData (parser);
			data.Sections.SetSectionData (SECTION_FAVORITE_ROOMS, new SectionData (SECTION_FAVORITE_ROOMS));
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


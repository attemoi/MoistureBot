using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using MoistureBot.Config;
using MoistureBot.Steam;

namespace MoistureBot
{

    [Extension(typeof(IConfig))]
    public class MoistureBotConfig : IConfig
    {

        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        private static string SECTION_FAVORITE_USERS = "favorite_users";
        private static string SECTION_FAVORITE_ROOMS = "favorite_rooms";

        private const string FILENAME = "MoistureBot.ini";
        private System.Text.Encoding ENCODING = System.Text.Encoding.UTF8;

        private static readonly object iniLock = new object();

        public MoistureBotConfig()
        {

        }

        private FileIniDataParser getParser()
        {
            var parser = new FileIniDataParser();
            return parser;
        }

        private void WriteData(IniData data, FileIniDataParser parser)
        {
            lock (iniLock)
            {
                try
                {
                    parser.WriteFile(FILENAME, data, ENCODING);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to write data", e);
                }
            }
        }

        private IniData ReadData(FileIniDataParser parser)
        {
            lock (iniLock)
            {
                try
                {
                    return parser.ReadFile(FILENAME, ENCODING);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to read data", e);
                }

                return null;
            }
        }

        private string GetSection(Enum settings)
        {
            return ConfigUtils.GetValue<SectionAttribute>(settings);
        }

        private string GetKey(Enum settings)
        {
            return ConfigUtils.GetValue<KeyAttribute>(settings);
        }

        #region IConfig implementation

        public void CreateConfig()
        {

            if (ConfigExists())
            {
                Logger.Info("Config already exists");
                return;
            }

            Logger.Info("Config file not found, creating... ");

            var data = new IniData();
            data.Sections.AddSection(SECTION_FAVORITE_ROOMS);
            data.Sections.AddSection(SECTION_FAVORITE_USERS);
            WriteData(data, getParser());

            SetSetting(
                ConfigSetting.STATUS, 
                ConfigUtils.GetValue<StringAttribute>(OnlineStatus.ONLINE));
        }

        public void ResetConfig()
        {
            Logger.Info("Resetting config " + FILENAME);

            if (!ConfigExists())
                CreateConfig();


            var parser = getParser();
            var data = new IniData();
            parser.WriteFile(FILENAME, data, ENCODING);
        }

        public bool ConfigExists()
        {
            return File.Exists(FILENAME);
        }

        public Dictionary<string, ulong> GetFavoriteUsers()
        {
            IniData data = ReadData(getParser());
            KeyDataCollection users = data[SECTION_FAVORITE_USERS];

            return IniCollectionToDictionary(users);
        }

        public Dictionary<string, ulong> GetFavoriteChatRooms()
        {
            IniData data = ReadData(getParser());
            KeyDataCollection rooms = data[SECTION_FAVORITE_ROOMS];

            return IniCollectionToDictionary(rooms);
        }

        private Dictionary<string, ulong> IniCollectionToDictionary(KeyDataCollection collection)
        {

            Dictionary<string, ulong> dict = new Dictionary<string, ulong>();

            if (collection == null)
                return dict;
				
            foreach (KeyData kd in collection)
            {
                try
                {
                    string key = kd.KeyName;
                    ulong id = UInt64.Parse(kd.Value);
                    dict.Add(key, id);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse favorite with key " + kd.Value, e);
                }
            }
            return dict;
        }

        public bool AddFavoriteUser(string key, ulong userId)
        {
            Logger.Info("Adding favorite user: key '" + key + "' + id: '" + userId + "'");

            var parser = getParser();
            var data = ReadData(parser);

            CreateSectionIfNotExists(data, SECTION_FAVORITE_USERS);

            if (data[SECTION_FAVORITE_USERS].ContainsKey(key))
                return false;

            data[SECTION_FAVORITE_USERS].AddKey(key, userId.ToString());
            WriteData(data, parser);
            return true;

        }

        public bool AddFavoriteChatRoom(string key, ulong chatRoomId)
        {
            Logger.Info("Adding favorite room: key '" + key + "' + id: '" + chatRoomId + "'");

            var parser = getParser();
            var data = ReadData(parser);

            CreateSectionIfNotExists(data, SECTION_FAVORITE_ROOMS);

            if (data[SECTION_FAVORITE_ROOMS].ContainsKey(key))
                return false;

            data[SECTION_FAVORITE_ROOMS].AddKey(key, chatRoomId.ToString());
            WriteData(data, parser);
            return true;
        }

        public bool RemoveFavoriteUser(string key)
        {
            Logger.Info("Removing favorite user with key '" + key + "'");

            var parser = getParser();
            var data = ReadData(parser);

            CreateSectionIfNotExists(data, SECTION_FAVORITE_USERS);

            if (!data[SECTION_FAVORITE_USERS].ContainsKey(key))
                return false;

            data[SECTION_FAVORITE_USERS].RemoveKey(key);
            WriteData(data, parser);
            return true;
        }

        public bool RemoveFavoriteChatRoom(string key)
        {
            var parser = getParser();
            var data = ReadData(parser);

            CreateSectionIfNotExists(data, SECTION_FAVORITE_ROOMS);

            if (!data[SECTION_FAVORITE_ROOMS].ContainsKey(key))
                return false;

            data[SECTION_FAVORITE_ROOMS].RemoveKey(key);
            WriteData(data, parser);
            return true;
        }

        public void SetSetting(string section, string key, string value)
        {

            Logger.Info("Changing ini setting '" + key + "' in section '" + section + "'");

            var parser = getParser();
            var data = ReadData(parser);

            CreateSectionAndKeyIfNotExists(data, section, key);

            Logger.Info("Setting ini setting value to '" + value + "'");
            data[section][key] = value;
            WriteData(data, parser);
        }


        public void SetSetting(ConfigSetting setting, string value)
        {
            SetSetting(GetSection(setting), GetKey(setting), value);
        }

        private void CreateSectionIfNotExists(IniData data, string section)
        {
            if (!data.Sections.ContainsSection(section))
            {
                Logger.Info("Section '" + section + "' doesn't exist, creating new...");
                data.Sections.AddSection(section);
            }
        }

        private void CreateSectionAndKeyIfNotExists(IniData data, string section, string key)
        {

            CreateSectionIfNotExists(data, section);

            if (!data[section].ContainsKey(key))
            {
                Logger.Info("Key '" + key + "' doesn't exist, creating new...");
                data[section].AddKey(key);
            }
        }

        public string GetSetting(ConfigSetting setting)
        {
            return GetSetting(GetSection(setting), GetKey(setting));
        }

        public string GetSetting(string section, string key)
        {
            try
            {
                Logger.Info("Reading ini setting '" + key + "' in section '" + section + "'");
                var data = ReadData(getParser());

                if (!data.Sections.ContainsSection(section))
                {
                    Logger.Info("Config section not found."); 
                    return null;
                }

                if (!data[section].ContainsKey(key))
                {
                    Logger.Info("Config key not found."); 
                    return null;
                }

                var value = ReadData(getParser())[section][key];
                Logger.Info("Found value '" + value + "'");
                return value;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to read setting", e);
                return null;
            }
        }

        public void RemoveAllFavoriteUsers()
        {
            Logger.Info("Removing all favorite users...");

            var parser = getParser();
            var data = ReadData(parser);
            data.Sections.RemoveSection(SECTION_FAVORITE_USERS);
            data.Sections.AddSection(SECTION_FAVORITE_USERS);
            WriteData(data, parser);
        }

        public void RemoveAllFavoriteChatRooms()
        {
            Logger.Info("Removing all favorite chatrooms...");

            var parser = getParser();
            var data = ReadData(parser);
            data.Sections.RemoveSection(SECTION_FAVORITE_ROOMS);
            data.Sections.AddSection(SECTION_FAVORITE_ROOMS);
            WriteData(data, parser);
        }

        public string GetFavoriteUserId(string key)
        {
            try
            {
                return ReadData(getParser())[SECTION_FAVORITE_USERS][key];
            }
            catch
            {
                return null;
            }
        }

        public string GetFavoriteChatRoomId(string key)
        {
            try
            {
                return ReadData(getParser())[SECTION_FAVORITE_ROOMS][key];
            }
            catch
            {
                return null;
            }
        }

        #endregion

    }
}


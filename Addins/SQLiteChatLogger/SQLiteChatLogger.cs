using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System.Collections.Generic;

namespace MoistureBot
{

    public class SQLiteChatLogger : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {
        const string CONFIG_SECTION = "sqlite_chat_logger";
        const string CONFIG_KEY = "database_path";

        const string URL_REGEX = @"\b(?:https?://|www\.)\S+\b";

        string connectionString;

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();
        private IConfig Config = MoistureBotComponentProvider.GetConfig();

        public SQLiteChatLogger()
        {
		
            var dbPath = Config.GetSetting(CONFIG_SECTION, CONFIG_KEY);

            if (String.IsNullOrEmpty(dbPath))
            {
                dbPath = "chat_log.db";
                Logger.Warn("Database path not defined. Setting default value to '" + dbPath + "'");
                Config.SetSetting(CONFIG_SECTION, CONFIG_KEY, "chat_log.db");
            } 

            connectionString = "URI=file:" + dbPath;

            try
            {
                if (File.Exists(dbPath))
                {
                    Logger.Info("Chat log database found");
                }
                else
                {
                    Logger.Info("Chat log database not found, creating...");
                    SqliteConnection.CreateFile(dbPath);
                    Logger.Info("Database created succesfully!");
                    Logger.Info("Creating tables for database");

                    CreateTables();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to create database", e);
            }
        }

        private void CreateTables()
        {

            Logger.Info("Creating tables...");

            try
            {
                using (var conn = (IDbConnection)new SqliteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"

						CREATE TABLE IF NOT EXISTS friend_chat (			
							timestamp TEXT NOT NULL,
							user_id TEXT NOT NULL,
							user_persona_name TEXT NOT NULL,
							message TEXT NOT NULL
						);

						CREATE TABLE IF NOT EXISTS group_chat (
							timestamp TEXT NOT NULL,
							user_id TEXT NOT NULL,
							user_persona_name TEXT NOT NULL,
							room_id TEXT NOT NULL,
							message TEXT NOT NULL
						);

						CREATE TABLE IF NOT EXISTS group_chat_urls (
							timestamp TEXT NOT NULL,
							user_id TEXT NOT NULL,
							user_persona_name TEXT NOT NULL,
							room_id TEXT NOT NULL,
							url TEXT NOT NULL
						);

						CREATE TABLE IF NOT EXISTS friend_chat_urls (
							timestamp TEXT NOT NULL,
							user_id TEXT NOT NULL,
							user_persona_name TEXT NOT NULL,
							url TEXT NOT NULL
						);
					";

                    conn.Open();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    Logger.Info("Log tables created");
                }

            }
            catch (Exception e)
            {
                Logger.Error("Failed to create tables", e);
            }
			
        }

        public void MessageReceived(FriendChatMessage message)
        {
            Logger.Info("Logging friend chat message to SQLite");
		
            var sql = "INSERT INTO friend_chat (timestamp, user_id, user_persona_name, message)" +
             " VALUES (@timestamp, @user_id, @user_persona_name, @message)";

            var personaName = Bot.GetPersonaName(message.ChatterId);

            List<SqliteParameter> parameters = new List<SqliteParameter>();
            String time = formatDateTime(DateTime.Now);
            parameters.Add(new SqliteParameter("@timestamp", time));
            parameters.Add(new SqliteParameter("@message", message.Message));
            parameters.Add(new SqliteParameter("@user_id", message.ChatterId));
            parameters.Add(new SqliteParameter("@user_persona_name", personaName));

            ExecuteQuery(sql, parameters);

            // Check if message contains urls
            MatchCollection matches = Regex.Matches(message.Message, URL_REGEX);
            if (matches == null)
                return;

            foreach (Match m in matches)
            {
                Logger.Info("Found url in message, inserting to database");

                var urlSql = "INSERT INTO friend_chat_urls (timestamp, user_id, user_persona_name, url)" +
                 " VALUES (@timestamp, @user_id, @user_persona_name, @url)";

                var urlParameters = new List<SqliteParameter>();

                urlParameters.Add(new SqliteParameter("@timestamp", time));
                urlParameters.Add(new SqliteParameter("@user_id", message.ChatterId));
                urlParameters.Add(new SqliteParameter("@user_persona_name", personaName));
                urlParameters.Add(new SqliteParameter("@url", m.Value));

                ExecuteQuery(urlSql, urlParameters);
            }
		
		
        }

        public void MessageReceived(GroupChatMessage message)
        {

            Logger.Info("Logging group chat message to SQLite");

            var sql = "INSERT INTO group_chat (timestamp, user_id, user_persona_name, room_id, message)" +
             " VALUES (@timestamp, @user_id, @user_persona_name, @room_id, @message)";

            var personaName = Bot.GetPersonaName(message.ChatterId);

            var parameters = new List<SqliteParameter>();
            var time = formatDateTime(DateTime.Now);
            parameters.Add(new SqliteParameter("@timestamp", time));
            parameters.Add(new SqliteParameter("@user_id", message.ChatterId));
            parameters.Add(new SqliteParameter("@user_persona_name", personaName));
            parameters.Add(new SqliteParameter("@room_id", message.ChatId));
            parameters.Add(new SqliteParameter("@message", message.Message));

            ExecuteQuery(sql, parameters);

            // Check if message contains urls
            MatchCollection matches = Regex.Matches(message.Message, URL_REGEX);
            if (matches == null)
                return;

            foreach (Match m in matches)
            {
                Logger.Info("Found url in message, inserting to database");

                var urlSql = "INSERT INTO group_chat_urls (timestamp, user_id, user_persona_name, room_id, url)" +
                 " VALUES (@timestamp, @user_id, @user_persona_name, @room_id, @url)";

                var urlParameters = new List<SqliteParameter>();

                urlParameters.Add(new SqliteParameter("@timestamp", time));
                urlParameters.Add(new SqliteParameter("@user_id", message.ChatterId));
                urlParameters.Add(new SqliteParameter("@user_persona_name", personaName));
                urlParameters.Add(new SqliteParameter("@room_id", message.ChatId));
                urlParameters.Add(new SqliteParameter("@url", m.Value));

                ExecuteQuery(urlSql, urlParameters);
            }


        }

        private void ExecuteQuery(string sql, IEnumerable<SqliteParameter> parameters)
        {

            try
            {
                using (var conn = (IDbConnection)new SqliteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    foreach (SqliteParameter p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to log chat message to sqlite database", e);
            }
		
        }
            
        private string formatDateTime(DateTime datetime)
        {
            return datetime.ToString("u");
        }
			
    }
}


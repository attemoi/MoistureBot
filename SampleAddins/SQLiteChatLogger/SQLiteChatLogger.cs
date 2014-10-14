using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using System.Data.SQLite;
using System.Data;
using System.IO;

[assembly:Addin("SQLiteChatLogger", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Logs received messages to sqlite.")]
[assembly:AddinName("SQLiteChatLogger")]
[assembly:AddinUrl("")]
namespace MoistureBot
{

	[Extension(typeof(IReceiveFriendChatMessages))]
	[Extension(typeof(IReceiveGroupChatMessages))]
	public class SQLiteChatLogger : IReceiveFriendChatMessages, IReceiveGroupChatMessages
	{
		const string CONFIG_SECTION = "sqlite_chat_logger";
		const string CONFIG_KEY = "database_path";

		string connectionString;

		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IConfig Config = MoistureBotComponentProvider.GetConfig();

		public SQLiteChatLogger() 
		{
		
			var dbPath = Config.GetSetting(CONFIG_SECTION,CONFIG_KEY);
			connectionString = "URI=file:" + dbPath;

			if (String.IsNullOrEmpty(connectionString))
			{
				Logger.Warn("Database path not defined. Setting default value to 'chat_log.db'");
				Config.SetSetting(CONFIG_SECTION,CONFIG_KEY,"chat_log.db");
			} 
			else 
			{
				try 
				{
					Logger.Info("Creating sqlite database...");
					SQLiteConnection.CreateFile(dbPath);
					Logger.Info("Database created succesfully!");
					Logger.Info("Creating tables for database");

					CreateTables();
				}
				catch (Exception e)
				{
					Logger.Error("Failed to create database",e);
				}
			}
		}

		private void CreateTables() 
		{
			try {
				using(var conn = new SQLiteConnection(connectionString))
				{

					var sql = @"

						CREATE TABLE IF NOT EXISTS friend_chat (
						timestamp TEXT NOT NULL,
						user_id TEXT NOT NULL,
						message TEXT NOT NULL
						);

						CREATE TABLE IF NOT EXISTS group_chat (
						timestamp TEXT NOT NULL,
						user_id TEXT NOT NULL,
						room_id TEXT NOT NULL,
						message TEXT NOT NULL
						);

					";

					Logger.Info("Creating tables...");
					conn.Open();
					var sqlCommand = new SQLiteCommand(sql, conn);
					sqlCommand.ExecuteNonQuery();

				}
			} 
			catch(Exception e)
			{
				Logger.Error("Failed to create tables",e);
			}
		}
			
		public void MessageReceived(FriendChatMessage message)
		{
			using(var conn = new SQLiteConnection(connectionString))
			{

				conn.Open();
			
				var insertSQL = new SQLiteCommand("INSERT INTO friend_chat (timestamp, user_id, message)" +
					" VALUES (@timestamp, @user_id, @message)", conn);
				insertSQL.Parameters.Add(new SQLiteParameter("@timestamp", DateTimeSQLite(DateTime.Now)));
				insertSQL.Parameters.Add(new SQLiteParameter("@message", message.Message));
				insertSQL.Parameters.Add(new SQLiteParameter("@user_id", message.ChatterId));

				insertSQL.ExecuteNonQuery();
			}
		}
			
		public void MessageReceived(GroupChatMessage message)
		{
			using(var conn = new SQLiteConnection(connectionString))
			{

				conn.Open();

				var insertSQL = new SQLiteCommand("INSERT INTO group_chat (timestamp, user_id, room_id, message)" +
					" VALUES (@timestamp, @user_id, @room_id, @message)", conn);
				insertSQL.Parameters.Add(new SQLiteParameter("@timestamp", DateTimeSQLite(DateTime.Now)));
				insertSQL.Parameters.Add(new SQLiteParameter("@message", message.Message));
				insertSQL.Parameters.Add(new SQLiteParameter("@user_id", message.ChatterId));
				insertSQL.Parameters.Add(new SQLiteParameter("@room_id", message.ChatId ));

				insertSQL.ExecuteNonQuery();
			}
		}

		private string DateTimeSQLite(DateTime datetime)
		{
			string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
			return string.Format(dateTimeFormat, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second,datetime.Millisecond);
		}
			
	}
}


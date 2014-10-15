using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using Mono.Data.Sqlite;
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

			if (String.IsNullOrEmpty(dbPath))
			{
				dbPath = "chat_log.db";
				Logger.Warn("Database path not defined. Setting default value to '" + dbPath + "'");
				Config.SetSetting(CONFIG_SECTION,CONFIG_KEY,"chat_log.db");
			} 

			connectionString = "URI=file:" + dbPath;

			try 
			{
				Logger.Info("Creating sqlite database...");
				SqliteConnection.CreateFile(dbPath);
				Logger.Info("Database created succesfully!");
				Logger.Info("Creating tables for database");

				CreateTables();
			}
			catch (Exception e)
			{
				Logger.Error("Failed to create database",e);
			}
		}

		private void CreateTables() 
		{

			Logger.Info("Creating tables...");

			IDbConnection conn = null;
			IDbCommand cmd = null;
			try 
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

				conn = (IDbConnection) new SqliteConnection(connectionString);
				conn.Open();

				cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();

				Logger.Info("Log tables created");

			} 
			catch(Exception e)
			{
				Logger.Error("Failed to create tables",e);
			}
			finally
			{
				if (cmd != null)
				{
					cmd.Dispose();
					cmd = null;
				}
				if (conn != null)
				{
					conn.Close();
					conn = null;
				}
			}
		}
			
		public void MessageReceived(FriendChatMessage message)
		{
			IDbConnection conn = null;
			IDbCommand cmd = null;
			try 
			{
				conn = (IDbConnection) new SqliteConnection(connectionString);
				conn.Open();
			
				var sql = "INSERT INTO friend_chat (timestamp, user_id, message)" +
				          " VALUES (@timestamp, @user_id, @message)";
				cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;

				cmd.Parameters.Add(new SqliteParameter("@timestamp", DateTimeSQLite(DateTime.Now)));
				cmd.Parameters.Add(new SqliteParameter("@message", message.Message));
				cmd.Parameters.Add(new SqliteParameter("@user_id", message.ChatterId));

				cmd.ExecuteNonQuery();
			}
			catch(Exception e)
			{
				Logger.Error("Failed to log friend chat message to sqlite database",e);
			}
			finally
			{
				if (cmd != null)
				{
					cmd.Dispose();
					cmd = null;
				}
				if (conn != null)
				{
					conn.Close();
					conn = null;
				}
			}
		}
			
		public void MessageReceived(GroupChatMessage message)
		{
			IDbConnection conn = null;

			try 
			{
				conn = (IDbConnection)new SqliteConnection(connectionString);
				conn.Open();

				var sql = "INSERT INTO group_chat (timestamp, user_id, room_id, message)" +
					" VALUES (@timestamp, @user_id, @room_id, @message)";
				var cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;

				cmd.Parameters.Add(new SqliteParameter("@timestamp", DateTimeSQLite(DateTime.Now)));
				cmd.Parameters.Add(new SqliteParameter("@message", message.Message));
				cmd.Parameters.Add(new SqliteParameter("@user_id", message.ChatterId));
				cmd.Parameters.Add(new SqliteParameter("@room_id", message.ChatId ));

				cmd.ExecuteNonQuery();
			}
			catch(Exception e)
			{
				Logger.Error("Failed to log group chat message to sqlite database",e);
			}
			finally
			{
				if (conn != null)
					conn.Close();
			}

		}

		private string DateTimeSQLite(DateTime datetime)
		{
			string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
			return string.Format(dateTimeFormat, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second,datetime.Millisecond);
		}
			
	}
}


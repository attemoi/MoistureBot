using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System.Globalization;
using System.Data;

namespace MoistureBot
{
    public class HistoryCommand : IChatCommand
    {

        const string CONFIG_SECTION = "sqlite_chat_logger";
        const string CONFIG_KEY = "database_path";

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();
        private IConfig Config = MoistureBotComponentProvider.GetConfig();

        string connectionString;

        public HistoryCommand()
        {
            var dbPath = Config.GetSetting(CONFIG_SECTION, CONFIG_KEY);
            connectionString = "URI=file:" + dbPath;
        }

        public void Execute(Command command) {
            var urls = getMessages(command, 20);
            var message = ".\n" + String.Join("\n", urls);              
            Bot.SendChatMessage(message, command.SenderId);
        }

        public void Help(Command command) {
            Bot.SendChatMessage("Displays the last 20 messages sent to a group chat.", command.SenderId);
        }

        private IEnumerable<String> getMessages(Command command, int count) {
            Logger.Info("Fetching messages...");
            var messages = new List<String>();
            try
            {
                using (var conn = (IDbConnection)new SqliteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    var isGroup = command.Source == Command.CommandSource.GROUPCHAT;
                    String table, whereClause;
                    if (isGroup)
                    {
                        table = "group_chat";
                        whereClause = " WHERE room_id = " + command.ChatRoomId;
                    }
                    else
                    {
                        table = "friend_chat";
                        whereClause = " WHERE user_id = " + command.SenderId;
                    }

                    String sql = 
                        "SELECT timestamp, user_persona_name, message from " + table
                        + whereClause
                        + " ORDER BY timestamp DESC"
                        + " LIMIT " + count;                      

                    conn.Open();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    var r = cmd.ExecuteReader();

                    while (r.Read())
                    {

                        var date = DateTime.ParseExact(r.GetString(0), "u", CultureInfo.InvariantCulture);
                        var user = r.GetString(1);
                        var message = r.GetString(2);
                        messages.Add(date.ToString("[yyyy-MM-dd HH:mm:ss] ") + user + ": " + message);
                    }

                    messages.Reverse();

                }

            }
            catch (Exception e)
            {
                Logger.Error("Failed to fetch messages", e);
            }

            return messages;

        }
            
    }
}


using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System.Globalization;

namespace MoistureBot
{
    public class HistoryCommand : IChatCommand
    {

        const string CONFIG_SECTION = "sqlite_chat_logger";
        const string CONFIG_KEY = "database_path";

        const int MESSAGE_COUNT = 20;

        IMoistureBot Bot = new MoistureBotFactory().GetBot();
        ILogger Logger = new MoistureBotFactory().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IConfig Config = new MoistureBotFactory().GetConfig();

        string connectionString;

        public HistoryCommand()
        {
            var dbPath = Config.GetSetting(CONFIG_SECTION, CONFIG_KEY);
            connectionString = "URI=file:" + dbPath;
        }

        public void Execute(Command command) {

            uint pageNum;
            try 
            {
                pageNum = parsePageNum(command);
            }
            catch (Exception)
            {
                Bot.SendChatMessage("Sorry, the page number seems invalid.", command.SenderId);
                return;
            }

            var urls = getMessages(command, pageNum);
            var message = ".\n" + String.Join("\n", urls);              
            Bot.SendChatMessage(message, command.SenderId);
        }

        public void Help(Command command) {

            var helpMessage = 
                @"Displays group chat message history.

!history
    displays the first page of chat history (20 messages / page).

!history <page_number>
    displays a specific page of chat history.
";

            Bot.SendChatMessage(helpMessage, command.SenderId);
        }

        private IEnumerable<String> getMessages(Command command, uint page) {
            Logger.Info("Fetching messages...");
            var messages = new List<String>();
            try
            {
                using (var conn = (IDbConnection)new SqliteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                                          
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = getSelectClause(command, page);
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
            

        private String getSelectClause( Command command, uint page ) {
            
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

            return
                "SELECT timestamp, user_persona_name, message from " + table
                + whereClause
                + " ORDER BY timestamp DESC"
                + " LIMIT " + MESSAGE_COUNT
                + " OFFSET " + (page - 1) * MESSAGE_COUNT;
            
        }

        private uint parsePageNum(Command command) {
            if (!command.HasArguments())
                return 1;

            return UInt32.Parse(command.Arguments[0]);
        }
    }
}


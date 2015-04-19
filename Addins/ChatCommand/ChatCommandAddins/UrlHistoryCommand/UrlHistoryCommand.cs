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
    public class UrlHistoryCommand : IChatCommand
    {

        const string CONFIG_SECTION = "sqlite_chat_logger";
        const string CONFIG_KEY = "database_path";

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();
        private IConfig Config = MoistureBotComponentProvider.GetConfig();

        const int URL_COUNT = 20;

        string connectionString;

        public UrlHistoryCommand()
        {
            var dbPath = Config.GetSetting(CONFIG_SECTION, CONFIG_KEY);
            connectionString = "URI=file:" + dbPath;
        }

        public void Execute(Command command) {
            if (command.Source == Command.CommandSource.GROUPCHAT)
            {

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

                var urls = getUrls(command, pageNum);
                var message = ".\n" + String.Join("\n", urls);
              
                Bot.SendChatMessage(message, command.SenderId);
            }
            else
            {
                Bot.SendChatMessage("This command is only available in group chat.", command.SenderId);
            }
        }

        public void Help(Command command) {
            
            var helpMessage = 
                @"Displays history of urls sent to a group chat.

!urlhistory
    displays the first page of url history (20 messages / page).

!urlhistory <page_number>
    displays a specific page of url history.
";

            Bot.SendChatMessage( 
                helpMessage, command.SenderId);
        }

        private IEnumerable<String> getUrls(Command command, uint page) {
            Logger.Info("Fetching urls...");
            var urls = new List<String>();
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
                        var url = r.GetString(2);
                        urls.Add(date.ToString("[yyyy-MM-dd HH:mm:ss] ") + user + ": " + url);
                    }

                    urls.Reverse();

                }

            }
            catch (Exception e)
            {
                Logger.Error("Failed to fetch urls", e);
            }

            return urls;

        }
            
        string getSelectClause(Command command, uint page)
        {
            return 
                  " SELECT timestamp, user_persona_name, url from group_chat_urls "
                + " WHERE room_id = '" + command.ChatRoomId + "'"
                + " ORDER BY timestamp DESC"
                + " LIMIT " + URL_COUNT
                + " OFFSET " + (page - 1) * URL_COUNT;
                   
        }

        private uint parsePageNum(Command command) {
            if (!command.HasArguments())
                return 1;

            return UInt32.Parse(command.Arguments[0]);
        }
    }
}


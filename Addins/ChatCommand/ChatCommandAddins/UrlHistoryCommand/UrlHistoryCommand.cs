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

        string connectionString;

        public UrlHistoryCommand()
        {
            var dbPath = Config.GetSetting(CONFIG_SECTION, CONFIG_KEY);
            connectionString = "URI=file:" + dbPath;
        }

        public void Execute(Command command) {
            var urls = getUrls();
            var message = ".\n" + String.Join("\n", urls);
            Bot.SendChatMessage(message, command.SenderId);
        }

        public void Help(Command command) {
            Bot.SendChatMessage("Displays the last 20 urls sent to a group chat.", command.SenderId);
        }
            
        private IEnumerable<String> getUrls() {
            Logger.Info("Fetching urls...");
            var urls = new List<String>();
            try
            {
                using (var conn = (IDbConnection)new SqliteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
                        SELECT timestamp, user_persona_name, url from group_chat_urls 
                        ORDER BY timestamp DESC
                        LIMIT 20                      
                    ";

                    conn.Open();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    var r = cmd.ExecuteReader();


                    while (r.Read())
                    {

                        var date = DateTime.ParseExact(r.GetString(0), "u", CultureInfo.InvariantCulture);
                        var user = r.GetString(1);
                        var url = r.GetString(2);
                        urls.Add(date.ToString("[yyyy-MM-dd HH:mm:ss] ") + user + ": " + url);
                    }

                }

            }
            catch (Exception e)
            {
                Logger.Error("Failed to fetch urls", e);
            }

            return urls;

        }
            
    }
}


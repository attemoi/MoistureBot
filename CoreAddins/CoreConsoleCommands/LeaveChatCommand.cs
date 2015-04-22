using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using MoistureBot.ExtensionAttributes;
using MoistureBot.Utils;

namespace MoistureBot.ConsoleCommands
{

    [ConsoleCommand(
        Name = "leave",
        Description = "Leave chat room(s).",
        ShortDescription = "Leave chat room(s).",
        ShortUsage = "leave <chat_id|favorite_key>",
        Usage = 
		"leave <chat_id|favorite_key>\n" +
        "leave -favorites"
    )]
    public class LeaveChatCommand : IConsoleCommand
    {

        IMoistureBot Bot = new MoistureBotFactory().GetBot();
        ILogger Logger = new MoistureBotFactory().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IConfig Config = new MoistureBotFactory().GetConfig();

        public Boolean favorites;

        public OptionSet Options {
            get
            {
                return new OptionSet() { { "f|favorites", "leave all favorite rooms", 
						h => favorites = h != null}
				};
			}
		}

		public bool Execute(string[] args)
		{

			Logger.Debug("Executing command...");

			List<string> extra = Options.Parse(args);

			string chatId = null;

			if (!Bot.IsConnected())
			{
				Console.WriteLine("Not connected to Steam.");
				return false;
			}

			if (favorites)
			{
				foreach (KeyValuePair<string, ulong> fav in Config.GetFavoriteChatRooms())
				{
					Console.WriteLine("Leaving chat room '" + fav.Key + "' [" + fav.Value + "]");
					Bot.LeaveChatRoom(fav.Value);
				}
				return false;
			}

			if (extra.Count == 0)
			{
				Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("leave"));
				return false;
			}

			chatId = extra.First();

			var favId = Config.GetFavoriteChatRoomId(chatId);
			if (favId != null)
			{
				ulong id;
				try
				{
					id = UInt64.Parse(favId);
				}
				catch
				{
					Console.WriteLine("Failed to leave favorite room: Invalid id.");
					return false;
				}
				Bot.LeaveChatRoom(id);
				Console.WriteLine("Leaving favorite chat room '" + chatId + "' [" + favId + "]");
			}
			else
			{
				ulong id;
				try
				{
					id = UInt64.Parse(chatId);
				}
				catch
				{
					Console.WriteLine("Failed to leave room: Invalid id.");
					return false;
				}

				Console.WriteLine("Leaving chat room '" + chatId + "'...");
				Bot.LeaveChatRoom(id);
			}
				
			return false;
		}
	}
}


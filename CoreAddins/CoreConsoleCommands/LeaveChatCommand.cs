using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Utils;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

    public class LeaveChatCommand : IConsoleCommand
    {

        IMoistureBot Bot;
        ILogger Logger;
        IConfig Config;

        [Provide]
        public LeaveChatCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(typeof(LeaveChatCommand));
            this.Config = context.GetConfig();
        }

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


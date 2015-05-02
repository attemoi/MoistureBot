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

    public class JoinChatCommand : IConsoleCommand
    {

        IMoistureBot Bot;
        IConfig Config;

        [Provide]
        public JoinChatCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Config = context.GetConfig();
        }

        public Boolean favorites;

        public OptionSet Options {
            get
            {
                return new OptionSet() { { "f|favorites", "join all favorite rooms", 
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
					Console.WriteLine("Joining chat room '" + fav.Key + "' [" + fav.Value + "]");
					Bot.JoinChatRoom(fav.Value);
				}
				return false;
			}

			if (extra.Count == 0)
			{
				Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("join"));
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
					Console.WriteLine("Failed to join favorite room: Invalid id.");
					return false;
				}
				Bot.JoinChatRoom(id);
				Console.WriteLine("Joining favorite chat room '" + chatId + "' [" + favId + "]");
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
					Console.WriteLine("Failed to join room: Invalid id.");
					return false;
				}

				Console.WriteLine("Joining chat room '" + chatId + "'...");
				Bot.JoinChatRoom(id);
			}
				
			return false;
		}
	}
}


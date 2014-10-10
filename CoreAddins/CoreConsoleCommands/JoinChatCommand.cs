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
		Name = "join",
		Description = "Join chat room(s).",
		ShortDescription = "Join chat room(s).",
		ShortUsage = "join <chat_id|favorite_key>",
		Usage = 
		"join <chat_id|favorite_key>\n" +
		"join -favorites"
	)]
	public class JoinChatCommand : IConsoleCommand
	{

		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

		public Boolean favorites;

		public OptionSet Options
		{
			get
			{
				return new OptionSet() { 
					{ "f|favorites", "join all favorite rooms", 
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


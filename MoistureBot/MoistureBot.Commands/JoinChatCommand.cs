using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;

namespace MoistureBot.Commands
{
	public class JoinChatCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string[] Args { get; set; }

		public Boolean help;
		public Boolean favorites;

		private OptionSet options;

		public JoinChatCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "f|favorites", "join all favorite rooms", 
					h => favorites = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"Join chat rooms.", 
				"join <chat_id|favorite_key>" + Environment.NewLine +
				"  join -favorites",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			log.Debug ("Executing command: join");

			List<string> extra = options.Parse(Args);

			string chatId = null;

			if (help) {
				WriteHelp ();
				return false;
			}

			if (!bot.IsConnected ()) {
				log.Info ("Not connected to Steam.");
				return false;
			}

			if (favorites) {
				foreach(KeyValuePair<string, ulong> fav in new MoistureBotConfig().GetFavoriteChatRooms() ) {
					log.Info ("Joining chat room '" + fav.Key + "' [" + fav.Value + "]");
					bot.JoinChat (fav.Value);
				}
				return false;
			}

			if (extra.Count == 0) {
				WriteHelp ();
				return false;
			} 

			chatId = extra.First ();

			var favId = new MoistureBotConfig ().GetFavoriteChatRoomId (chatId);
			if (favId != null) {
				ulong id;
				try {
					id = UInt64.Parse(favId);
				} catch {
					log.Info("Failed to join favorite room: Invalid id.");
					return false;
				}
				bot.JoinChat (id);
				log.Info("Joining favorite chat room '" + chatId + "' [" + favId + "]" );
			} else {
				ulong id;
				try {
					id = UInt64.Parse(chatId);
				} catch {
					log.Info("Failed to join room: Invalid id.");
					return false;
				}

				log.Info( "Joining chat room '" + chatId + "'...");
				bot.JoinChat(id);
			}
				
			return false;
		}

	}
}


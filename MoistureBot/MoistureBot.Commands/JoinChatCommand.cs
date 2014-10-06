using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;

namespace MoistureBot.Commands
{
	public class JoinChatCommand : ICommand
	{

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

			List<string> extra = options.Parse(Args);

			string chatId = null;

			if (help) {
				WriteHelp ();
				return false;
			}

			if (!bot.IsConnected ()) {
				Console.WriteLine ("Not connected to Steam.");
				return false;
			}

			if (favorites) {
				foreach(KeyValuePair<string, ulong> fav in new MoistureBotConfig().GetFavoriteChatRooms() ) {
					Console.WriteLine ("Joining chat room '{0}' [{1}]", fav.Key, fav.Value );
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
					Console.WriteLine("Failed to join favorite room: Invalid id.");
					return false;
				}
				bot.JoinChat (id);
				Console.WriteLine ("Joining favorite chat room '{0}' [{1}]", chatId, favId );
			} else {
				ulong id;
				try {
					id = UInt64.Parse(chatId);
				} catch {
					Console.WriteLine ("Failed to join room: Invalid id.");
					return false;
				}

				Console.WriteLine( "Joining chat room '{0}'...", chatId);
				bot.JoinChat(id);
			}
				
			return false;
		}

	}
}


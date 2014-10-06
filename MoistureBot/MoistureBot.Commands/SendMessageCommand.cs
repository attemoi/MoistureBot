using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;

namespace MoistureBot.Commands
{
	public class SendMessageCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string[] Args { get; set; }

		public Boolean help;
		public Boolean user;
		public Boolean room;

		private OptionSet options;

		public SendMessageCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"send message to user or room", 
				"msg user <user_id|favorite_key> <message>" + Environment.NewLine +
				"  msg room <room_id|favorite_key> <message>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			log.Debug ("Executing command...");
		
			List<string> extra = options.Parse(Args);

			if (help || extra.Count < 3) {
				WriteHelp ();
				return false;
			}

			if (!bot.IsConnected ()) {
				log.Info ("Not connected to Steam.");
				return false;
			}

			string target = extra.First ();
			string inputId = extra.ElementAt (1);
			// get rest
			string message = string.Join (" ", extra.Skip (2)).Trim ('\"');

			switch (target) {
			case "user":
				var favUserId = new MoistureBotConfig ().GetFavoriteUserId (inputId);
				if (favUserId != null) {
					ulong favId;
					try {
						favId = UInt64.Parse(favUserId);
					} catch {
						log.Info("Failed to send message: Invalid favorite id.");
						return false;
					}
					log.Info ("Sending chat message to '" + favId + "'...");
					bot.SendChatMessage (message, favId);
					return false;
				} 

				ulong userId;
				try {
					userId = UInt64.Parse(inputId);
				} catch {
					log.Info ("Failed to send message to user: Invalid id!");
					return false;
				}

				log.Info ("Sending chat message to user '" + inputId + "'...");
				bot.SendChatMessage (message, userId);

				return false;
			case "room":

				var favRoomId = new MoistureBotConfig ().GetFavoriteChatRoomId (inputId);
				if (favRoomId != null) {
					ulong favId;
					try {
						favId = UInt64.Parse(favRoomId);
					} catch {
						log.Info("Failed to send message to favorite room: Invalid id!");
						return false;
					}
					log.Info ("Sending chat message to room '" + favId + "'...");
					bot.SendChatRoomMessage (message, favId);
					return false;
				} 

				ulong roomId;
				try {
					roomId = UInt64.Parse(inputId);
				} catch {
					log.Info ("Failed to send message to room: Invalid id!");
					return false;
				}

				log.Info ("Sending chat message to room '" + roomId + "'...");
				bot.SendChatRoomMessage (message, roomId);
				return false;
			default:
				WriteHelp ();
				return false;
			}


		}

	}
}


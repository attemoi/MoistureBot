using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

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
				"msg user <user_id> <message>" + Environment.NewLine +
				"  msg room <room_id> <message>",
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
			string chatId = extra.ElementAt (1);
			// get rest
			string message = string.Join (" ", extra.Skip (2)).Trim ('\"');

			ulong id;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				log.Info ("Invalid chat id!");
				return false;
			}

			switch (target) {
			case "user":
				log.Info ("Sending chat message to friend '"+id+"'...");
				bot.SendChatMessage (message, id);
				return false;
			case "room":
				log.Info ("Sending chat message to room '"+id+"'...");
				bot.SendChatRoomMessage (message, id);
				return false;
			default:
				WriteHelp ();
				return false;
			}


		}

	}
}


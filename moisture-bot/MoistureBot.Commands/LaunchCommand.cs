using System;
using Mono.Options;
using System.Collections.Generic;
using moisturebot.config;
using System.Linq;

namespace moisturebot.commands
{
	public class LaunchCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private bool connect;
		private bool joinFavs;
		private string user;
		private string pass;

		public LaunchCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					v => help = v != null },
				{ "c|connect", "Connect to Steam on launch." ,
					v => connect = v != null},
				{ "j|join-favorites", "Join favourite rooms on launch" ,
					v => joinFavs = v != null},
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp (
				"Extensible chat bot for Steam." + Environment.NewLine +
				"  Please note that in order to send messages, a steam user needs to have" + Environment.NewLine +
				"  at least one game in their library (Steam requirement).", 
				"moisture-bot [OPTIONS]+" + Environment.NewLine +
				"  moisture-bot [OPTIONS]+ [<username> [<password>]]", 
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			List<string> extra = options.Parse (Args);

			if (help) {
				WriteHelp ();
				return true;
			}
				
			if (connect || joinFavs) {

				if (extra.Count > 0)
					user = extra.First ();
				if (extra.Count > 1)
					pass = extra.ElementAt (1);
					
				if (user == null) {
					Console.WriteLine();
					Console.Write ("username:");
					user = Console.ReadLine ();
				}
				if (pass == null) {
					Console.Write ("password:");
					pass = Console.ReadLine ();
				}

				bot.Connect (user, pass);
				bot.BlockUntilConnected ();

			}

			if (joinFavs) {
				foreach(KeyValuePair<string, ulong> fav in new MoistureBotConfig().GetFavoriteChatRooms() ) {
					Console.WriteLine ("Joining chat room '{0}' [{1}]", fav.Key, fav.Value );
					bot.JoinChat (fav.Value);
				}
			}

			return false;
		}

	}
}


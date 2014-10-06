﻿using System;
using Mono.Options;
using System.Collections.Generic;
using MoistureBot.Config;
using System.Linq;

namespace MoistureBot.Commands
{
	public class LaunchCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
				"MoistureBot [OPTIONS]+" + Environment.NewLine +
				"  MoistureBot [OPTIONS]+ [<username> [<password>]]", 
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			log.Debug ("Executing command...");

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

				log.Info("Logging in as " + user + "...");
				log.Info("Type 'status' to see bot status.");
				bot.Connect (user, pass);

			}

			if (joinFavs) {
				foreach(KeyValuePair<string, ulong> fav in new MoistureBotConfig().GetFavoriteChatRooms() ) {
					log.Info ("Joining chat room '"+ fav.Key +"' ["+fav.Value+"]" );
					bot.JoinChatRoom (fav.Value);
				}
			}

			return false;
		}

	}
}


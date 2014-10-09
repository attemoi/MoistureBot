using System;
using Mono.Options;
using System.Collections.Generic;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Config;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
	[ConsoleCommand(
		Name = "status",
		Description = "Show bot status.",
		ShortDescription = "Show bot status.",
		ShortUsage = "status",
		Usage = "status"
	)]
	public class StatusCommand : ICommand
	{

		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

		public OptionSet Options {
			get {
				return new OptionSet ();
			}
		}

		public bool Execute (string[] args)
		{

			Logger.Info ("Executing command...");

			Options.Parse(args);
			
			Console.WriteLine();
			Console.WriteLine("Connection status:");
			Console.WriteLine();
			string status = EnumUtils.GetValue<NameAttribute> (Bot.GetOnlineStatus());
			if (Bot.IsConnected ()) {
				Console.WriteLine ("  logged on as {0}", Bot.User);
				Console.WriteLine ("  online status: {0}", status);
			} else {
				Console.WriteLine ("  not connected");
				Console.WriteLine ("  online status set to {0}", status);
			}

			Console.WriteLine ();

//			Console.WriteLine ("Active chatrooms: ");
//			Console.WriteLine ();
//			var activeRooms = Bot.GetActiveChatRooms ();
//			if (activeRooms.Count == 0) {
//				Console.WriteLine ("  -- no active chatrooms --");
//			} else {
//				activeRooms.ForEach( id => Console.WriteLine("  " + id));
//			}

			Console.WriteLine ();
			Console.WriteLine ("Registered addins:");
			Console.WriteLine ();
			var addins = AddinManager.Registry.GetAddins ();
			if (addins.Length == 0) {
				Console.WriteLine ("  -- no addins registered --");
			} else {
				foreach (Addin addin in addins) {
					Console.WriteLine ("  {0} {1}", addin.Name, addin.Version);
				}
			}
				
			return false;
		}
	}
}


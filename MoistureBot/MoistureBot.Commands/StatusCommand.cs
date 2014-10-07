using System;
using Mono.Options;
using System.Collections.Generic;
using Mono.Addins;
using MoistureBot;

namespace MoistureBot.Commands
{
	public class StatusCommand : ICommand
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;

		public StatusCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"show bot status", 
				"status",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			log.Debug ("Executing command...");

			options.Parse(Args);

			if (help) {
				WriteHelp ();
				return false;
			}
				
			Console.WriteLine ();
			Console.WriteLine ("Connection status:");
			Console.WriteLine ();
			string status = EnumUtils.GetValue<NameAttribute> (bot.GetOnlineStatus());
			if (bot.IsConnected ()) {
				Console.WriteLine ("  logged on as {0}", bot.User);
				Console.WriteLine ("  online status: {0}", status);
			} else {
				Console.WriteLine ("  not connected");
				Console.WriteLine ("  online status set to {0}", status);
			}

			Console.WriteLine ();
			Console.WriteLine ("Active chatrooms: ");
			Console.WriteLine ();
			var activeRooms = bot.GetActiveChatRooms ();
			if (activeRooms.Count == 0) {
				Console.WriteLine ("  -- no active chatrooms --");
			} else {
				activeRooms.ForEach( id => Console.WriteLine("  " + id));
			}
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


using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot
{
	public class LaunchOptions : IOptionParser
	{
		public OptionSet LaunchOptionSet;

		public string User { get; set; }
		public string Pass { get; set; }
		public Boolean AutoConnect { get; set; }
		public Boolean ShowHelp { get; set; }
		public List<string> Extra = new List<string>();

		public LaunchOptions ()
		{

		}

		public void Parse(string[] args) {

			LaunchOptionSet = new OptionSet () {
				{ "h|help", "show this message and exit", 
					h => ShowHelp = h != null },
				{ "a|autoconnect", "Connect automatically when run" ,
					a => AutoConnect = a != null},
				{ "u=|user=", "Steam username for autoconnect", 
					u => User = u },
				{ "p=|password=", "Steam password for autoconnect", 
					p => Pass = p }
			};
				
			try {
				Extra = LaunchOptionSet.Parse (args);
			}
			catch (OptionException e) {
				Console.Write ("moisture-bot: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try moisture-bot --help' for more information.");
				return;
			}

		}

		public void WriteHelp() 
		{
			Console.WriteLine ();
			Console.WriteLine ("Usage: moisture-bot [OPTIONS]+");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			LaunchOptionSet.WriteOptionDescriptions (Console.Out);
		}
	}
}


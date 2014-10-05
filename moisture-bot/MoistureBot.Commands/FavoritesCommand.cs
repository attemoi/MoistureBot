using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class FavoritesCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private bool list;
		private bool add;
		private bool remove;
		private List<string> roomIds = new List<string> ();
		private List<string> friendIds = new List<string> ();

		public FavoritesCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "list", "add favorite", 
					h => help = h != null },
				{ "add", "add favorite", 
					h => help = h != null },
				{ "remove", "remove favorite", 
					h => help = h != null },
				{ "r|room=", "the {id} of room to add.",
					v => roomIds.Add (v) },
				{ "f|friend=", "the {id} of room to add.",
					v => friendIds.Add (v) },
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"manage favorite rooms and friends", 
				"favorites -list" + Environment.NewLine +
				"  favorites -add <name> <id>" + Environment.NewLine +
				"  favorites -remove <name>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}



			return false;
		}

	}
}


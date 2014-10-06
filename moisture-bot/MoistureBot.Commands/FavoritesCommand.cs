using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;

namespace MoistureBot.Commands
{
	// TODO: improve error handling

	public class FavoritesCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private bool list;

		public FavoritesCommand() {

			options = new OptionSet () {
				{ "h|help", "show this message", 
					v => help = v != null },
				{ "l|list", "list favourite rooms and users", 
					v => list = v != null },
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"Manage favorite rooms and users.", 
				"favorites -list" + Environment.NewLine +
				"  favorites add user/room <key> <id>" + Environment.NewLine +
				"  favorites remove user/room <key>" + Environment.NewLine +
				"  favorites remove-all users/rooms",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			List<string> extra = options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}

			if (list) {
				var favUsers = new MoistureBotConfig ().GetFavoriteUsers ();
				var favRooms = new MoistureBotConfig ().GetFavoriteChatRooms ();

				Console.WriteLine ();
				Console.WriteLine ("Favourite rooms:");
				Console.WriteLine ();
				WriteDict (favRooms);
				Console.WriteLine ();
				Console.WriteLine ("Favourite users:");
				Console.WriteLine ();
				WriteDict (favUsers);
				return false;
			}
			if (extra.Count == 0) {
				WriteHelp ();
				return false;
			}
			if (extra.Count < 2) {
				Console.WriteLine("Invalid number of parameters. Type 'favorites -h' for help'");
				return false;
			}

			var operation = extra.First ();
			var type = extra.ElementAt (1);

			switch (operation) {
			case "add":
				try {
					if (extra.Count != 4) {
						Console.WriteLine ("Invalid number of parameters. Type 'favorites -h' for help'");
						return false;
					}
					var key = extra.ElementAt (2);
					var value = extra.ElementAt (3);
					switch (type) {
					case "user":
						new MoistureBotConfig ().AddFavoriteUser (key, UInt64.Parse (value));
						Console.WriteLine("Added user '{0}' to favorites", key);
						break;
					case "room":
						new MoistureBotConfig ().AddFavoriteChatRoom (key, UInt64.Parse (value));
						Console.WriteLine("Added room '{0}' to favorites", key);
						break;
					default:
						Console.WriteLine ("Failed to add favorite. Check parameters and try again..");
						break;
					}
				} catch (Exception e){
					//TODO: log exception
					Console.WriteLine (e.StackTrace);
					Console.WriteLine ("Failed to add favorite. Check parameters and try again.");
				}
			
				break;
			case "remove":
				try {
					if (extra.Count != 3) {
						Console.WriteLine ("Invalid number of parameters. Type 'favorites -h' for help'");
						return false;
					}
					var key = extra.ElementAt (2);
					switch (type) {
					case "user":
						new MoistureBotConfig ().RemoveFavoriteUser (key);
						Console.WriteLine("Removed user '{0}' from favorites", key);
						break;
					case "room":
						new MoistureBotConfig ().RemoveFavoriteChatRoom (key);
						Console.WriteLine("Removed room '{0}' from favorites", key);
						break;
					default:
						Console.WriteLine ("Failed to remove favorite. Check parameters and try again.");
						break;
					}
				} catch {
					//TODO: log exception
					Console.WriteLine ("Failed to remove favorite. Check parameters and try again.");
				}
				break;
			case "remove-all":
				try {
					if (extra.Count != 2) {
						Console.WriteLine ("Invalid number of parameters. Type 'favorites -h' for help.'");
						return false;
					}
					switch (type) {
					case "users":
						new MoistureBotConfig ().RemoveAllFavoriteUsers();
						Console.WriteLine("Favorite users succesfully removed.");
						break;
					case "rooms":
						new MoistureBotConfig ().RemoveAllFavoriteChatRooms();
						Console.WriteLine("Favorite rooms succesfully removed.");
						break;
					default:
						Console.WriteLine ("Failed to remove favorites. Check parameters and try again.");
						break;
					}
				} catch {
					//TODO: log exception
					Console.WriteLine ("Failed to remove favorites. Check parameters and try again.");
				}
				break;
			}
					
			return false;
		}

		private void WriteDict(Dictionary<string, ulong> dict) {
			if (dict.Count == 0) {
				Console.WriteLine ("  -- no favorites added --");
				return;
			}

			foreach(KeyValuePair<string, ulong> pair in dict){
				Console.WriteLine ("  {0} [{1}]", pair.Key, pair.Value);
			}
		}

	}
}


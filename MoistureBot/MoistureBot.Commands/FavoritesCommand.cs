using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;

namespace MoistureBot.Commands
{
	public class FavoritesCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

			log.Debug ("Executing command...");
		
			List<string> extra = options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}

			if (list || extra.Count == 0) {

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

			if (extra.Count < 2) {
				log.Info ("Invalid number of parameters. Type 'favorites -h' for help'");
				return false;
			}

			var operation = extra.First ();
			var type = extra.ElementAt (1);

			switch (operation) {
			case "add":
				try {
					if (extra.Count != 4) {
						log.Info ("Invalid number of parameters. Type 'favorites -h' for help'");
						return false;
					}
					var key = extra.ElementAt (2);
					var value = extra.ElementAt (3);
					switch (type) {
					case "user":
						try {
							new MoistureBotConfig ().AddFavoriteUser (key, UInt64.Parse (value));
							log.Info ("Added user '" + key + "' to favorites");
						} catch (Exception e){
							log.Error ("Failed to add favorite user", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					case "room":
						try {
							new MoistureBotConfig ().AddFavoriteChatRoom (key, UInt64.Parse (value));
							log.Info("Added room '" + key + "' to favorites");
						} catch (Exception e){
							log.Error ("Failed to add favorite room", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					default:
						log.Info ("Failed to add favorite. Check parameters and try again.");
						break;
					}
				} catch (Exception e){
					log.Error ("Failed to add favorite", e);
					Console.WriteLine ("Error while adding favorite. See log files for more details.");
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
						try {
							new MoistureBotConfig ().RemoveFavoriteUser (key);
							log.Info("Removed user '" + key + "' from favorites");
						} catch (Exception e){
							log.Error ("Failed to remove favorite user", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					case "room":
						try {
							new MoistureBotConfig ().RemoveFavoriteChatRoom (key);
							log.Info("Removed room '" + key + "' from favorites");
						} catch (Exception e){
							log.Error ("Failed to remove favorite room", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					default:
						log.Info ("Failed to remove favorite. Check parameters and try again.");
						break;
					}
				} catch (Exception e){
					log.Error ("Failed to add favorite", e);
					Console.WriteLine ("Error while removing favorite. See log files for more details.");
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
						try {
							new MoistureBotConfig ().RemoveAllFavoriteUsers();
							Console.WriteLine("Favorite users succesfully removed.");
						} catch (Exception e){
							log.Error ("Error while removing favorite users", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					case "rooms":
						try {
							new MoistureBotConfig ().RemoveAllFavoriteChatRooms();
							Console.WriteLine("Favorite rooms succesfully removed.");
						} catch (Exception e){
							log.Error ("Error while removing favorite rooms", e);
							Console.WriteLine ("Error while adding favorite. See log files for more details.");
						}
						break;
					default:
						log.Info ("Failed to remove favorites. Check parameters and try again.");
						break;
					}
				} catch (Exception e){
					log.Error ("Failed to remove all favorites", e);
					Console.WriteLine ("Error while removing favorites. See log files for more details.");
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


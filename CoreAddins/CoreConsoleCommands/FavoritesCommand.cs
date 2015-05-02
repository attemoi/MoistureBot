using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Utils;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

    public class FavoritesCommand : IConsoleCommand
    {
      
        ILogger Logger;
        IConfig Config;

        [Provide]
        public FavoritesCommand(IContext context)
        {
            this.Logger = context.GetLogger(typeof(FavoritesCommand));
            this.Config = context.GetConfig();
        }

        private bool list;

        public OptionSet Options {
            get
            {
                return new OptionSet() { { "l|list", "list favourite rooms and users", 
                        v => list = v != null
                    },
                };
            }
        }

        public bool Execute(string[] args)
        {

            List<string> extra = Options.Parse(args);

            if (list || extra.Count == 0)
            {

                var favUsers = Config.GetFavoriteUsers();
                var favRooms = Config.GetFavoriteChatRooms();

                Console.WriteLine();
                Console.WriteLine("Favourite rooms:");
                Console.WriteLine();
                WriteDict(favRooms);
                Console.WriteLine();
                Console.WriteLine("Favourite users:");
                Console.WriteLine();
                WriteDict(favUsers);
                return false;
            }

            if (extra.Count < 2)
            {
                Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("favorites"));
                return false;
            }

            var operation = extra.First();
            var type = extra.ElementAt(1);

            switch (operation)
            {
                case "add":
                    try
                    {
                        if (extra.Count != 4)
                        {
                            Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("favorites"));
                            return false;
                        }
                        var key = extra.ElementAt(2);
                        var value = extra.ElementAt(3);
                        switch (type)
                        {
                            case "user":
                                try
                                {
                                    if (Config.AddFavoriteUser(key, UInt64.Parse(value)))
                                    {
                                        Console.WriteLine("Added user '" + key + "' to favorites.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to add favorite: Key '" + key + "' already exists.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Failed to add favorite user", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            case "room":
                                try
                                {
                                    if (Config.AddFavoriteChatRoom(key, UInt64.Parse(value)))
                                    {
                                        Console.WriteLine("Added room '" + key + "' to favorites");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to add favorite: Key '" + key + "' already exists.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Failed to add favorite room", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            default:
                                Console.WriteLine("Failed to add favorite. Check parameters and try again.");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to add favorite", e);
                        Console.WriteLine("Error while adding favorite. See log files for more details.");
                    }
			
                    break;
                case "remove":
                    try
                    {
                        if (extra.Count != 3)
                        {
                            Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("favorites"));
                            return false;
                        }
                        var key = extra.ElementAt(2);
                        switch (type)
                        {
                            case "user":
                                try
                                {
                                    if (Config.RemoveFavoriteUser(key))
                                    {
                                        Console.WriteLine("Removed user '" + key + "' from favorites");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Favorite user with key '" + key + "' doesn't exist.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Failed to remove favorite user", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            case "room":
                                try
                                {
                                    if (Config.RemoveFavoriteChatRoom(key))
                                    {
                                        Console.WriteLine("Removed room '" + key + "' from favorites");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Favorite room with key '" + key + "' doesn't exist.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Failed to remove favorite room", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            default:
                                Console.WriteLine("Failed to remove favorite. Check parameters and try again.");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to add favorite", e);
                        Console.WriteLine("Error while removing favorite. See log files for more details.");
                    }
                    break;
                case "remove-all":
                    try
                    {
                        if (extra.Count != 2)
                        {
                            Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("favorites"));
                            return false;
                        }
                        switch (type)
                        {
                            case "users":
                                try
                                {
                                    Config.RemoveAllFavoriteUsers();
                                    Console.WriteLine("Favorite users succesfully removed.");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Error while removing favorite users", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            case "rooms":
                                try
                                {
                                    Config.RemoveAllFavoriteChatRooms();
                                    Console.WriteLine("Favorite rooms succesfully removed.");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Error while removing favorite rooms", e);
                                    Console.WriteLine("Error while adding favorite. See log files for more details.");
                                }
                                break;
                            default:
                                Console.WriteLine("Failed to remove favorites. Check parameters and try again.");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to remove all favorites", e);
                        Console.WriteLine("Error while removing favorites. See log files for more details.");
                    }
                    break;
            }
					
            return false;
        }

        private void WriteDict(Dictionary<string, ulong> dict)
        {
            if (dict.Count == 0)
            {
                Console.WriteLine("  -- no favorites added --");
                return;
            }

            foreach (KeyValuePair<string, ulong> pair in dict)
            {
                Console.WriteLine("  {0} [{1}]", pair.Key, pair.Value);
            }
        }

    }
}


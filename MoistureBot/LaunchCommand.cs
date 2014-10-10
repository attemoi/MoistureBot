using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;
using Mono.Addins;

namespace MoistureBot
{

	/// <summary>
	/// The command used when first launching the program.
	/// </summary>
	public class LaunchCommand
	{

		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();

		private bool connect;
		private bool help;
		private bool joinFavs;
		private string user;
		private string pass;

		public OptionSet Options
		{
			get
			{
				return new OptionSet() { 
					{ "h|help", "Show this message." ,
						v => help = v != null },
					{ "c|connect", "Connect to Steam on launch." ,
						v => connect = v != null}, 
					{ "j|join-favorites", "Join favourite rooms on launch" ,
						v => joinFavs = v != null},
				};
			}
		}

		public bool Execute(string[] args)
		{
			Logger.Info("Executing command...");

			List<string> extra = Options.Parse(args);
				
			if (help)
			{
				Console.WriteLine();
				Console.WriteLine("  Extensible chat bot for Steam. A steam user with at least one game");
				Console.WriteLine("  is required to join chats or send messages (Steam requirement).");
				Console.WriteLine();
				Console.WriteLine("Usage: ");
				Console.WriteLine();
				Console.WriteLine("  MoistureBot [OPTIONS]+");
				Console.WriteLine();
				Console.WriteLine("Options: ");
				Console.WriteLine();
				Options.WriteOptionDescriptions(Console.Out);
				return true;
			}

			if (connect || joinFavs)
			{

				if (extra.Count > 0)
					user = extra.First();

				if (extra.Count > 1)
					pass = extra.ElementAt(1);
					
				if (user == null)
				{
					Console.WriteLine();
					Console.Write("username:");
					user = Console.ReadLine();
				}
				if (pass == null)
				{
					Console.Write("password:");
					pass = Console.ReadLine();
				}

				Console.WriteLine("Logging in as " + user + "...");
				Console.WriteLine("Type 'status' to see bot status.");
				Bot.Connect(user,pass);

			}

			if (joinFavs)
			{
				foreach (KeyValuePair<string, ulong> fav in new MoistureBotConfig().GetFavoriteChatRooms())
				{
					Console.WriteLine("Joining chat room '" + fav.Key + "' [" + fav.Value + "]");
					Bot.JoinChatRoom(fav.Value);
				}
			}

			return false;
		}

	}
}


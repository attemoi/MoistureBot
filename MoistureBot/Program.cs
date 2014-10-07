using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using Mono.Addins;
using System.Threading.Tasks;
using Mono.Options;
using System.Threading;
using MoistureBot.Commands;
using MoistureBot.Config;
using MoistureBot.Logging;



[assembly:AddinRoot ("MoistureBot", "1.0.0")]
[assembly:AddinAuthor ("Atte Moisio")]
[assembly:AddinDescription ("Extensible chat bot for Steam.")]
[assembly:AddinName("MoistureBot")]
[assembly:AddinUrl("") ]
[assembly:ImportAddinAssembly("MoistureBotLib.dll")]

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MoistureBot
{

	static class Program
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly IAddinLogger addinLogger = new AddinLogger();

		public static Core Bot { get; set; }

		static void Run(string[] args)
		{
				
			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;

			AddinManager.Initialize (".", ".", "./addins");
			AddinManager.Registry.Update ();

			Console.WriteLine ();
			// TODO: read version dynamically
			Console.WriteLine ("Moisturebot 1.0.0");

			var config = new MoistureBotConfig ();
			if (!config.ConfigExists())
				config.CreateConfig ();

			Bot = new Core ();

			var launchCmd = new LaunchCommand ();
			launchCmd.Args = args;
			if (launchCmd.Execute (Bot)) {
				return;
			}

			log.Debug ("Initializing addins");
			InitAddins ();

			HandleConsoleInput ();

		}

		public static void InitAddins() {
			foreach (IChatRoomAddin addin in AddinManager.GetExtensionObjects<IChatRoomAddin> ())
			{
				addin.Bot = Bot;
				addin.Logger = addinLogger;
			}
			foreach (IChatFriendAddin addin in AddinManager.GetExtensionObjects<IChatFriendAddin> ())
			{
				addin.Bot = Bot;
				addin.Logger = addinLogger;
			}
		}

		public static void HandleConsoleInput() {

			var exit = false;
			while (exit == false)
			{
				Console.WriteLine ();
				Console.WriteLine ("Enter command (type 'help' for a list of commands):");
				Console.Write (">>");
				var input = Console.ReadLine ();
				var command = CommandParser.Parse(input);
				if (command == null) {
					if (!String.IsNullOrWhiteSpace(input))
						log.Info ("Unknown command: '" + input + "'");
				} else {
					Console.Clear();
					exit = command.Execute(Bot);
				}
			}
		}

		static int Main( string[] args )
		{
		
			try
			{
				Run(args);
				return Environment.ExitCode;
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
				Trace.TraceError(e.ToString());

				return Environment.ExitCode != 0
					? Environment.ExitCode : 100;
			}

		}

		static void OnLoadError (object s, AddinErrorEventArgs args)
		{
			log.Error ("Failed to load addin", args.Exception);
		}

		static void OnLoad (object s, AddinEventArgs args)
		{
			log.Debug ("Add-in loaded: " + args.AddinId);
		}
			
	}
}

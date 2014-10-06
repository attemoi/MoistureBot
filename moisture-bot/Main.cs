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
using moisturebot.commands;
using moisturebot.config;

[assembly:AddinRoot ("moisturebot", "1.0")]
[assembly:ImportAddinAssembly("moisture-bot-lib.dll")]

namespace moisturebot
{

	static class Program
	{

		public static MoistureBot Bot { get; set; }

		static void Run(string[] args)
		{
				
			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;

			AddinManager.Initialize (".", ".", "./addins");
			AddinManager.Registry.Update ();

			Console.WriteLine ();
			Console.WriteLine ("Moisturebot 1.0");

			var config = new MoistureBotConfig ();
			if (!config.ConfigExists())
				config.CreateConfig ();

			Bot = new MoistureBot ();

			var launchCmd = new LaunchCommand ();
			launchCmd.Args = args;
			if (launchCmd.Execute (Bot)) {
				return;
			}

			InitAddins ();

			HandleConsoleInput ();

		}

		public static void InitAddins() {
			foreach (IChatRoomAddin addin in AddinManager.GetExtensionObjects<IChatRoomAddin> ())
			{
				addin.Bot = Bot;
			}
			foreach (IChatFriendAddin addin in AddinManager.GetExtensionObjects<IChatFriendAddin> ())
			{
				addin.Bot = Bot;
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
						Console.WriteLine ("Unknown command: '{0}'", input);
				} else {
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
			Console.WriteLine ("Failed to load addin: " + args.Message);
			Console.WriteLine (args.AddinId);
			Console.WriteLine (args.Exception);
		}

		static void OnLoad (object s, AddinEventArgs args)
		{
			// TODO: log
			// Console.WriteLine ("Add-in loaded: " + args.AddinId);
		}
			
	}
}

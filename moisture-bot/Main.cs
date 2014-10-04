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

[assembly:AddinRoot ("moisturebot", "1.0")]
[assembly:ImportAddinAssembly("moisture-bot-lib.dll")]

namespace moisturebot
{

	static class Program
	{

		public static MoistureBot Bot { get; set; }

		public static EventWaitHandle _connectionWaitHandle = new AutoResetEvent (false);

		static void Run(string[] args)
		{
			LaunchOptions options = new LaunchOptions ();
			options.Parse (args);

			if (options.ShowHelp) {
				options.WriteHelp ();
				return;
			}

			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;

			AddinManager.Initialize (".", ".", ".");
			AddinManager.Registry.Update ();

			Bot = new MoistureBot ();

			foreach (IChatAddin addin in AddinManager.GetExtensionObjects<IChatAddin> ())
			{
				addin.Bot = Bot;
			}

			if (options.AutoConnect) {
				if (String.IsNullOrEmpty (options.User) || String.IsNullOrEmpty (options.Pass)) {
					Console.WriteLine ("Autoconnect failed. Username and/or password not set.");
				} else {
					Bot.Connect (options.User, options.Pass);
				}

			}

			HandleConsoleInput ();

		}

		public static void HandleConsoleInput() {

			Console.WriteLine ("Type 'help' for a list of commands.");

			var exit = false;
			while (exit == false)
			{
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
			Console.WriteLine ("Add-in error: " + args.Message);
			Console.WriteLine (args.AddinId);
			Console.WriteLine (args.Exception);
		}

		static void OnLoad (object s, AddinEventArgs args)
		{
			Console.WriteLine ("Add-in loaded: " + args.AddinId);
		}
			
	}
}

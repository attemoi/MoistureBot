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

[assembly:AddinRoot ("moisturebot", "1.0")]
[assembly:ImportAddinAssembly("moisture-bot-lib.dll")]

namespace moisturebot
{

	static class Program
	{

		public static MoistureBot Bot { get; set; }

		public static EventWaitHandle _waitHandle = new AutoResetEvent (false);

		static void Run(string[] args)
		{
		
//			string user = null;
//			string pass = null;
			// TODO: remove default values
			string user = "Moisturebot";
			string pass = "JEApileet";
			ulong chatId = 103582791429523393;

			bool showHelp = false;

			var p = new OptionSet () {
				{ "h|help", "show this message and exit", 
					s => showHelp = s != null },
				{ "u=|user=", "Steam username", 
					s => user = s },
				{ "p=|password=", "Steam password", 
					s => pass = s }
			};

			List<string> extra;
			try {
				extra = p.Parse (args);
			}
			catch (OptionException e) {
				Console.Write ("moisture-bot: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try moisture-bot --help' for more information.");
				return;
			}

			if (showHelp) {
				ShowHelp (p);
				return;
			}

			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;

			AddinManager.Initialize (".", ".", ".");
			AddinManager.Registry.Update ();

			Bot = new MoistureBot ();

			if (user == null) 
			{
				Console.Write ("username: ");
				user = Console.ReadLine ();
			}
			if (pass == null) 
			{
				Console.Write ("password: ");
				pass = ConsoleUtils.ReadPassword ();
			}

			Bot.User = user;
			Bot.Pass = pass;
			Bot.ChatId = chatId;

			foreach (IGroupChatAddin addin in AddinManager.GetExtensionObjects<IGroupChatAddin> ())
			{
				addin.Initialize(Bot);
			}

			Bot.connect();

			Bot.Connected += new MoistureBot.ConnectedHandler (BotConnected);

			var botTask = Task.Factory.StartNew(() => { Bot.start(); });

			BlockConsoleThread ();

			Console.WriteLine ("Type 'help' for a list of commands.");
			Console.Write ("enter command:");
			string line;
			while ((line = Console.ReadLine ()) != null)
			{

				if (line.Equals ("exit") || line.Equals ("quit")) {
					Bot.stop ();
					Bot.disconnect ();
					return;
				} else if (line.Equals ("join chat")) {
					Console.WriteLine( "Joining chat room '{0}'...", 103582791429523393);
					Bot.JoinChatRoom (103582791429523393);
				} else {
					Console.WriteLine("Invalid command!");
					Console.WriteLine("Type 'help' for a list of commands.");
					// TODO print commands
				}
			}

		}

		public static void BlockConsoleThread() {
			_waitHandle.WaitOne ();
		}

		public static void ProceedConsoleThread() {
			_waitHandle.Set ();
		}

		public static void BotConnected(object sender)
		{
			Console.WriteLine( "Successfully logged on!" );
			ProceedConsoleThread ();
		}

		static void ShowHelp(OptionSet p) 
		{
			string executableName = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase);
			Console.WriteLine("Usage: moisture-bot [OPTIONS]+", executableName);
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions (Console.Out);

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
//			Console.WriteLine ("Add-in loaded: " + args.AddinId);
		}
			
	}
}

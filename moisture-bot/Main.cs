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
			Bot.ConnectionAttemptFinished += new MoistureBot.ConnectedHandler (BotConnected);

			foreach (IGroupChatAddin addin in AddinManager.GetExtensionObjects<IGroupChatAddin> ())
			{
				addin.Bot = Bot;
			}

			if (options.AutoConnect) {


				if (String.IsNullOrEmpty (options.User) || String.IsNullOrEmpty (options.Pass)) {
					Console.WriteLine ("Autoconnect failed. Username and/or password not set.");
				} else {
					Bot.Connect (options.User, options.Pass);
					BlockUntilConnected ();
				}

			}

			HandleConsoleInput ();

		}

		public static void HandleConsoleInput() {

			Console.WriteLine ("Type 'help' for a list of commands.");

			string line;
			var exit = false;
			while (exit == false)
			{

				Console.WriteLine();
				Console.WriteLine("Enter command (help to display help): "); 
				var command = CommandParser.Parse(Console.ReadLine());
				exit = command.Execute(Bot);

				// TODO: move these to parser implementation

//				line = Console.ReadLine ();

//				if (line == null || line.Equals ("/exit") || line.Equals ("/quit")) {
//					Bot.Disconnect ();
//					break;
//				} else if (line.Equals ("/connect")) {
//
//					string user, pass;
//
//					Console.Write ("username:");
//					user = Console.ReadLine ();
//					Console.Write ("password:");
//					pass = ConsoleUtils.ReadPassword ();
//
//					Bot.Connect(user, pass);
//					BlockUntilConnected ();
//
//				} else if (line.Equals ("/join")) {
//					Console.WriteLine( "Joining chat room '{0}'...", 103582791429523393);
//					Bot.JoinChatRoom (103582791429523393);
//				} else {
//					Console.WriteLine("Invalid command!");
//					Console.WriteLine("Type 'help' for a list of commands.");
//				}

			}

		}

		static void BlockUntilConnected() {
			_connectionWaitHandle.WaitOne ();
		}

		static void ProceedAfterConnected() {
			_connectionWaitHandle.Set ();
		}

		static void BotConnected(object sender)
		{
			ProceedAfterConnected ();
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

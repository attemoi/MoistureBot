using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Mono.Addins;
using System.Threading.Tasks;

[assembly:AddinRoot ("moisturebot", "1.0")]
[assembly:ImportAddinAssembly("moisture-bot-lib.dll")]

namespace moisturebot
{

	class Program
	{
		static void Run(string[] args)
		{

			string user = "Moisturebot";
			string pass = "JEApileet";
			ulong chatId = 103582791429523393;

			if (args.Length == 1)
			{
				if (args [0] == "-h" || args [0] == "--help") 
				{
					string executableName = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase);
					Console.WriteLine("Usage: {0} <username> <password> <chat room id>", executableName);
					return;
				}
			}

			if ( args.Length == 3 )
			{
				user = args [0];
				pass = args [1];
				try {
					chatId = Convert.ToUInt64(args [2]);
				} catch {
					Console.Error.WriteLine("Invalid chat room id");
				}
			}

			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;
			AddinManager.AddinUnloaded += OnUnload;

			AddinManager.Initialize (".", ".", ".");
			AddinManager.Registry.Update ();

			MoistureBot bot = new MoistureBot ();

			bot.User = user;
			bot.Pass = pass;
			bot.ChatId = chatId;

			foreach (IGroupChatAddin addin in AddinManager.GetExtensionObjects<IGroupChatAddin> ())
			{
				addin.Initialize (bot);
			}

			var botTask = Task.Factory.StartNew(() => { bot.connect(); });

			string line;
			while ((line = Console.ReadLine()) != null)
				Console.WriteLine(line);

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

		static void OnUnload (object s, AddinEventArgs args)
		{
			Console.WriteLine ("Add-in unloaded: " + args.AddinId);
		}

	}
}

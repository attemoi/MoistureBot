using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Mono.Addins;
using moisturebot.lib;

[assembly:AddinRoot ("moisturebot", "1.0")]
[assembly:ImportAddinAssembly ("moisture-bot-lib.dll")]

namespace moisturebot
{
	class Program
	{
		static void Run(string[] args)
		{

			MoistureBot bot = new MoistureBot ();
			bot.user = "Moisturebot";
			bot.pass = "JEApileet";
			bot.chatId = 103582791429523393;

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
				bot.user = args [0];
				bot.pass = args [1];
				try {
					bot.chatId = Convert.ToUInt64(args [2]);
				} catch {
					Console.Error.WriteLine("Invalid chat room id");
				}
			}

			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;
			AddinManager.AddinUnloaded += OnUnload;

			AddinManager.Initialize (".", ".", ".");
			AddinManager.Registry.Update ();

			bot.onChatMsgReceived += (object sender, ChatMsgEventArgs data) => 
			{
				foreach (IChatCommand cmd in AddinManager.GetExtensionObjects<IChatCommand> ())
				{
					string reply = cmd.ReplyToMessage(new ChatMessage(data.Message, data.Sender));
					if (reply != null)
					{
						bot.SendChatRoomMessage(reply);
					}
				}
			};

			bot.start();

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

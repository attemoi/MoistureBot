using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;

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

	}
}

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
using System.Linq;
using MoistureBot.ConsoleCommands;
using MoistureBot.ExtensionPoints;
using MoistureBot.ExtensionAttributes;

[assembly:AddinRoot("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Extensible chat bot for Steam.")]
[assembly:AddinName("MoistureBot")]
[assembly:AddinUrl("")]
[assembly:ImportAddinAssembly("MoistureBotLib.dll")]

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MoistureBot
{

	static class Program
	{

		public static ILogger Logger;
		public static IMoistureBot Bot;
		public static IConfig Config;

		static void Run(string[] args)
		{

			AddinManager.AddinLoadError += OnLoadError;
			AddinManager.AddinLoaded += OnLoad;
			AddinManager.AddinUnloaded += OnUnload;

			AddinManager.Initialize(".",".","./addins");
			AddinManager.Registry.Update();

			Logger = MoistureBotComponentProvider.GetLogger();
			Config = MoistureBotComponentProvider.GetConfig();
			Bot = MoistureBotComponentProvider.GetBot();

			Console.WriteLine();
			// TODO: read version dynamically
			Console.WriteLine("Moisturebot 1.0");

			if (!Config.ConfigExists())
				Config.CreateConfig();
				
			if (new LaunchCommand().Execute(args))
				return; 

			HandleConsoleInput();

			if (Bot != null)
				Bot.Terminate();

		}

		public static void HandleConsoleInput()
		{

			var exit = false;
			while (exit == false)
			{
				Console.WriteLine();
				Console.WriteLine("Enter command (type 'help' for a list of commands):");
				Console.Write(">>");

				var input = Console.ReadLine();
				if (!String.IsNullOrWhiteSpace(input))
				{

					Logger.Info("Parsing command '" + input + "'");

					// Parse your string and create Command object
					var commandParts = input.Split(' ').ToList();
					var commandName = commandParts[0];
					var args = commandParts.Skip(1).ToArray(); // the arguments is after the command

					IConsoleCommand command = null;

					ExtensionNodeList commands = AddinManager.GetExtensionNodes(typeof(IConsoleCommand));

					foreach (TypeExtensionNode<ConsoleCommandAttribute> node in commands)
					{
						var name = node.Data.Name;
						if (commandName.Equals(name))
						{
							command = (IConsoleCommand)node.CreateInstance();
							break;
						}
					}

					if (command != null)
					{
						exit = command.Execute(args);
					}
					else
					{
						Console.WriteLine("Unknown command: '" + input + "'");
					}

				}


			}
		}

		static int Main(string[] args)
		{
		
			try
			{
				Run(args);

				AddinManager.Shutdown();
				return Environment.ExitCode;
			}
			catch(Exception e)
			{
				Logger.Error("Program failure",e);
				Console.WriteLine(e);
				return Environment.ExitCode != 0
					? Environment.ExitCode : 100;
			}

		}

		static void OnLoadError(object s, AddinErrorEventArgs args)
		{
			Logger.Error("Failed to load addin",args.Exception);
		}

		static void OnUnload(object s, AddinEventArgs args)
		{
			Logger.Info("Add-in unloaded: " + args.AddinId);
		}

		static void OnLoad(object s, AddinEventArgs args)
		{
			Logger.Info("Add-in loaded: " + args.AddinId);
		}
			
	}
}

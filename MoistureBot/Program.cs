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
using System.Collections;

[assembly:AddinRoot("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Extensible chat bot for Steam.")]
[assembly:AddinName("MoistureBot")]
[assembly:AddinUrl("")]
[assembly:ImportAddinAssembly("MoistureBotLib.dll")]

namespace MoistureBot
{

	static class Program
	{

        public static ILogger Logger;
		public static IMoistureBot Bot;
		public static IConfig Config;

		public static AddinInvoker addinInvoker;

		static void Run(string[] args)
		{

            Logger = new MoistureBotFactory().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Config = new MoistureBotFactory().GetConfig();
            Bot = new MoistureBotFactory().GetBot();

			initializeAddinManager();

			addinInvoker = new AddinInvoker(Logger);

			Console.WriteLine();

			var version = AddinUtils.getAddinRoot("MoistureBot").Version;
			Console.WriteLine("Moisturebot " + version);

			if (!Config.ConfigExists())
				Config.CreateConfig();
				
			if (new LaunchCommand().Execute(args))
				return; 

			// Call addins extending IStartupCommand
			addinInvoker.invoke<IStartupCommand>((addin) => addin.ProgramStarted());

			HandleConsoleInput();

		}

		private static void initializeAddinManager() {

            AddinManager.AddinLoadError += (sender, args) => Logger.Error("Failed to load addin (" + args.Message + ")",args.Exception);
            AddinManager.AddinLoaded += (sender, args) => Logger.Info("Add-in loaded: " + args.AddinId);
			AddinManager.AddinUnloaded += (sender, args) => Logger.Info("Add-in unloaded: " + args.AddinId);

			AddinManager.Initialize(".",".","./addins");
			AddinManager.Registry.Update();

			// This needs to be called after Initialize
			AddinManager.ExtensionChanged += (sender, args) => Logger.Info("Extension changed (" + args.Path + ").");

		}

		private static void HandleConsoleInput()
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
                    var command = AddinManager
						.GetExtensionNodes<TypeExtensionNode<ConsoleCommandAttribute>>(typeof(IConsoleCommand))
						.FirstOrDefault((node) => node.Data.Name.Equals(commandName));

					if (command != null)
					{
						try {
							exit = ((IConsoleCommand)command.CreateInstance()).Execute(args);
						} catch (Exception e) {
							Console.WriteLine("Error while executing command!");
							Console.WriteLine("Message: " + e.Message);
							Logger.Error("Error while executing command.", e);
						}
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

				if (Bot != null)
					Bot.Terminate();
	
				AddinManager.Shutdown();
				return Environment.ExitCode;
			}
			catch(Exception e)
			{
				Logger.Error("Program failure", e);
				Console.WriteLine(e);
				return Environment.ExitCode != 0
					? Environment.ExitCode : 100;
			}

		}
			
	}
}

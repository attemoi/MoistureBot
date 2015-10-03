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
using MoistureBot;
using System.Collections;
using System.Runtime.Serialization;
using MoistureBot.Extensions;

//[assembly:AddinRoot("MoistureBot", "1.0")]
//[assembly:AddinAuthor("Atte Moisio")]
//[assembly:AddinDescription("Extensible chat bot for Steam.")]
//[assembly:AddinName("MoistureBot")]
//[assembly:AddinUrl("")]
//[assembly:ImportAddinAssembly("MoistureBotLib.dll")]

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MoistureBot
{

	static class Program
	{

        private static ILogger Logger;
        private static IConfig Config;
        private static IMoistureBot Bot;

        private static IContext Context;

		static void Run(string[] args)
		{

            Context = new MoistureBotContext();
            MoistureBotAddinManager.Context = Context;

            Logger = Context.GetLogger(typeof(Program));
            Config = Context.GetConfig();
            Bot = Context.GetBot();

			InitializeAddinManager();

			Console.WriteLine();
			var version = GetAddinRoot("MoistureBot").Version;
			Console.WriteLine("Moisturebot " + version);

			if (!Config.ConfigExists())
				Config.CreateConfig();
				
            Bot.Start();

            if (new LaunchCommand(Context).Execute(args))
				return; 

			// Call addins extending IStartupCommand
            Context.InvokeAddins<IStartupCommand>(addin => addin.ProgramStarted());

			HandleConsoleInput();

		}

        public static Addin GetAddinRoot(String name)
        {
            return AddinManager.Registry.GetAddinRoots().FirstOrDefault(addin => addin.Name.Equals(name));
        }

		private static void InitializeAddinManager() {

            AddinManager.AddinLoadError += (sender, args) => Logger.Error("Failed to load addin (" + args.Message + ")", args.Exception);
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

                    try
                    {
                        bool invoked = Context.InvokeAddins<IConsoleCommand, ConsoleCommandNode>("/MoistureBot/IConsoleCommand", 
                                node => node.Name.Equals(commandName),
                                addin => {
                                    Logger.Info("Executing " + addin.GetType().Name + ".");
                                    exit = addin.Execute(args);
                                });

                        if (!invoked)
                        {
                            Console.WriteLine("Unknown command: '" + input + "'");
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while executing command!");
                        Console.WriteLine("Message: " + e.Message);
                        Logger.Error("Error while executing console command.", e);
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

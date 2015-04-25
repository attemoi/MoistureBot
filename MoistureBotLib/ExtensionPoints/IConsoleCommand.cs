using System;
using System.Collections.ObjectModel;
using Mono.Options;
using MoistureBot;
using Mono.Addins;

namespace MoistureBot
{

    /// <summary>
    /// Extension point for a console command.
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IConsoleCommand", NodeType=typeof(ConsoleCommandNode))]
    public interface IConsoleCommand
    {

        OptionSet Options { get; }

        bool Execute(string[] args);

    }
}


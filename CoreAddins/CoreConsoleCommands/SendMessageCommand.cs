﻿using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Utils;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

    public class SendMessageCommand : IConsoleCommand
    {

        IMoistureBot Bot;
        IConfig Config;

        [Provide]
        public SendMessageCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Config = context.GetConfig();
        }

        public Boolean user;
        public Boolean room;

        public OptionSet Options {
            get { return new OptionSet() { }; }
        }

        public bool Execute(string[] args)
        {
            
            List<string> extra = Options.Parse(args);

            if (!Bot.IsConnected())
            {
                Console.WriteLine("Not connected to Steam.");
                return false;
            }

            if (extra.Count == 0)
            {
                Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("msg"));
                return false;
            }

            string target = extra.First();
            string inputId = extra.ElementAt(1);
            // get rest
            string message = string.Join(" ", extra.Skip(2)).Trim('\"');

            switch (target)
            {
                case "user":
                    var favUserId = Config.GetFavoriteUserId(inputId);
                    if (favUserId != null)
                    {
                        ulong favId;
                        try
                        {
                            favId = UInt64.Parse(favUserId);
                        }
                        catch
                        {
                            Console.WriteLine("Failed to send message: Invalid favorite id.");
                            return false;
                        }
                        Console.WriteLine("Sending chat message to '" + favId + "'...");
                        Bot.SendChatMessage(message, favId);
                        return false;
                    } 

                    ulong userId;
                    try
                    {
                        userId = UInt64.Parse(inputId);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to send message to user: Invalid id!");
                        return false;
                    }

                    Console.WriteLine("Sending chat message to user '" + inputId + "'...");
                    Bot.SendChatMessage(message, userId);

                    return false;
                case "room":

                    var favRoomId = Config.GetFavoriteChatRoomId(inputId);
                    if (favRoomId != null)
                    {
                        ulong favId;
                        try
                        {
                            favId = UInt64.Parse(favRoomId);
                        }
                        catch
                        {
                            Console.WriteLine("Failed to send message to favorite room: Invalid id!");
                            return false;
                        }
                        Console.WriteLine("Sending chat message to room '" + favId + "'...");
                        Bot.SendChatRoomMessage(message, favId);
                        return false;
                    } 

                    ulong roomId;
                    try
                    {
                        roomId = UInt64.Parse(inputId);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to send message to room: Invalid id!");
                        return false;
                    }

                    Console.WriteLine("Sending chat message to room '" + roomId + "'...");
                    Bot.SendChatRoomMessage(message, roomId);
                    return false;
                default:
                    Console.WriteLine(ConsoleMessage.InvalidParameters("msg"));
                    return false;
            }


        }

    }
}


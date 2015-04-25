﻿using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot;
using MoistureBot.Model;
using System.Threading;
using System.Collections.Generic;
using MoistureBot.Extensions;

namespace MoistureBot
{

    public class AutoReconnect : IStartupCommand
    {

        IMoistureBot Bot;
        ILogger Logger;
        IConfig Config;

        [Provide]
        public AutoReconnect(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.Config = context.GetConfig();
        }

        #pragma warning disable 414
        private Timer Timer;
        #pragma warning restore 414

        public void ProgramStarted()
        {
            Logger.Info("Creating timer for auto reconnect.");
            Timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private void TimerCallback(object state)
        {
			
            Logger.Info("Autoreconnect timer event fired.");

            if (String.IsNullOrEmpty(Bot.Username) || String.IsNullOrEmpty(Bot.Password))
            {
                Logger.Info("No username or password defined, cannot reconnect.");
                return;
            }

            if (!Bot.IsConnected())
            {
                Logger.Info("MoistureBot not connected, automatically reconnecting...");
                Bot.Connect(Bot.Username, Bot.Password);
            }
				
            int retries = 3;
            do
            {

                if (Bot.IsConnected())
                {
                    Logger.Info("Connected, joining favorite chat rooms...");
                    foreach (KeyValuePair<string, ulong> fav in Config.GetFavoriteChatRooms())
                    {
                        Bot.JoinChatRoom(fav.Value);
                    }
                    break;
                }

                // A little hacky but works :)
                Thread.Sleep(2000);
                retries--;
					
            } while(retries > 0);

        }

    }
}


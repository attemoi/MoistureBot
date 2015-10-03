using System;
using SteamKit2;
using MoistureBot.Model;

namespace MoistureBot
{
    public class MoistureBotConverter
    {
        public static OnlineStatus ToOnlineStatus(EPersonaState state)
        {
            switch (state)
            {
                case EPersonaState.Away:
                    return OnlineStatus.AWAY;
                case EPersonaState.Busy:
                    return OnlineStatus.BUSY;
                case EPersonaState.LookingToPlay:
                    return OnlineStatus.LOOKING_TO_PLAY;
                case EPersonaState.LookingToTrade:
                    return OnlineStatus.LOOKING_TO_TRADE;
                case EPersonaState.Offline:
                    return OnlineStatus.OFFLINE;
                case EPersonaState.Online:
                    return OnlineStatus.ONLINE;
                case EPersonaState.Snooze:
                    return OnlineStatus.SNOOZE;
                default:
                    return OnlineStatus.OFFLINE;
            }
        }

        public static EPersonaState ToEPersonaState(OnlineStatus status)
        {

            switch (status)
            {
                case OnlineStatus.AWAY:
                    return EPersonaState.Away;
                case OnlineStatus.BUSY:
                    return EPersonaState.Busy;
                case OnlineStatus.LOOKING_TO_PLAY:
                    return EPersonaState.LookingToPlay;
                case OnlineStatus.LOOKING_TO_TRADE:
                    return EPersonaState.LookingToTrade;
                case OnlineStatus.OFFLINE:
                    return EPersonaState.Offline;
                case OnlineStatus.ONLINE:
                    return EPersonaState.Online;
                case OnlineStatus.SNOOZE:
                    return EPersonaState.Snooze;
                default:
                    return EPersonaState.Online;

            }

        }
    }
}


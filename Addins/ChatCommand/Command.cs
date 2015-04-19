using System;
using System.Collections.Generic;

namespace MoistureBot
{
    public class Command
    {

        public enum CommandSource {
            GROUPCHAT,
            FRIEND
        }

        public String Name { get; set; }
        public List<String> Arguments { get; set; }
        public CommandSource Source { get; set; }

        public ulong SenderId { get; set; }
        public ulong ChatRoomId { get; set; }

        public Command()
        {
        }

        public bool HasArguments()
        {
            return Arguments.Count > 0;
        }

        public string FirstArgument
        {   
            get {
                if (!HasArguments())
                    return null;
            
                return Arguments[0];
            }
        }
            
    }
}


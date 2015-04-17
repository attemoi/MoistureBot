using System;

namespace MoistureBot
{
    public class Command
    {

        public enum CommandSource {
            GROUPCHAT,
            FRIEND
        }

        public String Name { get; set; }
        public String[] Arguments { get; set; }
        public CommandSource Source { get; set; }

        public ulong SenderId { get; set; }
        public ulong ChatRoomId { get; set; }

        public Command()
        {
        }

        public Command(String name, String[] arguments, CommandSource source, ulong chatterId, ulong chatRoomId)
        {
            Name = name;
            Arguments = arguments;
            Source = source;
            SenderId = chatterId;
            ChatRoomId = chatRoomId;
        }
            
    }
}


﻿using System;

namespace MoistureBot.Steam
{
    public class GroupChatMessage : FriendChatMessage
    {
        public ulong ChatId { get; internal set; }

        public GroupChatMessage(string message, ulong chatterId, ulong chatId) : base(message, chatterId)
        {
            this.ChatId = chatId;
        }
    }
}


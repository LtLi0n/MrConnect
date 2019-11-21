using System;
using System.Collections.Generic;
using System.Text;

namespace MrConnect.Shared
{
    public class InteractiveCommandContext
    {
        public ulong UserId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string Content { get; set; }

        public InteractiveCommandContext(ulong userId, ulong channelId, ulong messageId, string content)
        {
            UserId = userId;
            ChannelId = channelId;
            MessageId = messageId;
            Content = content;
        }
    }
}

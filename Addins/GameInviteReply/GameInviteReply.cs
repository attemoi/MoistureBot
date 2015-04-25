using System;
using MoistureBot;
using MoistureBot.Model;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using MoistureBot.Extensions;

namespace MoistureBot
{

    public class GameInviteReply
		: IReceiveGameLobbyInvites
    {

        static Random rnd = new Random();

        IMoistureBot Bot;

        [Provide]
        public GameInviteReply(IContext context)
        {
            this.Bot = context.GetBot();
            Replies = ReadReplies();  
        }

        IEnumerable<IEnumerable<String>> Replies;  

        private IEnumerable<IEnumerable<string>> ReadReplies()
        {

            //     GameInviteReply.xml example data:
            //
            //     <?xml version="1.0" encoding="UTF-8" ?>
            //     <replies>
            //     
            //         <reply>
            //             <message>Hello, Thank you for the invite!</message>
            //         	   <message>Unfortunately, I don't have a mouse or a keyboard.</message>
            //         </reply>
            //         
            //         <reply>
            //         	   <message>Hmm, let me think about this...</message>
            //         	   <message>...</message>
            //         	   <message>...</message>
            //         	   <message>No.</message>  
            //         </reply>
            //     
            //     </replies>

            XDocument xdoc = XDocument.Load("addins/GameInviteReply.xml");

            return xdoc.Root.Descendants("reply")
				.Select((e => from msg in e.Descendants("message")
                           select msg.Value));
        }

        public void InviteReceived(GameLobbyInvite invite)
        {
		
            // Pick random reply and send the messages
            int i = rnd.Next(Replies.Count());
            foreach (string message in Replies.ElementAt(i))
            {
                Bot.SendChatMessage(message, invite.InviterId);
                Thread.Sleep(1000);
            }
        }

    }
}
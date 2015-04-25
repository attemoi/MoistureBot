using System;
using MoistureBot;
using MoistureBot.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using MoistureBot.Extensions;

namespace MoistureBot
{
    public class JokeCommand : IChatCommand
    {

        static Random Rnd = new Random();

        IEnumerable<IEnumerable<String>> Jokes;

        IMoistureBot Bot;

        [Provide]
        public JokeCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Jokes = ReadJokes();
        }

        private IEnumerable<IEnumerable<string>> ReadJokes()
        {

            //     Jokes.xml example data:
            //
            //     <?xml version="1.0" encoding="UTF-8" ?>
            //     <jokes>
            //     
            //         <joke>
            //             <message>Hello, Thank you for the invite!</message>
            //             <message>Unfortunately, I don't have a mouse or a keyboard.</message>
            //         </joke>
            //         
            //         <joke>
            //             <message>Hmm, let me think about this...</message>
            //             <message>...</message>
            //             <message>...</message>
            //             <message>No.</message>  
            //         </joke>
            //     
            //     </jokes>

            XDocument xdoc = XDocument.Load("addins/Jokes.xml");

            return xdoc.Root.Descendants("joke")
                .Select((e => from msg in e.Descendants("message")
                    select msg.Value));
        }

        public void Execute(Command command)
        {
            // Pick random reply and send the messages
            int i = Rnd.Next(Jokes.Count());
            foreach (string message in Jokes.ElementAt(i))
            {
                if (command.Source == Command.CommandSource.FRIEND)
                    Bot.SendChatMessage(message, command.SenderId);
                else
                    Bot.SendChatRoomMessage(message, command.ChatRoomId);
                Thread.Sleep(2000);
            }
        }

        public void Help(Command command)
        {
            Bot.SendChatMessage("Tells a joke.", command.SenderId);
        }
           
       
    }
}


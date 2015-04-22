using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;
using MoistureBot;
using System.Linq;

namespace MoistureBot
{
    public class JokeCommand : IChatCommand
    {

        static Random rnd = new Random();

        IMoistureBot Bot = new MoistureBotFactory().GetBot();

        IEnumerable<IEnumerable<String>> jokes;

        public JokeCommand()
        {
            jokes = readJokes();
        }

        private IEnumerable<IEnumerable<string>> readJokes()
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
            int i = rnd.Next(jokes.Count());
            foreach (string message in jokes.ElementAt(i))
            {
                if (command.Source == Command.CommandSource.FRIEND)
                    Bot.SendChatMessage(message, command.SenderId);
                else
                    Bot.SendChatRoomMessage(message, command.ChatRoomId);
                Thread.Sleep(3000);
            }
        }

        public void Help(Command command)
        {
            Bot.SendChatMessage("Tells a joke.", command.SenderId);
        }
           
       
    }
}


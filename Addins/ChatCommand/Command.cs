using System;

namespace MoistureBot
{
    public class Command
    {

        public String Name { get; private set; }
        public String[] Arguments { get; private set; }

        public Command(String name, String[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }
            
    }
}


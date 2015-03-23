using System;

namespace MoistureBot.Steam
{
    public struct Favorite
    {
        public string Name, Id;

        public Favorite(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}


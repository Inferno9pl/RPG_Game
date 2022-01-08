using Game.Databases;
using System;

namespace Game.Objects
{
    public sealed class Item : Collectable
    {
        internal Item() { }
        public Item(string name, IGetObjectDataFromDatabase database)
        {
            if (database.GetItemData(name, out string[] result))
            {
                Name = name;
                Value = Int32.Parse(result[1]);
                Number = -1;

                var temp = result[2].Split(" ");
                Type = temp[0];

                //assign special item parameter
                if (temp[0].Equals("pożywienie") ||
                    temp[0].Equals("mikstura") ||
                    temp[0].Equals("mieszek"))
                {
                    Number = Int32.Parse(temp[1]);
                }
            }
        }

        public string Type { get; init; }
        public int Number { get; init; }

        public override int GetHashCode()
        {
            //return base.GetHashCode();
            return HashCode.Combine(Name);
        }
        public override bool Equals(object obj)
        {
            //return base.Equals(obj);
            return obj is Item item && Name == item.Name;
        }
        public override string ToString()
        {
            return Name.PadRight(25) + " | " + Value + " gold";
        }
    }
}
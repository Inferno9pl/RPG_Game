using Game.Databases;
using System;

namespace Game.Objects
{
    public sealed class Armor : Collectable
    {
        internal Armor() { }
        public Armor(string name, int meleeProtection, int arrowProtection, int fireProtection, int magicProtection, int value)
        {
            Name = name;
            MeleeProtection = meleeProtection;
            ArrowProtection = arrowProtection;
            FireProtection = fireProtection;
            MagicProtection = magicProtection;
            Value = value;
        }
        public Armor(string name, IGetObjectDataFromDatabase database)
        {
            if (database.GetArmorData(name, out string[] result))
            {
                Name = name;
                MeleeProtection = Int32.Parse(result[1]);
                ArrowProtection = Int32.Parse(result[2]);
                FireProtection = Int32.Parse(result[3]);
                MagicProtection = Int32.Parse(result[4]);
                Value = Int32.Parse(result[5]);
            }
            else
            {
                Name = "Znoszony strój";
                MeleeProtection = 2;
                ArrowProtection = 2;
                FireProtection = 0;
                MagicProtection = 0;
                Value = 10;
            }
        }

        public int MeleeProtection { get; init; }
        public int ArrowProtection { get; init; }
        public int FireProtection { get; init; }
        public int MagicProtection { get; init; }

        override public string ToString()
        {
            return Name.PadRight(10) + " | " + MeleeProtection.ToString().PadLeft(3) + " | " + ArrowProtection.ToString().PadLeft(3) + " | " + Value + " gold";
        }
    }
}



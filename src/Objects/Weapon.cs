using Game.Databases;
using System;

namespace Game.Objects
{
    public sealed class Weapon : Collectable
    {
        public Weapon(string name, char type, int damage, int requirement, char stat, int value)
        {
            Name = name;
            Type = type;
            Damage = damage;
            Requirement = requirement;
            Stat = stat;
            Value = value;
        }
        public Weapon(string name, IGetObjectDataFromDatabase db)
        {
            if (db.GetWeaponData(name, out string[] result))
            {
                Name = name;
                Type = result[1].ToCharArray()[0];
                Damage = Int32.Parse(result[2]);
                Requirement = Int32.Parse(result[3]);

                if (result[1].ToCharArray().Length == 2 || result[1].ToCharArray()[0] == 'Ł') Stat = 'Z';
                else Stat = 'S';
                Value = Int32.Parse(result[4]);
            }
            else
            {
                Name = "Zniszczona laga";
                Type = 'J';
                Damage = 5;
                Requirement = 0;
                Stat = 'S';
                Value = 1;
            }
        }

        public char Type { get; init; }
        public int Damage { get; init; }
        public int Requirement { get; init; }
        public char Stat { get; init; }

        public string GetDamageType()
        {
            if (Type == 'Ł' || Type == 'B')
                return "ranged";

            return "malee";
        }

        override public string ToString()
        {
            return Name.PadRight(10) + " | " + Type + " | " + Damage.ToString().PadLeft(3) + " dmg | " + Requirement.ToString().PadLeft(3) + " " + Stat + " | " + Value + " gold";
        }
    }
}

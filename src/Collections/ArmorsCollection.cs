using Game.Objects;
using System;
using System.Collections.Generic;

namespace Game.Collections
{
    public class ArmorsCollection : IArmorsCollection
    {
        private List<Armor> Armors { get; init; }

        public ArmorsCollection()
        {
            Armors = new();
        }

        public void Show(int equippedArmorIndex = -1)
        {
            Console.WriteLine("  Zbroje:");
            if (Armors.Count > 0)
            {
                for (int i = 0; i < Armors.Count; i++)
                {
                    if (i == equippedArmorIndex) Console.Write("[*] ");
                    else Console.Write("    ");
                    Console.WriteLine(Armors[i]);
                }
            }
            else Console.WriteLine("   <brak> ");
        }
        public Armor Get(int index)
        {
            return Armors[index];
        }
        public void Add(Armor armor)
        {
            Armors.Add(armor);
        }
        public void Remove(Armor armor, ref int equippedArmorIndex)
        {
            if (equippedArmorIndex != -1)
            {
                if (Armors[equippedArmorIndex] == armor)
                    equippedArmorIndex = -1;

                var index = FindIndex(armor);
                if (index < equippedArmorIndex)
                {
                    equippedArmorIndex -= 1;
                }
            }

            Armors.Remove(armor);
            Armors.TrimExcess();
        }
        public int Count()
        {
            return Armors.Count;
        }
        public bool Find(string name, out Armor armor)
        {
            if (name.Length >= 1)
                name = char.ToUpper(name[0]) + name[1..];
            for (int i = 0; i < Armors.Count; i++)
            {
                if (Armors[i].Name.Equals(name))
                {
                    armor = Armors[i];
                    return true;
                }

            }
            //Console.WriteLine("Nie znaleziono " + name);
            armor = null;
            return false;
        }
        public int FindIndex(Armor armor)
        {
            for (int i = 0; i < Armors.Count; i++)
                if (Armors[i].Equals(armor))
                    return i;

            return -1;
        }
    }
}

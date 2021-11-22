using Game.Objects;
using System;
using System.Collections.Generic;

namespace Game.Collections
{
    public class WeaponsCollection : IWeaponsCollection
    {
        private List<Weapon> Weapons { get; init; }

        public WeaponsCollection()
        {
            Weapons = new();
        }

        public void Show(int equippedWeaponIndex = -1)
        {
            Console.WriteLine("  Bronie:");
            if (Weapons.Count > 0)
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    if (i == equippedWeaponIndex) Console.Write("[*] ");
                    else Console.Write("    ");
                    Console.WriteLine(Weapons[i]);
                }
            }
            else Console.WriteLine("   <brak>");
        }
        public Weapon Get(int index)
        {
            return Weapons[index];
        }
        public void Add(Weapon weapon)
        {
            Weapons.Add(weapon);
        }
        public void Remove(Weapon weapon, ref int equippedWeaponIndex)
        {
            if (equippedWeaponIndex != -1)
            {
                if (Weapons[equippedWeaponIndex] == weapon)
                    equippedWeaponIndex = -1;

                var index = FindIndex(weapon);
                if (index < equippedWeaponIndex)
                {
                    equippedWeaponIndex -= 1;
                }
            }

            Weapons.Remove(weapon);
            Weapons.TrimExcess();
        }
        public int Count()
        {
            return Weapons.Count;
        }
        public bool Find(string name, out Weapon weapon)
        {
            if (name.Length >= 1)
                name = char.ToUpper(name[0]) + name[1..];
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (Weapons[i].Name.Equals(name))
                {
                    weapon = Weapons[i];
                    return true;
                }
            }
            //Console.WriteLine("Nie znaleziono " + name);
            weapon = null;
            return false;
        }
        public int FindIndex(Weapon weapon)
        {
            for (int i = 0; i < Weapons.Count; i++)
                if (Weapons[i].Equals(weapon))
                    return i;

            return -1;
        }
    }
}


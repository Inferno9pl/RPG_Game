using Game.Collections;
using Game.Objects;
using System;

namespace Game.Characters
{
    abstract public class Creature
    {
        public string Name { get; set; }
        public int Life { get; set; }
        public int MaxLife { get; set; }
        public int Level { get; set; }

        private int strenght;
        private int agility;
        public int Strenght { get => strenght; set => strenght = SetValue(value, 200); }
        public int Agility { get => agility; set => agility = SetValue(value, 200); }
        public Equipment Eq { get; init; }

        public void Equip(string element)
        {
            Eq.Armors.Find(element, out Armor armor);
            if (armor != null)
            {
                Equip(armor);
            }
            else
            {
                Eq.Weapons.Find(element, out Weapon weapon);
                if (weapon != null)
                    Equip(weapon);
                else
                    Console.WriteLine($"Nie znaleziono {element}");
            }
        }
        public void Equip(Armor armor)
        {
            int index = Eq.Armors.FindIndex(armor);
            if (index != -1)
            {
                Eq.EquippedArmorIndex = index;
                if (this.GetType().Equals(typeof(Knight)))
                {
                    Console.WriteLine($"  Założono {armor}");
                }
            }
            else Console.WriteLine($"  Nie posiadasz {armor}");
        }
        public void Equip(Weapon weapon)
        {
            int index = Eq.Weapons.FindIndex(weapon);

            if (index != -1)
            {
                switch (weapon.Stat)
                {
                    case 'S':
                        if (weapon.Requirement <= this.Strenght)
                        {
                            Eq.EquippedWeaponIndex = index;
                            if (this.GetType().Equals(typeof(Knight)))
                            {
                                Console.WriteLine("  Założono {0}", weapon.Name);
                            }
                        }
                        else
                            Console.WriteLine("  Niezałożono {0}. Brakuje {1} {2}", weapon.Name, weapon.Requirement - this.Strenght, weapon.Stat);
                        break;
                    case 'Z':
                        if (weapon.Requirement <= this.Agility)
                        {
                            Eq.EquippedWeaponIndex = index;
                            if (this.GetType().Equals(typeof(Knight)))
                            {
                                Console.WriteLine("  Założono {0}", weapon.Name);
                            }
                        }
                        else
                            Console.WriteLine("  Niezałożono {0}. Brakuje {1} {2}", weapon.Name, weapon.Requirement - this.Agility, weapon.Stat);
                        break;
                }
            }
            else Console.WriteLine($"  Nie posiadasz {weapon}");
        }

        public int GetProtection(string type)
        {
            if (Eq.EquippedArmorIndex == -1) return 0;

            int armor;
            armor = type switch
            {
                "malee" => Eq.Armors.Get(Eq.EquippedArmorIndex).MeleeProtection,
                "arrow" => Eq.Armors.Get(Eq.EquippedArmorIndex).ArrowProtection,
                "fire" => Eq.Armors.Get(Eq.EquippedArmorIndex).FireProtection,
                "magic" => Eq.Armors.Get(Eq.EquippedArmorIndex).MagicProtection,
                _ => 0,
            };
            return armor;
        }
        public void TransferEq(Creature enemy)
        {
            Console.WriteLine("   Zdobyto:");
            //drop armor (armors are not collectible)
            Eq.EquippedArmorIndex = -1;

            //drop weapons
            Eq.EquippedWeaponIndex = -1;
            for (int i = 0; i < Eq.Weapons.Count(); i++)
            {
                var weapon = Eq.Weapons.Get(i);
                Console.WriteLine(" >  " + weapon.Name);
                enemy.Eq.Weapons.Add(weapon);
            }

            //drop items
            for (int j = 0; j < Eq.Items.Count(); j++)
            {
                var item = Eq.Items.Get(j);
                Console.WriteLine(" >  " + item);
                enemy.Eq.Items.Add(item.Item, item.Quantity);
            }
        }

        private static int SetValue(int value, int maxValue)
        {
            if (value < 0) return 0;
            return value > maxValue ? maxValue : value;
        }

        override public string ToString()
        {
            return Name + " | Lvl: " + Level + " | " + Life + "/" + MaxLife + " | Siła: " + Strenght + " | Zręczność: " + Agility;
        }
    }
}

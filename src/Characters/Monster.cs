using Game.Collections;
using Game.Databases;
using Game.Objects;
using Game.Utils;
using System;

namespace Game.Characters
{
    public class Monster : Creature, IAttacker
    {
        private const int MinGold = 10;
        private const int MaxGold = 20;
        private const int DamageSplitter = 1;

        public Monster(string name, int level, int strenght, int agility, int life, int maxLife, int meleeProtection, int arrowProtection, int fireProtection, int magicProtection)
        {
            Name = name;
            Level = level;
            Strenght = strenght;
            Agility = agility;
            Life = life;
            MaxLife = maxLife;
            Eq = new Equipment();

            var armor = new Armor("Pancerz stwora",
                meleeProtection,
                arrowProtection,
                fireProtection,
                magicProtection,
                0
                );
            Eq.Armors.Add(armor);
            Equip(armor);
        }
        public Monster(string name, IGetObjectDataFromDatabase database)
        {
            if (database.GetMonsterData(name, out string[] result))
            {
                Name = name;
                Level = Int32.Parse(result[1]);
                Strenght = Int32.Parse(result[2]);
                Agility = Int32.Parse(result[3]);
                Life = Int32.Parse(result[4]);
                MaxLife = Int32.Parse(result[4]);
                Eq = new Equipment();

                var armor = new Armor("Pancerz stwora",
                    Int32.Parse(result[5]),
                    Int32.Parse(result[6]),
                    Int32.Parse(result[7]),
                    Int32.Parse(result[8]),
                    0
                    );
                Eq.Armors.Add(armor);
                Equip(armor);

                if (result.Length > 9)
                {
                    string[] itemsNames = result[9].Split(", ");

                    //Check if it is a weapon (weapon has first letter uppercase)
                    if (Char.IsUpper(itemsNames[0][0]))
                    {
                        var weapon = new Weapon(itemsNames[0], database);
                        Eq.Weapons.Add(weapon);
                        Equip(weapon);
                    }

                    //Add the rest of items
                    for (int i = 0 + Eq.Weapons.Count(); i < itemsNames.Length; i++)
                    {
                        if (itemsNames[i].Equals("złoto"))
                        {
                            Eq.Gold += Utilities.Rand(MinGold, MaxGold);
                        }
                        else
                        {
                            Item item = new(itemsNames[i], database);
                            Eq.Items.Add(item, 1);
                        }
                    }
                }
            }
            else
            {
                Name = "Zmarniona owca";
                Level = 1;
                Strenght = 1;
                Agility = 1;
                Life = 10;
                MaxLife = 10;
                Eq = new Equipment();

                var armor = new Armor("Pancerz stwora",
                    0,
                    0,
                    0,
                    0,
                    0
                    );
                Eq.Armors.Add(armor);
                Equip(armor);
            }
        }

        public void Attack(Creature enemy, string type = "malee")
        {
            if (enemy.Life > 0)
            {
                //check enemy armor against this type of damage
                var enemyArmor = enemy.GetProtection(type);

                int damage;
                //if unarmed attack
                if (Eq.EquippedWeaponIndex < 0)
                    damage = Strenght - enemyArmor;
                else
                    damage = (Strenght + Eq.Weapons.Get(Eq.EquippedWeaponIndex).Damage - enemyArmor - 10) / DamageSplitter;

                //            damage = damage > 5 ? damage : 5;
                damage = damage > 1 ? damage : 1;
                enemy.Life -= damage;

                Console.WriteLine("{0} oberwał za {1} (armor {2})", enemy.Name, damage, enemyArmor);
                //WinBattle(enemy);
            }
            else
            {
                Console.WriteLine("Gracz {0} jest już martwy!", enemy.Name);
            }
        }
        public bool WinBattle(Creature enemy)
        {
            if (enemy.Life <= 0)
            {
                enemy.Life = 0;
                Console.WriteLine($"   {enemy.Name} pokonany!");
                return true;
            }
            return false;
        }

        override public string ToString()
        {
            if (Eq.EquippedWeaponIndex == 0)
                return Name + " | Lvl: " + Level + " | " + Life + "/" + MaxLife + " | Siła: " + Strenght + " | Zręczność: " + Agility + " | " + Eq.Armors.Get(0) + " | " + Eq.Weapons.Get(0);

            if (Eq.EquippedArmorIndex == 0)
                return Name + " | Lvl: " + Level + " | " + Life + "/" + MaxLife + " | Siła: " + Strenght + " | Zręczność: " + Agility + " | " + Eq.Armors.Get(0).ToString();

            return Name + " | Lvl: " + Level + " | " + Life + "/" + MaxLife;
        }
    }
}

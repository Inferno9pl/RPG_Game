using Game.Characters;
using Game.Databases;
using Game.Objects;
using Game.Places;
using Game.Utils;
using System;
using System.Collections.Generic;


namespace Game.Main
{
    public class Program
    {
        static void Main()
        {
            Game();
        }

        static void Game()
        {
            var db = new DatabaseTxtFile();

            Knight Jack = new("Jack");
            var v = new Armor("Skórzany pancerz", db);
            //var v = new Armor(new string[] { "Plot armor", "70", "70", "0", "0", "0"});
            Jack.Eq.Armors.Add(v);
            Jack.Equip(v);
            Jack.Eq.Gold += 10000;

            var continueGame = true;
            while (continueGame)
            {
                for (int i = 0; i < 100; i++)
                {
                    Monster enemy = RandomEnemy(Jack.Level, db, 2);
                    Fight(Jack, enemy);
                    Jack.Life = Jack.MaxLife;
                }

                var s1 = new Market(ShopType.trader, Jack, db, 0.5);
                VisitShop(Jack, s1);

                Equip(Jack);
                Heal(Jack);

                if (Jack.Life <= 0)
                {
                    Console.WriteLine("Padł na polu bitwy..");
                    continueGame = false;
                }

            }
        }


        public static void Fight<T, P>(T one, P two)
            where T : Creature, IAttacker
            where P : Creature, IAttacker
        {
            Console.WriteLine($"Pojedynek pomiędzy {one.Name} - {two.Name}");

            int maxLoopRepeats = 100;
            int loopRepeats = 0;
            while (loopRepeats <= maxLoopRepeats)
            {
                one.Attack(two);
                if (two.Life <= 0)
                {
                    one.WinBattle(two);
                    return;
                }

                two.Attack(one);
                if (one.Life <= 0)
                {
                    two.WinBattle(one);
                    return;
                }

                loopRepeats++;
            }

            Console.WriteLine($"POKONANO {two.Name}\n");
        }
        public static Monster RandomEnemy(int level, DatabaseTxtFile db, int levelOffset = 5)
        {
            int maxLoops = 100;
            while (maxLoops >= 0)
            {
                int key = Utilities.Rand(level - levelOffset, level + levelOffset);
                var monsters = db.GetMonstersLevels();
                if (monsters.ContainsKey(key))
                {
                    int temp = Utilities.Rand(0, monsters[key].Count - 1);
                    return new Monster(monsters[key][temp], db);
                }
                maxLoops--;
            }
            throw new Exception();
        }

        public static void VisitShop(Knight knight, IShop shop)
        {
            bool continueShopping = true;
            while (continueShopping)
            {
                shop.ShowClientEq(knight);
                shop.ShowAssortment();
                Console.WriteLine();
                Console.WriteLine("Co chcesz zrobić? (buy, sell, exit, sell trash)");
                switch (Console.ReadLine())
                {
                    case "buy":
                        Console.WriteLine($"Posiadane złoto: {knight.Eq.Gold}");
                        Console.WriteLine("Jaki przedmiot chcesz kupić?");
                        var itemName = Console.ReadLine();
                        if (itemName == "")
                            break;

                        Console.WriteLine("Ile sztuk?");
                        var input = Console.ReadLine();
                        var count = input.Equals("") ? -1 : Int32.Parse(input);

                        shop.BuyFromShop(knight, itemName, count);
                        break;
                    case "sell":
                        itemName = Console.ReadLine();
                        if (itemName == "")
                            break;

                        Console.WriteLine("Ile sztuk?");
                        input = Console.ReadLine();
                        count = input.Equals("") ? -1 : Int32.Parse(input);

                        shop.SellToShop(knight, itemName, count);
                        break;
                    case "exit":
                        continueShopping = false;
                        break;
                    case "sell trash":
                        knight.SellAllTrash(shop);
                        break;
                    default:
                        Console.WriteLine("Niepoprawna komenda");
                        break;
                }
            }
        }
        public static void Heal(Knight knight)
        {
            var items = knight.Eq.Items;
            List<string> healItems = new();

            bool continueHealing = true;
            while (continueHealing)
            {
                healItems.Clear();
                healItems.TrimExcess();

                Console.WriteLine();
                Console.WriteLine($"{knight.Name} posiada {knight.Life} / {knight.MaxLife} HP");
                Console.WriteLine("Przedmioty leczące:");
                if (items.Count() > 0)
                {
                    for (int i = 0; i < items.Count(); i++)
                    {
                        var item = items.Get(i).Item;
                        var quantity = items.Get(i).Quantity;

                        if (item.Type.Equals("pożywienie") || item.Type.Equals("mikstura"))
                        {
                            healItems.Add(item.Name);
                            Console.WriteLine("    " + healItems.Count.ToString().PadLeft(2) + "." + item.Number.ToString().PadLeft(6) + " HP | " + item.Name.PadRight(24) + " | " + "x".PadLeft(3 - quantity.ToString().Length) + quantity + " | ");
                        }
                    }
                }
                else
                    Console.WriteLine("   <brak> ");

                Console.WriteLine();
                Console.WriteLine("Co chcesz użyć? (exit - wyjście)");
                var input = Console.ReadLine();
                if (Int32.TryParse(input, out int index))
                {
                    if (index > 0 && index <= healItems.Count)
                        input = healItems[index - 1];
                }

                if (input.Equals("exit"))
                {
                    continueHealing = false;
                }
                else if (items.Find(input, out ItemAndQuantity healItem))
                {
                    knight.Heal(healItem);
                }
                else
                {
                    Console.WriteLine($"Nie ma takiego przedmiotu {input}");
                }
            }
        }
        public static void Equip(Knight knight)
        {
            List<string> equipable = new();

            bool continueEquipping = true;
            while (continueEquipping)
            {
                equipable.Clear();
                equipable.TrimExcess();

                Console.WriteLine();
                Console.WriteLine($"## Ekwipunek {knight.Name} ##");
                Console.WriteLine("  Zbroje:");
                if (knight.Eq.Armors.Count() > 0)
                {
                    for (int i = 0; i < knight.Eq.Armors.Count(); i++)
                    {
                        var armor = knight.Eq.Armors.Get(i);
                        equipable.Add(armor.Name);

                        if (knight.Eq.EquippedArmorIndex == i)
                            Console.Write("[*] ");
                        else
                            Console.Write("    ");

                        Console.WriteLine(equipable.Count.ToString().PadLeft(2) + ".  "
                            + armor.Name.PadRight(20) + " | "
                            + armor.MeleeProtection.ToString().PadLeft(3) + " | "
                            + armor.ArrowProtection.ToString().PadLeft(3) + " | ");
                    }
                }
                else
                    Console.WriteLine("   <brak> ");

                Console.WriteLine("  Bronie:");
                if (knight.Eq.Weapons.Count() > 0)
                {
                    for (int i = 0; i < knight.Eq.Weapons.Count(); i++)
                    {
                        var weapon = knight.Eq.Weapons.Get(i);
                        equipable.Add(weapon.Name);

                        if (knight.Eq.EquippedWeaponIndex == i)
                            Console.Write("[*] ");
                        else
                            Console.Write("    ");

                        Console.WriteLine(equipable.Count.ToString().PadLeft(2) + ".  "
                            + weapon.Name.PadRight(20) + " | "
                            + weapon.Type + " | "
                            + weapon.Damage.ToString().PadLeft(3) + " dmg | "
                            + weapon.Requirement.ToString().PadLeft(3) + " "
                            + weapon.Stat + " | ");
                    }
                }
                else
                    Console.WriteLine("   <brak> ");


                Console.WriteLine();
                Console.WriteLine("Co wyposażyć? (exit - wyjście)");
                var input = Console.ReadLine();
                if (Int32.TryParse(input, out int index))
                {
                    if (index > 0 && index <= equipable.Count)
                        input = equipable[index - 1];
                }

                if (input.Equals("exit"))
                {
                    continueEquipping = false;
                }
                else
                {
                    knight.Equip(input);
                }
            }
        }

    }
}

//serializacja - zapis postepow
//miktury lecznicze
//miktury esencja lecznicza 50, ekstrakt 70, eliksir 100
//sprawdzenie podawania nazw niepoprawnych towarów i złej ilości albo znaków zamiast ilości
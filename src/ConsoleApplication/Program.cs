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


            //var v = new Armor(new string[] { "Plot armor", "70", "70", "0", "0", "0"});

            //Knight Jack = new("Jack");

            //var a = new Armor("Skórzany pancerz", db);
            //Jack.Eq.Armors.Add(a);
            //Jack.Equip(a);

            //var w = new Weapon("Laga", db);
            //Jack.Eq.Weapons.Add(w);
            //Jack.Equip(w);

            var Jack = db.LoadPlayer("Jack");
            Jack.Life = Jack.MaxLife;
            



            var continueGame = true;
            while (continueGame)
            {
                Jack.Life = (int)(0.9 * Jack.MaxLife);
                Fight(Jack, RandomEnemy(Jack.Level, db, 2));
                if (Jack.Life <= 0)
                {
                    Console.WriteLine("Padł na polu bitwy..");
                    break;
                }

                // loot tresures around and cook meat in camp
                Jack.Eq.Gold += Utilities.Rand(0, 10 * (Jack.Level + 1));
                CookMeat(Jack, db);

                db.SavePlayer(Jack);

                var merchant = RandomMerchant(Jack, db);
                var teacher = RandomTeacher();
                Menu(Jack, merchant, teacher);
            }
        }

        public static void Fight<T, P>(T one, P two)
            where T : Creature, IAttacker
            where P : Creature, IAttacker
        {
            Console.WriteLine($"Pojedynek pomiędzy {one.Name} - {two.Name}");

            int maxLoopRepeats = 1000;
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

            return RandomEnemy(level, db, levelOffset + 5);
            //throw new Exception($"Nie ma potwora o poziomie od {level - levelOffset} do {level + levelOffset}");
        }
        public static Teacher RandomTeacher()
        {
            return new Teacher(
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0));
        }
        public static Merchant RandomMerchant(Knight knight, DatabaseTxtFile db)
        {
            var temp = Utilities.Rand(1, 4);

            return temp switch
            {
                1 => new Merchant(ShopType.weaponry, knight, db, 1),
                2 => new Merchant(ShopType.armory, knight, db, 1),
                3 => new Merchant(ShopType.trader, knight, db, 1),
                _ => new Merchant(ShopType.all, knight, db, 1),
            };
        }


        public static void Menu(Knight knight, Merchant merchant, Teacher teacher)
        {
            bool continueMenu = true;

            while (continueMenu)
            {
                Console.WriteLine("");
                Console.WriteLine("########## MENU ##########");
                DrawExpProgress(knight);
                Console.WriteLine($"   1. Odwiedź sklep ( {merchant.Type} )");
                Console.WriteLine("   2. Załóż ekwipunek");
                Console.WriteLine($"   3. Odwieź nauczyciela ( masz {knight.SkillPoints} punktów nauki )");
                Console.WriteLine($"   4. Lecz rany ( masz {knight.Life} / {knight.MaxLife} hp )");
                Console.WriteLine("   x. Dalej ->");
                Console.WriteLine("");

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        VisitShop(knight, merchant);
                        break;
                    case '2':
                        Equip(knight);
                        break;
                    case '3':
                        VisitTeacher(knight, teacher);
                        break;
                    case '4':
                        Heal(knight);
                        break;
                    case 'x':
                        continueMenu = false;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void VisitShop(Knight knight, IMerchant shop)
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
        public static void VisitTeacher(Knight knight, Teacher teacher)
        {
            bool continueTeaching = true;
            while (continueTeaching)
            {
                Console.WriteLine();
                Console.WriteLine($"Posiadane punkty nauki - {knight.SkillPoints}");
                Console.WriteLine();
                knight.ShowSkill();
                Console.WriteLine();
                teacher.ShowWhatITeach();
                Console.WriteLine();
                Console.WriteLine("Czego się nauczyć? ( exit - wyjście ) ");

                switch (Console.ReadLine())
                {
                    case "s":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Strenght)} )");
                        var number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "strenght", number);
                        break;
                    case "a":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Agility)} )");
                        number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "agility", number);
                        break;
                    case "1":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Skill.OneHanded)} )");
                        number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "onehanded", number);
                        break;
                    case "2":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Skill.TwoHanded)} )");
                        number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "twohanded", number);
                        break;
                    case "b":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Skill.Bow)} )");
                        number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "bow", number);
                        break;
                    case "c":
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(knight.Skill.Crossbow)} )");
                        number = Int32.Parse(Console.ReadLine());
                        number = number > 5 ? 5 : number;
                        number = number < 0 ? 0 : number;
                        teacher.Teach(knight, "crossbow", number);
                        break;
                    case "exit":
                        continueTeaching = false;
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
                            Console.WriteLine("    " + healItems.Count.ToString().PadLeft(2) + "." + item.Number.ToString().PadLeft(6) + " HP | " + item.Name.PadRight(24) + " | " + "x".PadLeft(4 - quantity.ToString().Length) + quantity + " | ");
                        }
                    }
                }
                else
                    Console.WriteLine("   <brak> ");

                Console.WriteLine();
                Console.WriteLine("Co chcesz użyć? ( exit - wyjście )");
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

        public static void CookMeat(Knight knight, DatabaseTxtFile db)
        {
            if (knight.Eq.Items.Find("surowe mięso", out ItemAndQuantity rawMeat))
            {
                var cookedMeat = new Item("smażone mięso", db);
                knight.Eq.Items.Add(cookedMeat, rawMeat.Quantity);
                knight.Eq.Items.Remove(rawMeat.Item, rawMeat.Quantity);
            }
        }
        public static void DrawExpProgress(Knight knight)
        {
            var size = 30;

            var prevLvlExp = 250 * knight.Level * (knight.Level + 1);

            var difLevels = knight.GetNextLvlExp() - prevLvlExp;
            int step = difLevels / size;

            int lineCount = (knight.Exp - prevLvlExp) / step;

            Console.Write($" <{knight.Level}> [");

            for(int i = 0; i < size; i++)
            {
                if(i <= lineCount)
                    Console.Write("|");
                else
                    Console.Write(".");
            }

            Console.Write($"] <{knight.Level + 1}> ");
            Console.WriteLine();
        }
    }
}


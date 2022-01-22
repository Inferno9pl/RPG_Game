using Game.Characters;
using Game.Databases;
using Game.Objects;
using Game.Places;
using Game.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ConsoleApplication
{
    public class ConsoleGame<Base> where Base : IGetObjectDataFromDatabase, IGetSpecificListFromDatabase, ILoadAndSaveToDatabase
    {
        public Base Db { get; set; }
        public Knight Player { get; set; }

        public ConsoleGame(Base db)
        {
            Db = db;
            StartGame();
        }

        private void StartGame()
        {
            OpeningMenu(out bool continueGame);

            while (continueGame && Fight(Player, RandomEnemy(Player.Level, 2), out var log))
            {
                LootAndCookRawMeat();
                Db.SavePlayer(Player);
                Menu(log);
            }

            if (continueGame)
            {
                InputChar(out var _);
                StartGame();
            }

        }
        private void OpeningMenu(out bool continueGame)
        {
            continueGame = true;
            bool continueStartMenu = true;
            while (continueStartMenu)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine($" ########## MENU GŁÓWNE ##########");
                Console.WriteLine($"  > 1. Nowa gra");
                Console.WriteLine($"  > 2. Wczytaj zapis");
                Console.WriteLine($"  > 3. Wyjście");
                Console.WriteLine();

                if (InputChar(out var key))
                {
                    switch (key)
                    {
                        case '1':
                            continueStartMenu = false;
                            NewGame();
                            break;
                        case '2':
                            continueStartMenu = false;
                            LoadGame();
                            break;
                        case '3':
                            continueGame = false;
                            continueStartMenu = false;
                            break;
                    }
                }
            }
        }
        private void NewGame()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" ########## NOWA GRA ##########");
            Console.WriteLine("  Jak nazwiesz swoją postać?");

            if (InputString(out var inputString))
            {
                var name = inputString.Split(" ")[0];

                name = name.Length == 0 ? "Jack" : name;
                name = name.Length > 20 ? name.Substring(0, 20) : name;

                Player = new(name);
                Db.SavePlayer(Player);
                Menu();
            }
            else
            {
                StartGame();
            }
        }
        private void LoadGame()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" ########## WCZYTAJ GRĘ ##########");
            Console.WriteLine("  Jaką postać chcesz wczytać?");

            var saveGames = Db.GetSaveFiles();
            for (int i = 0; i < saveGames.Count; i++)
            {
                Console.WriteLine($"   {i + 1}. {saveGames[i]}");
            }

            Console.WriteLine();
            if (InputString(out var inputString))
            {
                if (Int32.TryParse(inputString, out int index))
                {
                    if (index > 0 && index <= saveGames.Count)
                        inputString = saveGames[index - 1];
                }

                if (Db.LoadPlayer(inputString, out Knight player))
                {
                    Player = player;
                    Menu();
                }
                else
                {
                    LoadGame();
                }
            }
            else
            {
                StartGame();
            }
        }

        private void Menu(string fightLog = "")
        {
            bool continueMenu = true;
            
            bool generateNPCs = false;
            var merchant = RandomMerchant();
            var teacher = RandomTeacher();

            while (continueMenu)
            {
                if(generateNPCs)
                {
                    merchant = RandomMerchant();
                    teacher = RandomTeacher();
                }

                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(" ########## MENU ##########");
                Console.WriteLine($"  > 1. Odwiedź sklep ( {merchant.Type} )");
                Console.WriteLine("  > 2. Załóż ekwipunek");
                Console.WriteLine($"  > 3. Odwieź nauczyciela ( {Player.SkillPoints} PN )");
                Console.WriteLine($"  > 4. Lecz rany ( {Player.Life} / {Player.MaxLife} HP )");
                Console.WriteLine("  > x. Dalej ->");
                Console.WriteLine();

                DrawPlayerStatsBars(40, 2);

                Console.WriteLine(" ########## PRZEBIEG POPRZEDNIEGO POJEDYNKU ##########");
                Console.WriteLine(fightLog);

                if (InputChar(out var key))
                {
                    switch (key)
                    {
                        case '1':
                            VisitShop(merchant);
                            break;
                        case '2':
                            Equip();
                            break;
                        case '3':
                            VisitTeacher(teacher);
                            break;
                        case '4':
                            Heal();
                            break;
                        case 'x':
                            generateNPCs = true;
                            continueMenu = false;
                            break;
                        default:
                            break;
                    }
                } else
                {
                    generateNPCs = false;
                }
            }
        }

        private static bool Fight<T, P>(T one, P two, out string log)
           where T : Creature, IAttacker
           where P : Creature, IAttacker
        {
            StringBuilder sb = new();
            sb.AppendFormat($"  Pojedynek pomiędzy {one.Name} - {two.Name}\n");

            int maxLoopRepeats = 1000;

            for (int i = 0; i < maxLoopRepeats; i++)
            {
                one.Attack(two, out var message);
                sb.AppendLine(message);
                if (two.Life <= 0)
                {
                    one.WinBattle(two);
                    sb.AppendFormat($"  Pokonano {two.Name}\n");
                    log = sb.ToString();
                    return true;
                }

                two.Attack(one, out message);
                sb.AppendLine(message);
                if (one.Life <= 0)
                {
                    two.WinBattle(one);
                    sb.AppendFormat($"  {one.Name} został pokonany\n");
                    sb.AppendFormat($"  Przeciwnikowy {two.Name} pozostało {two.Life} / {two.MaxLife} HP");
                    log = sb.ToString();

                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine($" ########## KONIEC GRY ##########");
                    Console.WriteLine(log);
                    return false;
                }
            }
            log = "Pojedynek nie zakończony";
            return false;
        }
        private void LootAndCookRawMeat()
        {
            Player.Eq.Gold += Utilities.Rand(0, 10 * (Player.Level + 1));

            if (Player.Eq.Items.Find("surowe mięso", out ItemAndQuantity rawMeat))
            {
                var cookedMeat = new Item("smażone mięso", Db);
                Player.Eq.Items.Add(cookedMeat, rawMeat.Quantity);
                Player.Eq.Items.Remove(rawMeat.Item, rawMeat.Quantity);
            }
        }

        private void VisitShop(IMerchant shop)
        {
            var prevMessage = "";

            bool continueShopping = true;
            while (continueShopping)
            {
                Console.Clear();
                Console.WriteLine(prevMessage);

                shop.ShowClientEq(Player);
                shop.ShowAssortment();

                Console.WriteLine();
                Console.WriteLine($" ########## KUPIEC ##########");
                Console.WriteLine("  > 1. Kup");
                Console.WriteLine("  > 2. Sprzedaj");
                Console.WriteLine("  > 3. Sprzedaj zbędne rzeczy");
                Console.WriteLine("  > x. Wyjście");
                Console.WriteLine();

                if (InputChar(out var inputString))
                {
                    switch (inputString)
                    {
                        case '1':
                            Console.WriteLine();
                            Console.WriteLine($" ########## KUP ##########");
                            Console.WriteLine($"  Jaki przedmiot chcesz kupić? ({Player.Eq.Gold} złota)");

                            if (InputString(out var ware))
                            {
                                if (shop.AskForQuantity(ware))
                                {
                                    Console.WriteLine("  Ile sztuk?");
                                    if (InputInt(out var count, false))
                                    {
                                        prevMessage = " Kupiono " + count + "x " + shop.BuyFromShop(Player, ware, count);
                                    }
                                }
                                else
                                {
                                    prevMessage = " Kupiono " + shop.BuyFromShop(Player, ware, 1);
                                }
                            }
                            break;
                        case '2':
                            Console.WriteLine();
                            Console.WriteLine($" ########## SPRZEDAJ ##########");
                            Console.WriteLine($"  Jaki przedmiot chcesz sprzedać? ({Player.Eq.Gold} złota)");

                            if (InputString(out ware))
                            {
                                if (shop.AskForQuantity(ware))
                                {
                                    Console.WriteLine("  Ile sztuk?");
                                    if (InputInt(out var count, false))
                                    {
                                        prevMessage = " Sprzedano " + count + "x " + shop.SellToShop(Player, ware, count);
                                    }
                                }
                                else
                                {
                                    prevMessage = " Sprzedano " + shop.SellToShop(Player, ware, 1);
                                }
                            }
                            break;
                        case '3':
                            var temp = Player.Eq.Gold;
                            SellAllTrash(shop);
                            prevMessage = " Sprzedano śmieci za " + (Player.Eq.Gold - temp) + " złota";
                            break;
                        case 'x':
                            continueShopping = false;
                            break;
                    }
                }
                else
                {
                    continueShopping = false;
                }
            }
        }
        private void SellAllTrash(IMerchant shop)
        {
            for (int i = Player.Eq.Items.Count() - 1; i >= 0; i--)
            {
                var item = Player.Eq.Items.Get(i).Item;
                if (!item.Type.Equals("pożywienie") && !item.Type.Equals("mikstura"))
                {
                    shop.SellToShop(Player, item.Name);
                }
            }

            Weapon equippedWeapon = null;
            if (Player.Eq.EquippedWeaponIndex != -1)
            {
                equippedWeapon = Player.Eq.Weapons.Get(Player.Eq.EquippedWeaponIndex);
            }

            for (int j = Player.Eq.Weapons.Count() - 1; j >= 0; j--)
            {
                var weapon = Player.Eq.Weapons.Get(j);
                if (weapon.Name.Equals("Laga") ||
                    weapon.Name.Equals("Zardzewiały krótki miecz") ||
                    weapon.Name.Equals("Zardzewiały miecz dwuręczny") ||
                    weapon.Name.Equals("Lekki orkowy topór") ||
                    weapon.Name.Equals("Jaszczurzy miecz") ||
                    weapon.Name.Equals("Średni orkowy topór") ||
                    weapon.Name.Equals("Miecz dwuręczny") ||
                    weapon.Name.Equals("Orkowy miecz wojenny"))
                {
                    //if weapon is equiped then didnt sell
                    if (j != Player.Eq.EquippedWeaponIndex)
                    {
                        shop.SellToShop(Player, weapon.Name);
                    }
                }
            }
            if (Player.Eq.EquippedWeaponIndex != -1)
            {
                Player.Equip(equippedWeapon.Name, out var _);
            }
        }
        private void Equip()
        {
            List<string> equipable = new();
            var prevMessage = "";

            bool continueEquipping = true;
            while (continueEquipping)
            {
                Console.Clear();
                Console.WriteLine(prevMessage);

                equipable.Clear();
                equipable.TrimExcess();

                Console.WriteLine();
                Console.WriteLine($" ## Ekwipunek {Player.Name} ##");
                Console.WriteLine("  Zbroje:");
                if (Player.Eq.Armors.Count() > 0)
                {
                    for (int i = 0; i < Player.Eq.Armors.Count(); i++)
                    {
                        var armor = Player.Eq.Armors.Get(i);
                        equipable.Add(armor.Name);

                        if (Player.Eq.EquippedArmorIndex == i)
                            Console.Write("   [*] ");
                        else
                            Console.Write("       ");

                        Console.WriteLine(equipable.Count.ToString().PadLeft(2) + ".  "
                            + armor.Name.PadRight(20) + " | "
                            + armor.MeleeProtection.ToString().PadLeft(3) + " | "
                            + armor.ArrowProtection.ToString().PadLeft(3) + " | ");
                    }
                }
                else
                    Console.WriteLine("      <brak> ");

                Console.WriteLine("  Bronie:");
                if (Player.Eq.Weapons.Count() > 0)
                {
                    for (int i = 0; i < Player.Eq.Weapons.Count(); i++)
                    {
                        var weapon = Player.Eq.Weapons.Get(i);
                        equipable.Add(weapon.Name);

                        if (Player.Eq.EquippedWeaponIndex == i)
                            Console.Write("   [*] ");
                        else
                            Console.Write("       ");

                        Console.WriteLine(equipable.Count.ToString().PadLeft(2) + ".  "
                            + weapon.Name.PadRight(20) + " | "
                            + weapon.Type + " | "
                            + weapon.Damage.ToString().PadLeft(3) + " dmg | "
                            + weapon.Requirement.ToString().PadLeft(3) + " "
                            + weapon.Stat + " | ");
                    }
                }
                else
                    Console.WriteLine("      <brak> ");

                Console.WriteLine();
                Console.WriteLine($" ########## WYPOSAŻANIE ##########");
                Console.WriteLine("  Co wyposażyć?");

                if (InputString(out var inputString))
                {
                    if (Int32.TryParse(inputString, out int index))
                    {
                        if (index > 0 && index <= equipable.Count)
                            inputString = equipable[index - 1];
                    }
                    Player.Equip(inputString, out prevMessage);
                }
                else
                {
                    continueEquipping = false;
                }
            }
        }
        private void VisitTeacher(ITeacher teacher)
        {
            List<string> skills = new();
            var prevMessage = "";

            bool continueTeaching = true;
            while (continueTeaching)
            {
                Console.Clear();
                Console.WriteLine(prevMessage);

                skills.Clear();
                skills.TrimExcess();

                Console.WriteLine();
                Console.WriteLine($" ## Umiejętności {Player.Name} ##");
                Player.ShowSkills();

                Console.WriteLine();
                Console.WriteLine($" ## Nauczyciel uczy ##");
                teacher.ShowWhatITeach();
                Console.WriteLine();

                Console.WriteLine($" ########## NAUCZYCIEL ##########");
                Console.WriteLine($"  Czego chcesz się nauczyć? ({Player.SkillPoints} PN)");

                if (InputChar(out var skill))
                {
                    if (teacher.GetStudentAbilityPoints(Player, skill.ToString()) != -1)
                    {
                        Console.WriteLine($"Ile punktów? ( 1 : {teacher.GetCostOfSkillImprovement(teacher.GetStudentAbilityPoints(Player, skill.ToString()))} )");

                        if (InputChar(out var numberChar))
                        {
                            if (Int32.TryParse(numberChar.ToString(), out var number))
                            {
                                number = number > 5 ? 5 : number;
                                number = number < 0 ? 0 : number;
                                teacher.Teach(Player, skill.ToString(), number, out prevMessage);
                            }
                            else
                            {
                                prevMessage = numberChar + " to nie liczba!";
                            }
                        }
                        else
                        {
                            prevMessage = "";
                        }
                    }
                    else
                    {
                        prevMessage = "Nie ma takiej umięjętności";
                    }
                }
                else
                {
                    prevMessage = "";
                    continueTeaching = false;
                }
            }
        }
        private void Heal()
        {
            var items = Player.Eq.Items;
            List<string> healItems = new();
            var prevMessage = "";

            bool continueHealing = true;
            while (continueHealing)
            {
                Console.Clear();
                Console.WriteLine(prevMessage);

                healItems.Clear();
                healItems.TrimExcess();

                Console.WriteLine();
                Console.WriteLine($" ## {Player.Name} posiada {Player.Life} / {Player.MaxLife} HP ##");
                Console.WriteLine("  Przedmioty leczące:");
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
                Console.WriteLine($" ########## LECZENIE ##########");
                Console.WriteLine("  Co chcesz użyć?");

                if (InputString(out var inputString))
                {
                    if (Int32.TryParse(inputString, out int index))
                    {
                        if (index > 0 && index <= healItems.Count)
                            inputString = healItems[index - 1];
                    }

                    if (items.Find(inputString, out ItemAndQuantity healItem))
                    {
                        prevMessage = "";
                        Player.Heal(healItem);
                    }
                    else
                    {
                        prevMessage = "Nie ma takiego przedmiotu " + inputString;
                    }
                }
                else
                {
                    continueHealing = false;
                }
            }
        }

        private Monster RandomEnemy(int level, int levelOffset = 5)
        {
            int maxLoops = 100;
            while (maxLoops >= 0)
            {
                int key = Utilities.Rand(level - levelOffset, level + levelOffset);
                var monsters = Db.GetMonstersLevels();
                if (monsters.ContainsKey(key))
                {
                    int temp = Utilities.Rand(0, monsters[key].Count - 1);
                    return new Monster(monsters[key][temp], Db);
                }
                maxLoops--;
            }

            return RandomEnemy(level, levelOffset + 5);
        }
        private static Teacher RandomTeacher()
        {
            return new Teacher(
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0),
                (Utilities.Rand(0, 100) % 2 > 0));
        }
        private Merchant RandomMerchant()
        {
            var temp = Utilities.Rand(1, 4);

            return temp switch
            {
                1 => new Merchant(ShopType.weaponry, Player, Db, 1),
                2 => new Merchant(ShopType.armory, Player, Db, 1),
                3 => new Merchant(ShopType.trader, Player, Db, 1),
                _ => new Merchant(ShopType.all, Player, Db, 1),
            };
        }

        private static bool InputString(out string inputString)
        {
            inputString = "";
            var continueCreatingString = true;
            (int l, int t) = Console.GetCursorPosition();
            Console.SetCursorPosition(l + 3, t);

            while (continueCreatingString)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    return false;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    continueCreatingString = false;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (inputString.Length > 0)
                    {
                        (int Left, int Top) = Console.GetCursorPosition();
                        Console.SetCursorPosition(Left - 1, Top);
                        Console.Write(" ");
                        Console.SetCursorPosition(Left - 1, Top);
                        inputString = inputString[0..^1];
                    }
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.Write(" ");
                    inputString += " ";
                }
                else
                {
                    Console.Write(key.KeyChar);
                    inputString += (key.KeyChar);
                }
            }
            return true;
        }
        private static bool InputChar(out char inputChar)
        {
            inputChar = 'x';

            (int l, int t) = Console.GetCursorPosition();
            Console.SetCursorPosition(l + 3, t);

            var key = Console.ReadKey(true);

            if (Char.IsLetterOrDigit(key.KeyChar))
            {
                inputChar = key.KeyChar;
                return true;
            }

            return false;

        }
        public static bool InputInt(out int inputInt, bool haveOneDigit = true)
        {
            inputInt = -1;

            (int l, int t) = Console.GetCursorPosition();
            Console.SetCursorPosition(l + 3, t);

            if (haveOneDigit)
            {
                var key = Console.ReadKey(true);

                if (Char.IsDigit(key.KeyChar))
                {
                    inputInt = Int32.Parse(key.KeyChar.ToString());
                    return true;
                }
                return false;
            }
            else
            {
                if (InputString(out var inputString))
                {
                    if (Int32.TryParse(inputString, out int number))
                    {
                        inputInt = number;
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        private void DrawPlayerStatsBars(int left, int top, int size = 30)
        {
            var lvlLength = Player.Level.ToString().Length;
            var hpLenght = Player.Life.ToString().Length;
            var dif = hpLenght - lvlLength;

            (int l, int t) = Console.GetCursorPosition();
            Console.SetCursorPosition(left, top);
            DrawExpBar(size);
            Console.SetCursorPosition(left + 1 - dif, top + 1);
            DrawHpBar(size);
            Console.SetCursorPosition(l, t);
        }
        private void DrawExpBar(int barSize = 30)
        {
            var prevLvlExp = 250 * Player.Level * (Player.Level + 1);
            int step = (Player.GetNextLvlExp() - prevLvlExp) / barSize;
            int lineCount = (Player.Exp - prevLvlExp) / step;

            Console.Write($" LVL <{Player.Level}> [");
            for (int i = 0; i < barSize; i++)
            {
                if (i <= lineCount)
                    Console.Write("█");
                else
                    Console.Write("_");
            }

            Console.Write($"] <{Player.Level + 1}> ");
            Console.WriteLine();
        }
        private void DrawHpBar(int barSize = 30)
        {
            double step = (double)Player.MaxLife / barSize;
            int lineCount = (int)(Player.Life / step);

            Console.Write($" HP <{Player.Life}> [");
            for (int i = 0; i < barSize; i++)
            {
                if (i <= lineCount)
                    Console.Write("█");
                else
                    Console.Write("_");
            }

            Console.Write($"] <{Player.MaxLife}> ");
            Console.WriteLine();
        }
    }
}
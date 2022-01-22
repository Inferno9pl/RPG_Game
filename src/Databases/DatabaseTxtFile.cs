using Game.Characters;
using Game.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Databases
{
    public class DatabaseTxtFile : IGetObjectDataFromDatabase, IGetSpecificListFromDatabase, ILoadAndSaveToDatabase
    {
        private string Path { init; get; }
        private int FilenameExtension { set; get; }
        private Dictionary<string, string[]> AllWeapons { set; get; }
        private Dictionary<string, string[]> AllArmors { set; get; }
        private Dictionary<string, string[]> AllItems { set; get; }
        private Dictionary<string, string[]> AllMonsters { set; get; }
        private SortedDictionary<int, List<string>> MonstersLevels { set; get; }

        public DatabaseTxtFile(string path, int additionalFilenameOffset = 2)
        {
            Path = path;

            if (!Directory.Exists(path + @"\Saves\"))
            {
                Directory.CreateDirectory(path + @"\Saves\");
            }

            FilenameExtension = 4 + additionalFilenameOffset;
            AllWeapons = new();
            AllArmors = new();
            AllItems = new();
            AllMonsters = new();
            MonstersLevels = new();
            Init();
        }

        private void Init()
        {
            LoadFile("ArmorsTP.txt");
            LoadFile("MeleeNK.txt");
            LoadFile("RangedNK.txt");
            LoadFile("MonstersNK.txt");
            LoadFile("ItemsNK.txt");
        }
        private void LoadFile(string file)
        {
            string fileLocation = Path + @"\" + file;
            if (File.Exists(fileLocation))
            {
                using StreamReader sr = File.OpenText(fileLocation);
                var s = "";

                var assetType = file[0..^FilenameExtension];
                //var assetType = file.Substring(0, file.Length - fileNameEndingLength);

                string[] splittedRow;
                while ((s = sr.ReadLine()) != null)
                {
                    splittedRow = s.Split("; ");
                    try
                    {
                        switch (assetType)
                        {
                            case "Melee":
                                AllWeapons.Add(splittedRow[0], splittedRow);
                                break;
                            case "Ranged":
                                AllWeapons.Add(splittedRow[0], splittedRow);
                                break;
                            case "Armors":
                                AllArmors.Add(splittedRow[0], splittedRow);
                                break;
                            case "Monsters":
                                AllMonsters.Add(splittedRow[0], splittedRow);
                                AddToSortedMonsters(splittedRow[0], splittedRow);
                                break;
                            case "Items":
                                AllItems.Add(splittedRow[0], splittedRow);
                                break;
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(splittedRow[0] + " - " + e.Message);
                    }
                }
            }
        }
        private void AddToSortedMonsters(string name, string[] splittedRow)
        {
            int lvl = Int32.Parse(splittedRow[1]);
            List<string> monsterNames;

            if (MonstersLevels.ContainsKey(lvl))
                monsterNames = MonstersLevels.GetValueOrDefault(lvl);
            else
                monsterNames = new();

            monsterNames.Add(name);
            MonstersLevels[lvl] = monsterNames;
        }

        public bool GetWeaponData(string name, out string[] result)
        {
            result = Array.Empty<string>();
            if (AllWeapons.ContainsKey(name))
            {
                result = AllWeapons.GetValueOrDefault(name);
                return true;
            }
            return false;
        }
        public bool GetArmorData(string name, out string[] result)
        {
            result = Array.Empty<string>();
            if (AllArmors.ContainsKey(name))
            {
                result = AllArmors.GetValueOrDefault(name);
                return true;
            }
            return false;
        }
        public bool GetItemData(string name, out string[] result)
        {
            result = Array.Empty<string>();
            if (AllItems.ContainsKey(name))
            {
                result = AllItems.GetValueOrDefault(name);
                return true;
            }
            return false;
        }
        public bool GetMonsterData(string name, out string[] result)
        {
            result = Array.Empty<string>();
            if (AllMonsters.ContainsKey(name))
            {
                result = AllMonsters.GetValueOrDefault(name);
                return true;
            }
            return false;
        }
        public SortedDictionary<int, List<string>> GetMonstersLevels()
        {
            return MonstersLevels;
        }

        public List<Item> GetItemsByType(string[] types)
        {
            List<Item> items = new();

            //IEnumerable<string> result =
            //    from item in AllItems
            //    where item.Value[2].Split(" ")[0].Equals(types[0])
            //    select item.Key;

            foreach (KeyValuePair<string, string[]> x in AllItems)
            {
                var item = new Item(x.Key, this);
                foreach (string type in types)
                {
                    if (item.Type.Equals(type))
                    {
                        items.Add(item);
                    }
                }
            }
            return items;
        }
        public List<Armor> GetArmorByValue(int value)
        {
            List<Armor> armors = new();
            foreach (KeyValuePair<string, string[]> x in AllArmors)
            {
                var armor = new Armor(x.Key, this);
                if ((armor.Value <= value))
                    armors.Add(armor);
            }
            return armors;
        }
        public List<Weapon> GetWeaponsByAttrybute(int maxStrenght, int maxAgility)
        {
            List<Weapon> weapons = new();
            foreach (KeyValuePair<string, string[]> x in AllWeapons)
            {
                var weapon = new Weapon(x.Key, this);
                if ((weapon.Stat == 'S') && (weapon.Requirement <= maxStrenght))
                    weapons.Add(weapon);
                else if ((weapon.Stat == 'Z') && (weapon.Requirement <= maxAgility))
                    weapons.Add(weapon);
            }
            return weapons;
        }

        public void SavePlayer(Knight player)
        {
            string fileLocation = Path + @"\Saves\" + player.Name + @".sv";
            FileStream file = File.Create(fileLocation);

            System.Xml.Serialization.XmlSerializer writer = new(typeof(Knight));
            writer.Serialize(file, player);

            Console.WriteLine("Zapisano " + player.Name);
            file.Close();
        }
        public bool LoadPlayer(string playerName, out Knight player)
        {
            if(SaveFileExist(playerName))
            {
                string fileLocation = Path + @"\Saves\" + playerName + @".sv";
                StreamReader file = new(fileLocation);

                System.Xml.Serialization.XmlSerializer reader = new(typeof(Knight));
                player = (Knight)reader.Deserialize(file);

                Console.WriteLine(" Wczytano " + player.Name);
                file.Close();
                return true;
            } else
            {
                player = null;
                Console.WriteLine($" Nie ma zapisu o nazwie {playerName} ");
                return false;
            }  
        }
        public List<string> GetSaveFiles()
        {
            List<string> playerName = new();
            try
            {
                string[] dirs = Directory.GetFiles(Path + @"\Saves\", "*.sv");
                foreach (string dir in dirs)
                {
                    var temp = dir.Split(@"\");
                    playerName.Add(temp[^1][0..^3]); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return playerName;
        }

        private bool SaveFileExist(string name)
        {
            var saveFiles = GetSaveFiles();
            
            foreach (string save in saveFiles)
            {
                if (save.Equals(name))
                    return true;
            }
            return false;
        }
    }
}
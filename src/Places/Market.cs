using Game.Characters;
using Game.Collections;
using Game.Databases;
using Game.Objects;
using Game.Utils;
using System;
using System.Collections.Generic;

namespace Game.Places
{
    public class Market : IShop
    {
        private const int SellPriceMultipierPercentage = 15;
        private const int AbilityOffset = 20;

        private Dictionary<int, string> ShopProductList { set; get; }
        private Dictionary<int, string> ClientProductList { set; get; }

        public Equipment Eq { get; set; }
        public double PriceMultiplier { get; set; }

        public Market(ShopType type, Creature client, IGetSpecificListFromDatabase database, double priceMult = 1.0f)
        {
            Eq = type switch
            {
                ShopType.armory => GenerateAssortment(5, 0, 0, client, database),
                ShopType.weaponry => GenerateAssortment(0, 5, 0, client, database),
                ShopType.trader => GenerateAssortment(0, 0, 10, client, database),
                ShopType.all => GenerateAssortment(2, 3, 10, client, database),
                _ => throw new ArgumentException("Niepoprawny typ sklepu"),
            };
            PriceMultiplier = priceMult;
            ShopProductList = new();
            ClientProductList = new();
        }

        public void BuyFromShop(Creature client, string name, int count = -1)
        {
            if (Int32.TryParse(name, out int index))
            {
                var temp = ShopProductList.GetValueOrDefault(index);
                if (temp != null)
                    name = temp;
            }

            if (Eq.Armors.Find(name, out Armor armor))
            {
                var cost = BuyValue(armor.Value);
                if (ClientHaveEnoughGold(client, cost))
                {
                    Eq.Gold += cost;
                    client.Eq.Gold -= cost;
                    client.Eq.Armors.Add(armor);
                    var equippedArmorIndex = Eq.EquippedArmorIndex;
                    Eq.Armors.Remove(armor, ref equippedArmorIndex);
                }
            }
            else if (Eq.Weapons.Find(name, out Weapon weapon))
            {
                var cost = BuyValue(weapon.Value);
                if (ClientHaveEnoughGold(client, cost))
                {
                    Eq.Gold += cost;
                    client.Eq.Gold -= cost;
                    client.Eq.Weapons.Add(weapon);
                    var equippedWeaponIndex = Eq.EquippedWeaponIndex;
                    Eq.Weapons.Remove(weapon, ref equippedWeaponIndex);
                }
            }
            else if (Eq.Items.Find(name, out ItemAndQuantity itemAndQuantity))
            {
                var item = itemAndQuantity.Item;

                var quantity = count;
                if (quantity == -1 || quantity > itemAndQuantity.Quantity)
                    quantity = itemAndQuantity.Quantity;

                var cost = BuyValue(item.Value, quantity);
                if (ClientHaveEnoughGold(client, cost))
                {
                    Eq.Gold += cost;
                    client.Eq.Gold -= cost;
                    client.Eq.Items.Add(item, quantity);
                    Eq.Items.Remove(item, quantity);
                }
            }
            else
            {
                Console.WriteLine($"Nie znaleziono {name}");
            }
        }
        public void SellToShop(Creature client, string name, int count = -1)
        {
            if (Int32.TryParse(name, out int index))
            {
                name = ClientProductList.GetValueOrDefault(index);
            }

            if (client.Eq.Armors.Find(name, out Armor armor))
            {
                var equippedArmorIndex = client.Eq.EquippedArmorIndex;
                client.Eq.Armors.Remove(armor, ref equippedArmorIndex);
                client.Eq.EquippedArmorIndex = equippedArmorIndex;
                Eq.Armors.Add(armor);
                client.Eq.Gold += SellValue(armor.Value);
            }
            else if (client.Eq.Weapons.Find(name, out Weapon weapon))
            {
                var equippedWeaponIndex = client.Eq.EquippedWeaponIndex;
                client.Eq.Weapons.Remove(weapon, ref equippedWeaponIndex);
                client.Eq.EquippedWeaponIndex = equippedWeaponIndex;
                Eq.Weapons.Add(weapon);
                client.Eq.Gold += SellValue(weapon.Value);
            }
            else if (client.Eq.Items.Find(name, out ItemAndQuantity itemAndQuantity))
            {
                var item = itemAndQuantity.Item;

                var quantity = count;
                if (quantity == -1 || quantity > itemAndQuantity.Quantity)
                    quantity = itemAndQuantity.Quantity;

                client.Eq.Items.Remove(item, quantity);
                Eq.Items.Add(item, quantity);
                client.Eq.Gold += SellValue(item.Value, quantity);
            }
            else
            {
                Console.WriteLine($"Nie znaleziono {name}");
            }
        }
        public void ShowAssortment()
        {
            ShopProductList.Clear();
            var index = 1;
            Console.WriteLine();
            Console.WriteLine("## Asortyment sklepikarza ##");
            if (Eq.Armors.Count() > 0)
            {
                Console.WriteLine("  Zbroje:");
                for (int i = 0; i < Eq.Armors.Count(); i++)
                {
                    var armor = Eq.Armors.Get(i);
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + BuyValue(armor.Value).ToString().PadLeft(6) + " gold | "
                        + armor.Name.PadRight(24) + " | "
                        + armor.MeleeProtection.ToString().PadLeft(3) + " | "
                        + armor.ArrowProtection.ToString().PadLeft(3) + " | ");

                    ShopProductList.Add(index++, armor.Name);
                }
            }
            if (Eq.Weapons.Count() > 0)
            {
                Console.WriteLine("  Bronie:");
                for (int i = 0; i < Eq.Weapons.Count(); i++)
                {
                    var weapon = Eq.Weapons.Get(i);
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + BuyValue(weapon.Value).ToString().PadLeft(6) + " gold | "
                        + weapon.Name.PadRight(24) + " | "
                        + weapon.Type + " | "
                        + weapon.Damage.ToString().PadLeft(3) + " dmg | "
                        + weapon.Requirement.ToString().PadLeft(3) + " "
                        + weapon.Stat + " | ");

                    ShopProductList.Add(index++, weapon.Name);
                }
            }

            if (Eq.Items.Count() > 0)
            {
                Console.WriteLine("  Przedmioty:");
                for (int i = 0; i < Eq.Items.Count(); i++)
                {
                    var quantity = Eq.Items.Get(i).Quantity;
                    var item = Eq.Items.Get(i).Item;
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + BuyValue(item.Value).ToString().PadLeft(6) + " gold | "
                        + item.Name.PadRight(24) + " | "
                        + "x".PadLeft(3 - quantity.ToString().Length) + quantity + " | ");

                    ShopProductList.Add(index++, item.Name);
                }
            }
        }
        public void ShowClientEq(Creature client)
        {
            ClientProductList.Clear();
            var index = 1;
            Console.WriteLine();
            Console.WriteLine($"## Ekwipunek {client.Name} ({client.Eq.Gold} złota) ##");
            if (client.Eq.Armors.Count() > 0)
            {
                Console.WriteLine("  Zbroje:");
                for (int i = 0; i < client.Eq.Armors.Count(); i++)
                {
                    var armor = client.Eq.Armors.Get(i);
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + SellValue(armor.Value).ToString().PadLeft(6) + " gold | "
                        + armor.Name.PadRight(20) + " | "
                        + armor.MeleeProtection.ToString().PadLeft(3) + " | "
                        + armor.ArrowProtection.ToString().PadLeft(3) + " | ");

                    ClientProductList.Add(index++, armor.Name);
                }
            }
            if (client.Eq.Weapons.Count() > 0)
            {
                Console.WriteLine("  Bronie:");
                for (int i = 0; i < client.Eq.Weapons.Count(); i++)
                {
                    var weapon = client.Eq.Weapons.Get(i);
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + SellValue(weapon.Value).ToString().PadLeft(6) + " gold | "
                        + weapon.Name.PadRight(20) + " | "
                        + weapon.Type + " | "
                        + weapon.Damage.ToString().PadLeft(3) + " dmg | "
                        + weapon.Requirement.ToString().PadLeft(3) + " "
                        + weapon.Stat + " | ");

                    ClientProductList.Add(index++, weapon.Name);
                }
            }

            if (client.Eq.Items.Count() > 0)
            {
                Console.WriteLine("  Przedmioty:");
                for (int i = 0; i < client.Eq.Items.Count(); i++)
                {
                    var quantity = client.Eq.Items.Get(i).Quantity;
                    var item = client.Eq.Items.Get(i).Item;
                    Console.WriteLine("    " + index.ToString().PadLeft(2) + "." + SellValue(item.Value).ToString().PadLeft(6) + " gold | "
                        + item.Name.PadRight(24) + " | "
                        + "x".PadLeft(3 - quantity.ToString().Length) + quantity + " | ");

                    ClientProductList.Add(index++, item.Name);
                }
            }
        }

        private static Equipment GenerateAssortment(int newArmors, int newWeapons, int newItems, Creature client, IGetSpecificListFromDatabase database)
        {
            var eq = new Equipment();

            if (newArmors > 0)
            {
                var armorList = database.GetArmorByValue(client.Level * 1000 + 1000);
                for (int i = 0; i < newArmors; i++)
                {
                    var randomIndex = Utilities.Rand(0, armorList.Count - 1);
                    eq.Armors.Add(armorList[randomIndex]);
                }
            }

            if (newWeapons > 0)
            {
                var weaponList = database.GetWeaponsByAttrybute(client.Strenght + AbilityOffset, client.Agility + AbilityOffset);
                for (int i = 0; i < newWeapons; i++)
                {
                    var randomIndex = Utilities.Rand(0, weaponList.Count - 1);
                    eq.Weapons.Add(weaponList[randomIndex]);
                }
            }

            if (newItems > 0)
            {
                var itemList = database.GetItemsByType(new string[] { "pożywienie", "mikstura" });
                for (int i = 0; i < newItems; i++)
                {
                    var randomIndex = Utilities.Rand(0, itemList.Count - 1);
                    var randomQuantity = Utilities.Rand(1, 5);
                    eq.Items.Add(itemList[randomIndex], randomQuantity);
                }
            }
            return eq;
        }
        private static bool ClientHaveEnoughGold(Creature client, int cost)
        {
            if (cost <= client.Eq.Gold)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Nie masz odpowiednich środków");
                return false;
            }

        }
        private static int SellValue(int value, int quantity = -1)
        {
            double sellValuefor1 = (value * SellPriceMultipierPercentage) / 100;
            var sellValuefor1Rounded = (int)Math.Round(sellValuefor1) > 0 ? (int)Math.Round(sellValuefor1) : 1;

            if (quantity == -1)
                return sellValuefor1Rounded;

            return sellValuefor1Rounded * quantity;
        }
        private int BuyValue(int value, int quantity = -1)
        {
            double cost = value * PriceMultiplier;
            var costRounded = (int)Math.Round(cost) > 0 ? (int)Math.Round(cost) : 1;

            if (quantity == -1)
                return costRounded;

            return costRounded * quantity;
        }
    }
}

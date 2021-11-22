using Game.Collections;
using Game.Objects;
using Game.Places;
using System;

namespace Game.Characters
{
    public sealed class Knight : Creature, IAttacker, IExperiencer
    {
        public int Exp { get; set; }
        public int SkillPoints { get; set; }
        public Skills Skill { get; init; }

        public Knight(string name)
        {
            Name = name;
            Level = 0;
            Exp = 0;
            Life = 40;
            MaxLife = 40;
            Strenght = 10;
            Agility = 10;
            Eq = new Equipment();

            Skill = new Skills(10, 10, 10, 10);
        }
        public Knight(string name, params int[] statistics)
        {
            Name = name;
            Eq = new Equipment();

            Life = 40 + 12 * statistics[0];
            MaxLife = 40 + 12 * statistics[0];
            Exp = 250 * statistics[0] * (statistics[0] + 1);

            Level = statistics[0];
            Strenght = statistics[1];
            Agility = statistics[2];
            Skill = new Skills(
                statistics[3],
                statistics[4],
                statistics[5],
                statistics[6]);
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
                    damage = (Strenght - 10 - enemyArmor) / 10;
                else
                {
                    //checking did it is a critical hit
                    Random rand = new();
                    var chance = (rand.Next() % 100) + 1;

                    bool critical = Eq.Weapons.Get(Eq.EquippedWeaponIndex).Type switch
                    {
                        'J' => Skill.OneHanded >= chance,
                        'D' => Skill.TwoHanded >= chance,
                        'B' => Skill.Bow >= chance,
                        'C' => Skill.Crossbow >= chance,
                        _ => false,
                    };

                    if (critical)
                        damage = this.Strenght + Eq.Weapons.Get(Eq.EquippedWeaponIndex).Damage - enemyArmor;
                    else
                        damage = (this.Strenght + Eq.Weapons.Get(Eq.EquippedWeaponIndex).Damage - enemyArmor - 10) / 10;
                }
                damage = damage > 5 ? damage : 5;
                enemy.Life -= damage;
                Console.WriteLine("{0} oberwał za {1} (armor {2})", enemy.Name, damage, enemyArmor);

                //WinBattle(enemy);

            }
            else
            {
                Console.WriteLine("{0} jest już martwy!", enemy.Name);
            }
        }
        public bool WinBattle(Creature enemy)
        {
            if (enemy.Life <= 0)
            {
                enemy.Life = 0;
                this.TakeExp(enemy.Level * 10);
                Console.WriteLine($"   {enemy.Name} pokonany!");
                Console.WriteLine($"   Doświadczenie: {Exp} / {GetNextLvlExp()}");
                enemy.TransferEq(this);
                return true;
            }
            return false;
        }

        public int GetNextLvlExp()
        {
            return 250 * (Level + 1) * (Level + 2);
        }
        public void TakeExp(int ex)
        {
            Exp += ex;
            if (Exp > GetNextLvlExp())
                LevelUp();
        }
        public void LevelUp()
        {
            Level += 1;
            MaxLife += 12;
            Life += 12;
            SkillPoints += 10;
            Console.WriteLine($"   Gratulacje, awansowałeś na {Level} poziom!");
        }

        public void ShowEq()
        {
            Console.WriteLine();
            Console.WriteLine($"## Ekwipunek ({Eq.Gold} złota) ##");
            Eq.Armors.Show();
            Eq.Weapons.Show();
            Eq.Items.Show();
        }
        public void SellAllTrash(IShop shop)
        {
            for (int i = Eq.Items.Count() - 1; i >= 0; i--)
            {
                var item = Eq.Items.Get(i).Item;
                if (!item.Type.Equals("pożywienie") && !item.Type.Equals("mikstura"))
                    shop.SellToShop(this, item.Name);
            }
        }
        public void Heal(ItemAndQuantity itemAndQuantity)
        {
            var item = itemAndQuantity.Item;

            if (item.Type.Equals("pożywienie") || item.Type.Equals("mikstura"))
            {
                Eq.Items.Remove(item, 1);
                Life += item.Number;
                Life = Life > MaxLife ? Life = MaxLife : Life;
            }
            else
                Console.WriteLine("Ten przedmiot nie regeneruje zdrowia!");

        }

        public override string ToString()
        {
            return Name + " | Lvl: " + Level + " | " + SkillPoints + "PN | " + Exp + "/" + GetNextLvlExp() + "exp. | " + Life + "/" + MaxLife + " | STR: " + Strenght + " | AGL: " + Agility;
        }
    }
}

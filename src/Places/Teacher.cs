using Game.Characters;
using Game.Utils;
using System;

namespace Game.Places
{
    public class Teacher : Creature, ITeacher
    {
        private Game.Objects.Skills MaxSkillLevels { get; set; }

        public Teacher(bool teachStrenght, bool teachAgility, bool teachOneHanded, bool teachTwoHanded, bool teachBow, bool teachCrossBow)
        {
            Name = "Hagen";
            Life = 500;
            MaxLife = 500;
            Level = 0;
            Strenght = teachStrenght ? Utilities.Rand(0, 21) * 10 : 0;
            Agility = teachAgility ? Utilities.Rand(0, 21) * 10 : 0;

            MaxSkillLevels = new(
                teachOneHanded ? Utilities.Rand(1, 10) * 10 : 0,
                teachTwoHanded ? Utilities.Rand(1, 10) * 10 : 0,
                teachBow ? Utilities.Rand(1, 10) * 10 : 0,
                teachCrossBow ? Utilities.Rand(1, 10) * 10 : 0);
        }

        public void ShowWhatITeach()
        {
            Console.WriteLine("\nUczę:");
            if (Strenght > 0) Console.WriteLine($" >> Siły (max {this.Strenght} pkt)");
            if (Agility > 0) Console.WriteLine($" >> Zręczności (max {this.Agility} pkt)");
            if (MaxSkillLevels.OneHanded > 0) Console.WriteLine($" >> Walki bronią jednoręczną (max {this.MaxSkillLevels.OneHanded} pkt)");
            if (MaxSkillLevels.TwoHanded > 0) Console.WriteLine($" >> Walki bronią dwuręczną (max {this.MaxSkillLevels.TwoHanded} pkt)");
            if (MaxSkillLevels.Bow > 0) Console.WriteLine($" >> Łucznictwa (max {this.MaxSkillLevels.Bow} pkt)");
            if (MaxSkillLevels.Crossbow > 0) Console.WriteLine($" >> Kusznictwa (max {this.MaxSkillLevels.Crossbow} pkt)");
        }
        public int GetCostOfSkillImprovement(int skillLevel)
        {
            if (skillLevel > 149) skillLevel = 149;
            return (skillLevel / 30) + 1;
        }

        public void Teach(Knight student, string skill, int skillImprovementPoints)
        {
            switch (skill)
            {
                case "strenght":
                    if (this.Strenght > 0)
                    {
                        if (this.Strenght >= student.Strenght + skillImprovementPoints)
                        {
                            student.Strenght = TncreaseSkillLevel(student, student.Strenght, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.Strenght} Siły!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Siły!");
                    break;

                case "agility":
                    if (this.Agility > 0)
                    {
                        if (this.Agility >= student.Agility + skillImprovementPoints)
                        {
                            student.Agility = TncreaseSkillLevel(student, student.Agility, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.Agility} Zręczności!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Zręczności!");
                    break;

                case "onehanded":
                    if (this.MaxSkillLevels.OneHanded > 0)
                    {
                        if (this.MaxSkillLevels.OneHanded >= student.Skill.OneHanded + skillImprovementPoints)
                        {
                            student.Skill.OneHanded = TncreaseSkillLevel(student, student.Skill.OneHanded, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.MaxSkillLevels.OneHanded} Walki bronią jednoręczną!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Walki bronią jednoręczną!");
                    break;

                case "twohanded":
                    if (this.MaxSkillLevels.TwoHanded > 0)
                    {
                        if (this.MaxSkillLevels.TwoHanded >= student.Skill.TwoHanded + skillImprovementPoints)
                        {
                            student.Skill.TwoHanded = TncreaseSkillLevel(student, student.Skill.TwoHanded, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.MaxSkillLevels.TwoHanded} Walki bronią dwuręczną!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Walki bronią dwuręczną!");
                    break;

                case "bow":
                    if (this.MaxSkillLevels.Bow > 0)
                    {
                        if (this.MaxSkillLevels.Bow >= student.Skill.Bow + skillImprovementPoints)
                        {
                            student.Skill.Bow = TncreaseSkillLevel(student, student.Skill.Bow, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.MaxSkillLevels.Bow} Łucznictwa!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Łucznictwa!");
                    break;

                case "crossbow":
                    if (this.MaxSkillLevels.Crossbow > 0)
                    {
                        if (this.MaxSkillLevels.Crossbow >= student.Skill.Crossbow + skillImprovementPoints)
                        {
                            student.Skill.Crossbow = TncreaseSkillLevel(student, student.Skill.Crossbow, skillImprovementPoints);
                        }
                        else
                            Console.WriteLine($"Ten nauczyciel nauczy Cię maksymalnie {this.MaxSkillLevels.Crossbow} Kusznictwa!");
                    }
                    else
                        Console.WriteLine("Ten nauczyciel nie uczy Kusznictwa!");
                    break;

                default:
                    Console.WriteLine("Nie znaleziono podanej umiejętności");
                    return;
            }
        }
        private int TncreaseSkillLevel(Knight student, int actualSkillLevel, int skillImprovementPoints)
        {
            var necessarySkillPoints = GetCostOfSkillImprovement(actualSkillLevel) * skillImprovementPoints;

            if (necessarySkillPoints <= student.SkillPoints)
            {
                student.SkillPoints -= necessarySkillPoints;
                actualSkillLevel += skillImprovementPoints;
            }
            else
            {
                var missedPoints = necessarySkillPoints - student.SkillPoints;
                Console.WriteLine($"Nie masz wystarczającej ilości punktów nauki. Brakuje {missedPoints} punktów");
            }
            return actualSkillLevel;
        }
    }
}

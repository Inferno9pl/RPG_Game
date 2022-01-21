using Game.Characters;
using Game.Objects;
using Game.Utils;
using System;
using System.Collections.Generic;

namespace Game.Places
{
    public class Teacher : Creature, ITeacher
    {
        private Skills MaxSkillLevels { get; set; }

        private Dictionary<int, string> TeacherSkills { set; get; }

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

            TeacherSkills = new();
        }

        public void ShowWhatITeach()
        {
            TeacherSkills.Clear();
            var index = 1;

            if (Strenght > 0)
            {
                Console.WriteLine($"   {index}. Siła (max {this.Strenght} pkt)");
                TeacherSkills.Add(index++, "strenght");
            }
            if (Agility > 0)
            {
                Console.WriteLine($"   {index}. Zręczność (max {this.Agility} pkt)");
                TeacherSkills.Add(index++, "agility");
            }
            if (MaxSkillLevels.OneHanded > 0)
            {
                Console.WriteLine($"   {index}. Walka bronią jednoręczną (max {this.MaxSkillLevels.OneHanded} pkt)");
                TeacherSkills.Add(index++, "onehanded");
            }
            if (MaxSkillLevels.TwoHanded > 0)
            {
                Console.WriteLine($"   {index}. Walka bronią dwuręczną (max {this.MaxSkillLevels.TwoHanded} pkt)");
                TeacherSkills.Add(index++, "twohanded");
            }
            if (MaxSkillLevels.Bow > 0)
            {
                Console.WriteLine($"   {index}. Łucznictwo (max {this.MaxSkillLevels.Bow} pkt)");
                TeacherSkills.Add(index++, "bow");
            }
            if (MaxSkillLevels.Crossbow > 0)
            {
                Console.WriteLine($"   {index}. Kusznictwo (max {this.MaxSkillLevels.Crossbow} pkt)");
                TeacherSkills.Add(index++, "crossbow");
            }
        }
        public int GetCostOfSkillImprovement(int skillLevel)
        {
            if (skillLevel > 149) skillLevel = 149;
            return (skillLevel / 30) + 1;
        }
        public int GetStudentAbilityPoints(Knight student, string ability)
        {
            if (Int32.TryParse(ability, out int index))
            {
                var temp = TeacherSkills.GetValueOrDefault(index);
                if (temp != null)
                    ability = temp;
            }

            return ability switch
            {
                "strenght" => student.Strenght,
                "agility" => student.Agility,
                "onehanded" => student.Skill.OneHanded,
                "twohanded" => student.Skill.TwoHanded,
                "bow" => student.Skill.Bow,
                "crossbow" => student.Skill.Crossbow,
                _ => -1,//Console.WriteLine($" Nie ma takiej umiejętności - {ability}");
            };
        }
        public bool Teach(Knight student, string skill, int skillImprovementPoints, out string message)
        {
            if (Int32.TryParse(skill, out int index))
            {
                var temp = TeacherSkills.GetValueOrDefault(index);
                if (temp != null)
                    skill = temp;
            }

            switch (skill)
            {
                case "strenght":
                    if (this.Strenght > 0)
                    {
                        if (this.Strenght >= student.Strenght + skillImprovementPoints)
                        {
                            int strenghtSkill = student.Strenght;
                            if (TncreaseSkillLevel(student, ref strenghtSkill, skillImprovementPoints, out message))
                            {
                                message += " Siły";
                                student.Strenght = strenghtSkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.Strenght + " Siły!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Siły!";
                    break;

                case "agility":
                    if (this.Agility > 0)
                    {
                        if (this.Agility >= student.Agility + skillImprovementPoints)
                        {
                            var agilitySkill = student.Agility;
                            if (TncreaseSkillLevel(student, ref agilitySkill, skillImprovementPoints, out message))
                            {
                                message += " Zręczności";
                                student.Agility = agilitySkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.Agility + " Zręczności!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Zręczności!";
                    break;

                case "onehanded":
                    if (this.MaxSkillLevels.OneHanded > 0)
                    {
                        if (this.MaxSkillLevels.OneHanded >= student.Skill.OneHanded + skillImprovementPoints)
                        {
                            var oneHandedSkill = student.Skill.OneHanded;
                            if (TncreaseSkillLevel(student, ref oneHandedSkill, skillImprovementPoints, out message))
                            {
                                message += " Walki bronią jednoręczną";
                                student.Skill.OneHanded = oneHandedSkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.MaxSkillLevels.OneHanded + " Walki bronią jednoręczną!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Walki bronią jednoręczną!";
                    break;

                case "twohanded":
                    if (this.MaxSkillLevels.TwoHanded > 0)
                    {
                        if (this.MaxSkillLevels.TwoHanded >= student.Skill.TwoHanded + skillImprovementPoints)
                        {
                            var twoHandedSkill = student.Skill.TwoHanded;
                            if (TncreaseSkillLevel(student, ref twoHandedSkill, skillImprovementPoints, out message))
                            {
                                message += " Walki bronią dwuręczną";
                                student.Skill.TwoHanded = twoHandedSkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.MaxSkillLevels.TwoHanded + " Walki bronią dwuręczną!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Walki bronią dwuręczną!";
                    break;

                case "bow":
                    if (this.MaxSkillLevels.Bow > 0)
                    {
                        if (this.MaxSkillLevels.Bow >= student.Skill.Bow + skillImprovementPoints)
                        {
                            var bowSkill = student.Skill.Bow;
                            if (TncreaseSkillLevel(student, ref bowSkill, skillImprovementPoints, out message))
                            {
                                message += " Łucznictwa";
                                student.Skill.Bow = bowSkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.MaxSkillLevels.Bow + " Łucznictwa!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Łucznictwa!";
                    break;

                case "crossbow":
                    if (this.MaxSkillLevels.Crossbow > 0)
                    {
                        if (this.MaxSkillLevels.Crossbow >= student.Skill.Crossbow + skillImprovementPoints)
                        {
                            var crossbowSkill = student.Skill.Crossbow;
                            if (TncreaseSkillLevel(student, ref crossbowSkill, skillImprovementPoints, out message))
                            {
                                message += " Kusznictwo";
                                student.Skill.Crossbow = crossbowSkill;
                                return true;
                            }
                        }
                        else
                            message = "Ten nauczyciel nauczy Cię maksymalnie " + this.MaxSkillLevels.Crossbow + " Kusznictwa!";
                    }
                    else
                        message = "Ten nauczyciel nie uczy Kusznictwa!";
                    break;

                default:
                    message = "Nie znaleziono podanej umiejętności";
                    break;
            }
            return false;
        }

        private bool TncreaseSkillLevel(Knight student, ref int actualSkillLevel, int skillImprovementPoints, out string message)
        {
            var necessarySkillPoints = GetCostOfSkillImprovement(actualSkillLevel) * skillImprovementPoints;

            if (necessarySkillPoints <= student.SkillPoints)
            {
                student.SkillPoints -= necessarySkillPoints;
                actualSkillLevel += skillImprovementPoints;
                message = "Nauczono +" + skillImprovementPoints;
                return true;
            }
            else
            {
                var missedPoints = necessarySkillPoints - student.SkillPoints;
                message = "Nie masz wystarczającej ilości punktów nauki. Brakuje " + missedPoints + " punktów";
                return false;
            }
        }
    }
}

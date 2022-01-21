using Game.Characters;

namespace Game.Places
{
    public interface ITeacher
    {
        void ShowWhatITeach();
        int GetCostOfSkillImprovement(int skillLevel);
        int GetStudentAbilityPoints(Knight student, string ability);
        public bool Teach(Knight student, string skill, int skillImprovementPoints, out string message);
    }
}
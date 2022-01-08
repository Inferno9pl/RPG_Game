using Game.Characters;

namespace Game.Places
{
    public interface ITeacher
    {
        void ShowWhatITeach();
        int GetCostOfSkillImprovement(int skillLevel);
        public void Teach(Knight student, string skill, int skillImprovementPoints);
    }
}
namespace Game.Characters
{
    public interface IExperiencer
    {
        void LevelUp();
        void TakeExp(int ex);
        int GetNextLvlExp();
    }
}

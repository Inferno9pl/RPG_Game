namespace Game.Characters
{
    public interface IAttacker
    {
        void Attack(Creature enemy, string type = "malee");
        bool WinBattle(Creature enemy);
    }
}

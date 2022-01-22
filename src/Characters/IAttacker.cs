namespace Game.Characters
{
    public interface IAttacker
    {
        void Attack(Creature enemy, out string message, string type = "malee");
        bool WinBattle(Creature enemy);
    }
}

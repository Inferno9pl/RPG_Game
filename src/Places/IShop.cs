using Game.Characters;

namespace Game.Places
{
    public interface IShop
    {
        void BuyFromShop(Creature client, string name, int count = -1);
        void SellToShop(Creature client, string name, int count = -1);
        void ShowAssortment();
        void ShowClientEq(Creature client);
    }
}

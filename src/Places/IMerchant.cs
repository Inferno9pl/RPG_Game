using Game.Characters;

namespace Game.Places
{
    public interface IMerchant
    {
        string BuyFromShop(Creature client, string name, int count = -1);
        string SellToShop(Creature client, string name, int count = -1);
        void ShowAssortment();
        void ShowClientEq(Creature client);
        bool AskForQuantity(string name);
    }
}

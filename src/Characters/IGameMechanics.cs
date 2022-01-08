using Game.Objects;
using Game.Places;

namespace Game.Characters
{
    public interface IGameMechanics
    {
        void ShowEq();
        void SellAllTrash(IMerchant shop);
        void Heal(Item item);
    }
}

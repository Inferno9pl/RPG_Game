using Game.Objects;
using Game.Places;

namespace Game.Characters
{
    public interface IGameMechanics
    {
        void ShowEq();
        void ShowSkills();
        void Heal(ItemAndQuantity itemAndQuantity);
    }
}

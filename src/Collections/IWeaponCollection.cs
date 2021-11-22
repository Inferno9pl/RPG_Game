using Game.Objects;

namespace Game.Collections
{
    public interface IWeaponsCollection
    {
        void Show(int equippedWeaponIndex = -1);
        Weapon Get(int index);
        void Add(Weapon weapon);
        void Remove(Weapon weapon, ref int equippedWeaponIndex);
        int Count();
        bool Find(string name, out Weapon weapon);
        int FindIndex(Weapon weapon);
    }
}

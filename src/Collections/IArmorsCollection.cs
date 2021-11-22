using Game.Objects;

namespace Game.Collections
{
    public interface IArmorsCollection
    {
        void Show(int equippedArmorIndex = -1);
        Armor Get(int index);
        void Add(Armor armor);
        void Remove(Armor armor, ref int equippedArmorIndex);
        int Count();
        bool Find(string name, out Armor armor);
        int FindIndex(Armor armor);
    }
}

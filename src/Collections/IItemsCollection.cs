using Game.Objects;

namespace Game.Collections
{
    public interface IItemsCollection
    {
        void Show();
        ItemAndQuantity Get(int index);
        void Add(Item item, int quantity);
        void Remove(Item item, int quantity = -1);
        int Count();
        bool Find(string name, out ItemAndQuantity item);
        int FindIndex(Item item);
    }
}

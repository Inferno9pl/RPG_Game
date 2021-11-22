using Game.Objects;
using System;
using System.Collections.Generic;

namespace Game.Collections
{
    public class ItemsCollection : IItemsCollection
    {
        private List<ItemAndQuantity> Items { get; init; }

        public ItemsCollection()
        {
            Items = new();
        }

        public void Show()
        {
            Console.WriteLine("  Przedmioty:");
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Console.WriteLine("   " + Items[i]);
                }
            }
            else Console.WriteLine("   <brak>");
        }
        public ItemAndQuantity Get(int index)
        {
            return Items[index];
        }
        public void Add(Item item, int quantity)
        {
            if ((quantity < 1) && (!item.Name.Equals("złoto")))
                throw new ArgumentException("Podano niepoprawną ilość");

            var itemIndex = FindIndex(item);

            if (itemIndex == -1)
            {
                Items.Add(new ItemAndQuantity(item, quantity));
            }
            else
            {
                Items[itemIndex].Quantity += quantity;
            }
        }
        public void Remove(Item item, int quantity = -1)
        {
            var itemIndex = FindIndex(item);

            if (quantity == -1 || quantity == Items[itemIndex].Quantity)
            {
                Items.RemoveAt(itemIndex);
                Items.TrimExcess();
            }
            else
            {
                Items[itemIndex].Quantity -= quantity;
                if (Items[itemIndex].Quantity < 0)
                {
                    Items[itemIndex].Quantity = 0;
                    throw new ArgumentException("Podano za dużą ilość");
                }
            }
        }
        public int Count()
        {
            return Items.Count;
        }
        public bool Find(string name, out ItemAndQuantity item)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Item.Name.Equals(name))
                {
                    item = Items[i];
                    return true;
                }
            }
            //Console.WriteLine("Nie znaleziono " + name);
            item = null;
            return false;
        }
        public int FindIndex(Item item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Item.Equals(item))
                    return i;

            return -1;
        }
    }
}

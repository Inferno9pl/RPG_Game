using System;

namespace Game.Objects
{
    public class ItemAndQuantity
    {
        internal ItemAndQuantity() { }
        public ItemAndQuantity(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public Item Item { get; init; }
        public int Quantity { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item);
        }
        public override bool Equals(object obj)
        {
            return obj is ItemAndQuantity itemAndQuantity && Item == itemAndQuantity.Item;
            //return obj is String name && Item.Name == name;
        }
        public override string ToString()
        {
            return Quantity + " x".PadRight(5 - Quantity.ToString().Length) + Item;
        }
    }
}

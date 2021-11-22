namespace Game.Collections
{
    public sealed class Equipment
    {
        internal IArmorsCollection Armors { get; set; }
        internal IWeaponsCollection Weapons { get; set; }
        internal IItemsCollection Items { get; set; }
        public int Gold { get; set; }
        public int EquippedArmorIndex { get; set; }
        public int EquippedWeaponIndex { get; set; }

        public Equipment()
        {
            Armors = new ArmorsCollection();
            Weapons = new WeaponsCollection();
            Items = new ItemsCollection();
            Gold = 0;
            EquippedArmorIndex = -1;
            EquippedWeaponIndex = -1;
        }
    }
}

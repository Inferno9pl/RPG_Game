namespace Game.Collections
{
    public sealed class Equipment
    {
        public ArmorsCollection Armors { get; set; }
        public WeaponsCollection Weapons { get; set; }
        public ItemsCollection Items { get; set; }
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

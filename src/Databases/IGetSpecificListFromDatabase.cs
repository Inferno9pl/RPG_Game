using Game.Objects;
using System.Collections.Generic;

namespace Game.Databases
{
    public interface IGetSpecificListFromDatabase
    {
        List<Item> GetItemsByType(string[] types);
        List<Armor> GetArmorByValue(int value);
        List<Weapon> GetWeaponsByAttrybute(int maxStrenght, int maxAgility);
    }
}
using System.Collections.Generic;

namespace Game.Databases
{
    public interface IGetObjectDataFromDatabase
    {
        bool GetWeaponData(string name, out string[] result);
        bool GetArmorData(string name, out string[] result);
        bool GetItemData(string name, out string[] result);
        bool GetMonsterData(string name, out string[] result);
        SortedDictionary<int, List<string>> GetMonstersLevels();
    }
}
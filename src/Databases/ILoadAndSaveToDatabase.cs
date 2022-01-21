using Game.Characters;
using System.Collections.Generic;

namespace Game.Databases
{
    public interface ILoadAndSaveToDatabase
    {
        void SavePlayer(Knight player);
        public bool LoadPlayer(string playerName, out Knight player);
        List<string> GetSaveFiles();
    }
}
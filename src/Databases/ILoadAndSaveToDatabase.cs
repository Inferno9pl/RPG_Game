using Game.Characters;
using System.Collections.Generic;

namespace Game.Databases
{
    public interface ILoadAndSaveToDatabase
    {
        void SavePlayer(Knight player);
        Knight LoadPlayer(string playerName);
    }
}
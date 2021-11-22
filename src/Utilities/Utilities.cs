using Game.Databases;
using System;
using System.Collections.Generic;

namespace Game.Utils
{
    sealed class Utilities
    {
        public static int Rand(int min, int max)
        {
            Random rand = new();
            return (rand.Next() % (max - min + 1)) + min;
        }
        public static void SeeMonsterLevels(IGetObjectDataFromDatabase database)
        {
            Console.WriteLine("####################");
            foreach (KeyValuePair<int, List<string>> entry in database.GetMonstersLevels())
            {
                int level = entry.Key;
                List<string> names = entry.Value;
                Console.Write(level + " | ");

                for (int i = 0; i < names.Count; i++)
                {
                    Console.Write(names[i] + ", ");
                }

                Console.WriteLine();
            }
        }
    }
}

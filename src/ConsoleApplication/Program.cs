using Game.ConsoleApplication;
using Game.Databases;
using System;
using System.IO;

namespace Game.Main
{
    public class Program
    {
        static void Main()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filesPath = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\..\Files"));

            var db = new DatabaseTxtFile(filesPath);
            _ = new ConsoleGame<DatabaseTxtFile>(db);     
        }

    }
}
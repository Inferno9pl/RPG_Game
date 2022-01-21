using Game.ConsoleApplication;
using Game.Databases;

namespace Game.Main
{
    public class Program
    {
        static void Main()
        {
            var db = new DatabaseTxtFile(@"C:\Users\Infer\OneDrive\Pulpit\Projekt");
            _ = new ConsoleGame<DatabaseTxtFile>(db);     
        }

    }
}
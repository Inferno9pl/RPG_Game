using System;

namespace Game.Objects
{
    public class Skills
    {
        internal Skills() { }
        public Skills(int oneHanded, int twoHanded, int bow, int crossbow)
        {
            OneHanded = oneHanded;
            TwoHanded = twoHanded;
            Bow = bow;
            Crossbow = crossbow;
        }
        public int OneHanded { get; set; }
        public int TwoHanded { get; set; }
        public int Bow { get; set; }
        public int Crossbow { get; set; }
    }
}

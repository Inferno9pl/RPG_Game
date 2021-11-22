namespace Game.Objects
{
    public abstract class Collectable
    {
        public string Name { get; init; }
        public int Value { get; set; }

        override public string ToString()
        {
            return Name.PadRight(10) + " | " + Value + " gold";
        }
    }
}

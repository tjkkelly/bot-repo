namespace TheCountBot.Models
{
    public class NumberStore
    {
        public string Username;

        public int Number;

        public bool Correct;

        public string Timestamp;

        public override string ToString()
        {
            return $"{Username} {Correct} {Number} {Timestamp}\n";
        }
    }
}
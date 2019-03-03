namespace TheCountBot.Api.ApiResponses
{
    public class NewMessageResponse
    {
        public string UserName { get; set; }

        public int Number { get; set; }

        public bool IsCorrect { get; set; }

        public bool IsPalindrome { get; set; }

        public bool IsAllSameDigits { get; set; }

        public bool IsPowerOf10 { get; set; }
    }
}

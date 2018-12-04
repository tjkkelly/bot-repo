namespace TheCountBot.Core.DataModels
{
    public class UserStatistics
    {
        public string Username 
        { 
            get; 
            set; 
        }

        public int MessagesSentCount 
        { 
            get; 
            set;
        }

        public int MistakeCount
        {
            get;
            set;
        }

        public double ErrorRate
        {
            get;
            set;
        }

        public double TotalMessagePercentage
        {
            get;
            set;
        }

        public double TotalErrorRate
        {
            get;
            set;
        }

        public double RelativeErrorRate
        {
            get;
            set;
        }
    }
}

namespace TheCountBot.Configuration
{
    public interface ISettingsProvider
    {
        string Retrieve( string settingKey );
    }
}
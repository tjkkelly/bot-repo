namespace TheCountBot.Configuration
{
    public interface ISettingsProvider
    {
        object Retrieve( string settingKey );
    }
}
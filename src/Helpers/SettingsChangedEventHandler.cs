namespace PartyYomi.Helpers
{
    public delegate void SettingsChangedEventHandler(object sender, string name, object value);
    public interface ISettingsChangedEvent
    {
        event SettingsChangedEventHandler OnSettingsChanged;
    }
}

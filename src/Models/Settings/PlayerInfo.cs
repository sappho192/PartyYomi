using PartyYomi.Helpers;

namespace PartyYomi.Models.Settings
{
    public class PlayerInfo : ISettingsChangedEvent
    {
        public string Name 
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnSettingsChanged?.Invoke(this, nameof(Name), name);
                }
            }
        }
        private string name;

        public event SettingsChangedEventHandler OnSettingsChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}

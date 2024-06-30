using PartyYomi.Helpers;

namespace PartyYomi.Models.Settings
{
    public class UISettings : ISettingsChangedEvent
    {
        public PartyYomiTheme Theme
        {
            get => theme;
            set
            {
                if (value != theme)
                {
                    theme = value;
                    OnSettingsChanged?.Invoke(this, nameof(Theme), theme);
                }
            }
        }
        private PartyYomiTheme theme;

        public bool IsTtsEnabled
        {
            get => isTtsEnabled;
            set
            {
                if (value != isTtsEnabled)
                {
                    isTtsEnabled = value;
                    OnSettingsChanged?.Invoke(this, nameof(IsTtsEnabled), isTtsEnabled);
                }
            }
        }
        private bool isTtsEnabled;


        public event SettingsChangedEventHandler OnSettingsChanged;
    }

    public enum PartyYomiTheme
    {
        Light,
        Dark
    }
}

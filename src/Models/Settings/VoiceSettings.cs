using PartyYomi.Helpers;

namespace PartyYomi.Models.Settings
{
    public class VoiceSettings : ISettingsChangedEvent
    {
        public int GlobalVolume
        {
            get => volume;
            set
            {
                if (value != volume)
                {
                    volume = value;
                    OnSettingsChanged?.Invoke(this, nameof(GlobalVolume), volume);
                }
            }
        }
        private int volume;

        public int GlobalRate
        {
            get => rate;
            set
            {
                if (value != rate)
                {
                    rate = value;
                    OnSettingsChanged?.Invoke(this, nameof(GlobalRate), rate);
                }
            }
        }
        private int rate;

        public event SettingsChangedEventHandler OnSettingsChanged;
    }
}

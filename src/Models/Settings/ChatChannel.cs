using PartyYomi.FFXIV;
using PartyYomi.Helpers;
using System.Windows.Controls;

namespace PartyYomi.Models.Settings
{
    public class ChatChannel : ISettingsChangedEvent
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

        public int ChatCode
        {
            get => (int)chatCode;
            set
            {
                if (value != (int)chatCode)
                {
                    chatCode = (ChatCode)value;
                    OnSettingsChanged?.Invoke(this, nameof(ChatCode), chatCode);
                }
            }
        }
        private ChatCode chatCode;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    OnSettingsChanged?.Invoke(this, nameof(IsEnabled), isEnabled);
                }
            }
        }
        private bool isEnabled;

        public event SettingsChangedEventHandler OnSettingsChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}

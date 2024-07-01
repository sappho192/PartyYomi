using ObservableCollections;
using PartyYomi.Helpers;

namespace PartyYomi.Models.Settings
{
    public class ChatSettings : ISettingsChangedEvent
    {
        public ObservableList<PlayerInfo>? PlayerInfos
        {
            get => playerNames;
            set
            {
                if (value != playerNames)
                {
                    playerNames = value;
                    OnSettingsChanged?.Invoke(this, nameof(PlayerInfos), playerNames);
                }
            }
        }
        private ObservableList<PlayerInfo>? playerNames;

        public event SettingsChangedEventHandler OnSettingsChanged;
    }
}

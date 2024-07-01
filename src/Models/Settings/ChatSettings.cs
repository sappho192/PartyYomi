using ObservableCollections;
using PartyYomi.Helpers;
using YamlDotNet.Serialization;

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

        public ObservableList<ChatChannel>? ChatChannels
        {
            get => chatChannel;
            set
            {
                if (value != chatChannel)
                {
                    chatChannel = value;
                    OnSettingsChanged?.Invoke(this, nameof(ChatChannels), chatChannel);
                }
            }
        }
        private ObservableList<ChatChannel>? chatChannel;

        [YamlIgnore]
        public ObservableList<ChatChannel>? EnabledChatChannels
        {
            get => enabledChatChannels;
            set
            {
                if (value != enabledChatChannels)
                {
                    enabledChatChannels = value;
                    OnSettingsChanged?.Invoke(this, nameof(EnabledChatChannels), enabledChatChannels);
                }
            }
        }
        private ObservableList<ChatChannel>? enabledChatChannels = [];

        public event SettingsChangedEventHandler OnSettingsChanged;
    }
}

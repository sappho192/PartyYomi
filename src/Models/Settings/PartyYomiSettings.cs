using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using NLog.Fluent;
using Newtonsoft.Json;

namespace PartyYomi.Models.Settings
{
    public class PartyYomiSettings
    {
        public static PartyYomiSettings? Instance { get; set; }

        public bool? StandaloneMode { get; set; }
        public ChatSettings? ChatSettings { get; set; }
        public UISettings? UiSettings { get; set; }

        public static PartyYomiSettings CreateDefault()
        {
            var settings = new PartyYomiSettings
            {
                StandaloneMode = false,
                ChatSettings = new ChatSettings
                {
                    PlayerInfos = [
                        new PlayerInfo { Name = "Forename Surname" },
                        ]
                },
                UiSettings = new UISettings
                {
                    Theme = PartyYomiTheme.Light,
                    IsTtsEnabled = true
                }
            };
            var serializer = new SerializerBuilder()
                             .WithNamingConvention(UnderscoredNamingConvention.Instance)
                             .Build();
            File.WriteAllText("settings.yaml", serializer.Serialize(settings));
            return settings;
        }

        private void onSettingsChanged(string group, object sender, string name, object value, bool showValue = true)
        {
            string template = showValue ? " settings {@propertyName} changed to {@value}"
                    : " settings {@propertyName} changed";

            UpdateSettingsFile(this);
        }

        public static void UpdateSettingsFile(PartyYomiSettings settings)
        {
            var serializer = new SerializerBuilder()
                 .WithNamingConvention(UnderscoredNamingConvention.Instance)
                 .Build();
            File.WriteAllText("settings.yaml", serializer.Serialize(settings));
        }

        public static void InitializeSettingsChangedEvent(PartyYomiSettings settings)
        {
            settings.UiSettings.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("UI", sender, name, value); };
            settings.ChatSettings.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("Chat", sender, name, value); };
            settings.ChatSettings.PlayerInfos.ForEach(playerInfo => playerInfo.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("PlayerInfo", sender, name, value); });
            settings.ChatSettings.PlayerInfos.CollectionChanged += PlayerInfos_CollectionChanged;
        }

        private static void PlayerInfos_CollectionChanged(in ObservableCollections.NotifyCollectionChangedEventArgs<PlayerInfo> e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.IsSingleItem)
                    {
                        e.NewItem.OnSettingsChanged += (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                        UpdateSettingsFile(Instance);
                    }
                    else
                    {
                        foreach (var item in e.NewItems)
                        {
                            item.OnSettingsChanged += (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                        }
                        UpdateSettingsFile(Instance);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.IsSingleItem)
                    {
                        e.OldItem.OnSettingsChanged -= (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                        UpdateSettingsFile(Instance);
                    }
                    else
                    {
                        foreach (var item in e.OldItems)
                        {
                            item.OnSettingsChanged -= (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                            UpdateSettingsFile(Instance);
                        }
                    }
                    break;
            }
        }
    }
}

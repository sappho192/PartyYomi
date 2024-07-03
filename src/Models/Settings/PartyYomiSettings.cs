using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using PartyYomi.FFXIV;
using ObservableCollections;
using PartyYomi.Helpers;
using Serilog;
using System.Globalization;

namespace PartyYomi.Models.Settings
{
    public class PartyYomiSettings
    {
        public static PartyYomiSettings? Instance { get; set; }

        public bool? StandaloneMode { get; set; }
        public ChatSettings? ChatSettings { get; set; }
        public UISettings? UiSettings { get; set; }
        public UILanguages? UiLanguages { get; set; }

        [TraceMethod]
        public static PartyYomiSettings CreateDefault()
        {
            var settings = new PartyYomiSettings
            {
                StandaloneMode = false,
                ChatSettings = new ChatSettings
                {
                    PlayerInfos = [
                        new PlayerInfo { Name = "Forename Surname" },
                        ],
                    ChatChannels = [
                        new ChatChannel { Name = "Party", ChatCode = (int)ChatCode.Party, IsEnabled = true },
                        new ChatChannel { Name = "Alliance", ChatCode = (int)ChatCode.Alliance, IsEnabled = true },
                        new ChatChannel { Name = "Say", ChatCode = (int)ChatCode.Say, IsEnabled = false },
                        new ChatChannel { Name = "Shout", ChatCode = (int)ChatCode.Shout, IsEnabled = false },
                        new ChatChannel { Name = "Yell", ChatCode = (int)ChatCode.Yell, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell1", ChatCode = (int)ChatCode.LinkShell1, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell2", ChatCode = (int)ChatCode.LinkShell2, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell3", ChatCode = (int)ChatCode.LinkShell3, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell4", ChatCode = (int)ChatCode.LinkShell4, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell5", ChatCode = (int)ChatCode.LinkShell5, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell6", ChatCode = (int)ChatCode.LinkShell6, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell7", ChatCode = (int)ChatCode.LinkShell7, IsEnabled = false },
                        new ChatChannel { Name = "LinkShell8", ChatCode = (int)ChatCode.LinkShell8, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell1", ChatCode = (int)ChatCode.CWLinkShell1, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell2", ChatCode = (int)ChatCode.CWLinkShell2, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell3", ChatCode = (int)ChatCode.CWLinkShell3, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell4", ChatCode = (int)ChatCode.CWLinkShell4, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell5", ChatCode = (int)ChatCode.CWLinkShell5, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell6", ChatCode = (int)ChatCode.CWLinkShell6, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell7", ChatCode = (int)ChatCode.CWLinkShell7, IsEnabled = false },
                        new ChatChannel { Name = "CWLinkShell8", ChatCode = (int)ChatCode.CWLinkShell8, IsEnabled = false },
                        new ChatChannel { Name = "FreeCompany", ChatCode = (int)ChatCode.FreeCompany,  IsEnabled = false },
                        ],
                    EnabledChatChannels = [],
                },
                UiSettings = new UISettings
                {
                    Theme = PartyYomiTheme.Light,
                    IsTtsEnabled = true
                },
                UiLanguages = new UILanguages
                {
                    CurrentLanguage = UILanguages.LanguageList.First()
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
            Log.Information($"[Settings] {group}@{sender}.{name} → {value}");
            if (group.Equals("ChatChannel") && name.Equals("IsEnabled")) {
                if ((bool)value)
                {
                    ChatSettings?.EnabledChatChannels.Add((ChatChannel)sender);
                }
                else
                {
                    ChatSettings?.EnabledChatChannels.Remove((ChatChannel)sender);
                }
            } 

            UpdateSettingsFile(this);
        }

        public static void UpdateSettingsFile(PartyYomiSettings settings)
        {
            var serializer = new SerializerBuilder()
                 .WithNamingConvention(UnderscoredNamingConvention.Instance)
                 .Build();
            File.WriteAllText("settings.yaml", serializer.Serialize(settings));
        }

        [TraceMethod]
        public static void InitializeSettingsChangedEvent(PartyYomiSettings settings)
        {
            settings.UiSettings.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("UI", sender, name, value); };
            settings.ChatSettings.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("Chat", sender, name, value); };
            settings.ChatSettings.PlayerInfos.ForEach(playerInfo => playerInfo.OnSettingsChanged += (sender, name, value) => {
                settings.onSettingsChanged("PlayerInfo", sender, name, value); 
            });
            settings.ChatSettings.PlayerInfos.CollectionChanged += PlayerInfos_CollectionChanged;
            settings.ChatSettings.ChatChannels.ForEach(chatChannel => chatChannel.OnSettingsChanged += (sender, name, value) => { 
                settings.onSettingsChanged("ChatChannel", sender, name, value); 
            });

            foreach (var item in settings.ChatSettings.ChatChannels)
            {
                if (item.IsEnabled)
                {
                    settings.ChatSettings.EnabledChatChannels.Add(item);
                }
            }
            settings.ChatSettings.EnabledChatChannels.CollectionChanged += EnabledChatChannels_CollectionChanged;

            settings.UiLanguages.OnSettingsChanged += (sender, name, value) => { settings.onSettingsChanged("UILanguages", sender, name, value); };
        }

        private static void EnabledChatChannels_CollectionChanged(in NotifyCollectionChangedEventArgs<ChatChannel> e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.IsSingleItem)
                    {
                        e.NewItem.OnSettingsChanged += (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                    }
                    else
                    {
                        foreach (var item in e.NewItems)
                        {
                            item.OnSettingsChanged += (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.IsSingleItem)
                    {
                        e.OldItem.OnSettingsChanged -= (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                    }
                    else
                    {
                        foreach (var item in e.OldItems)
                        {
                            item.OnSettingsChanged -= (sender, name, value) => { Instance?.onSettingsChanged("PlayerInfo", sender, name, value); };
                        }
                    }
                    break;
            }
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

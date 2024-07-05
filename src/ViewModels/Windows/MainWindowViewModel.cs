using Octokit;
using PartyYomi.FFXIV;
using PartyYomi.Helpers;
using PartyYomi.Models;
using PartyYomi.Models.Settings;
using Serilog;
using Sharlayan.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using Wpf.Ui.Controls;

namespace PartyYomi.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public GameContext gameContext;

        public MainWindowViewModel()
        {
            // Initialize the view model
            InitTTS();
            if ((bool)!PartyYomiSettings.Instance.StandaloneMode)
            {
                gameContext = GameContext.Instance();
                ChatQueue.ChatLogItems.CollectionChanged += ChatLogItems_CollectionChanged;
            }
        }

        [TraceMethod]
        private void ChatLogItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ChatQueue.oq.TryDequeue(out ChatLogItem? chat);
            if (chat == null || !PartyYomiSettings.Instance.UiSettings.IsTtsEnabled)
            {
                return;
            }
            int.TryParse(chat.Code, System.Globalization.NumberStyles.HexNumber, null, out var intCode);
            if (PartyYomiSettings.Instance.ChatSettings.ChatChannels.Where(ch => ch.ChatCode == intCode && ch.IsEnabled).Any())
            {
                string line = chat.Line;
                ChatLogItem decodedChat = chat.Bytes.DecodeAutoTranslate();

                var author = decodedChat.Line.RemoveAfter(":");
                var sentence = decodedChat.Line.RemoveBefore(":");
                foreach (var player in PartyYomiSettings.Instance.ChatSettings.PlayerInfos)
                {
                    if (player.Name == author)
                    {
                        return;
                    }
                }

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    chatBox.Text += $"{sentence}{Environment.NewLine}";
                //});
                GlobalSpeech.defaultTTS.SpeakAsync(sentence);
            }
        }

        [TraceMethod]
        private static void InitTTS()
        {
            if (!GlobalSpeech.Initialize())
            {
                App.RequestShutdown();
            }
            GlobalSpeech.defaultTTS.SpeakAsync("パーティー読みを起動します");
        }

        [ObservableProperty]
        private string _applicationTitle = "PartyYomi";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems =
        [
            new NavigationViewItem()
            {
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            }
        ];

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems =
        [
            new NavigationViewItem()
            {
                Content = Localizer.getString("main.navigation.settings"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        ];

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}

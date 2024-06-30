using PartyYomi.FFXIV;
using PartyYomi.Helpers;
using Sharlayan.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Speech.Synthesis;
using Wpf.Ui.Controls;

namespace PartyYomi.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly SpeechSynthesizer tts = new();

        public GameContext gameContext;

        public MainWindowViewModel()
        {
            // Initialize the view model
            InitTTS();
            gameContext = GameContext.Instance();
            ChatQueue.ChatLogItems.CollectionChanged += ChatLogItems_CollectionChanged;
        }

        private void ChatLogItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ChatQueue.oq.TryDequeue(out ChatLogItem? chat);
            if (chat == null)
            {
                return;
            }
            int.TryParse(chat.Code, System.Globalization.NumberStyles.HexNumber, null, out var intCode);
            var code = (ChatCode)intCode;
            if (code == ChatCode.Party || code == ChatCode.Say)
            {
                string line = chat.Line;
                ChatLogItem decodedChat = chat.Bytes.DecodeAutoTranslate();

                var author = decodedChat.Line.RemoveAfter(":");
                var sentence = decodedChat.Line.RemoveBefore(":");

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    chatBox.Text += $"{sentence}{Environment.NewLine}";
                //});
                tts.SpeakAsync(sentence);
            }
        }

        private void InitTTS()
        {
            tts.SetOutputToDefaultAudioDevice();
            var list = tts.GetInstalledVoices().ToList();
            // Exit if "Microsoft Haruka Desktop" is not installed
            if (list.Find(x => x.VoiceInfo.Name == "Microsoft Haruka Desktop") == null)
            {
                if (System.Windows.MessageBox.Show("Microsoft Haruka Desktop 음성을 설치해주세요.") == System.Windows.MessageBoxResult.OK)
                {
                    var ps = new ProcessStartInfo("https://support.microsoft.com/ko-kr/topic/%EC%9D%8C%EC%84%B1%EC%9D%84-%EC%9C%84%ED%95%9C-%EC%96%B8%EC%96%B4-%ED%8C%A9-%EB%8B%A4%EC%9A%B4%EB%A1%9C%EB%93%9C-24d06ef3-ca09-ddcc-70a0-63606fd16394")
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(ps);
                }
                Application.Current.Shutdown();
            }

            tts.SelectVoice("Microsoft Haruka Desktop");

            // Speak in different thread
            //tts.SpeakAsync("パーティー読みを開始します");
        }

        [ObservableProperty]
        private string _applicationTitle = "PartyYomi";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "PartyYomi",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Data",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}

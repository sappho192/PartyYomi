﻿using PartyYomi.FFXIV;
using PartyYomi.Helpers;
using PartyYomi.Models.Settings;
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
                tts.SpeakAsync(sentence);
            }
        }

        [TraceMethod]
        private void InitTTS()
        {
            tts.SetOutputToDefaultAudioDevice();
            var list = tts.GetInstalledVoices().ToList();
            // Exit if "Microsoft Haruka Desktop" is not installed
            if (list.Find(x => x.VoiceInfo.Name == "Microsoft Haruka Desktop") == null)
            {
                if (System.Windows.MessageBox.Show("Microsoft Haruka Desktop 음성을 설치해주세요.") == System.Windows.MessageBoxResult.OK)
                {
                    var ps = new ProcessStartInfo("https://github.com/sappho192/PartyYomi/wiki/%EC%9C%88%EB%8F%84%EC%9A%B0-TTS-%EC%84%A4%EC%B9%98%ED%95%98%EA%B8%B0")
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
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "설정",
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

using PartyYomi.Models.Settings;
using PartyYomi.Services;
using PartyYomi.Views.Windows;
using Wpf.Ui;

namespace PartyYomi.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;
        [ObservableProperty]
        private bool _isSpeechActive = true;
        [ObservableProperty]
        private string _speechToggleState = "TTS 작동 중";
        [ObservableProperty]
        private string _speechToggleDescription = "게임 내 채팅을 읽을 준비가 되어있습니다.";
        [ObservableProperty]
        private string _speechIcon = "DesktopSpeaker20";
        [ObservableProperty]
        private List<PlayerInfo> _playerInfos = PartyYomiSettings.Instance.ChatSettings.PlayerInfos;

        private readonly INavigationService _navigationService;

        public DashboardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            if (PartyYomiSettings.Instance.UiSettings.IsTtsEnabled)
            {
                _isSpeechActive = true;
            }
            else
            {
                _isSpeechActive = false;
            }
            OnSpeechToggle();
        }

        [RelayCommand]
        private void OnSpeechToggle()
        {
            if (IsSpeechActive)
            {
                SpeechToggleState = "TTS 작동 중";
                SpeechToggleDescription = "게임 내 채팅을 읽을 준비가 되어있습니다.";
                SpeechIcon = "DesktopSpeaker20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = true;

            }
            else
            {
                SpeechToggleState = "TTS 작동 중지됨";
                SpeechToggleDescription = "TTS를 활성화하여 게임 내 채팅을 읽게 해보세요.";
                SpeechIcon = "DesktopSpeakerOff20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = false;
            }
        }

        [RelayCommand]
        private void OnNavigateToSettingsPage()
        {
            _ = _navigationService.Navigate(typeof(Views.Pages.SettingsPage));
        }
    }
}

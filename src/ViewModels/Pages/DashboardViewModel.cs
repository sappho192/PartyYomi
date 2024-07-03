using ObservableCollections;
using PartyYomi.Helpers;
using PartyYomi.Models.Settings;
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
        private string _speechToggleState = Localizer.getString("dashboard.tts.enabled");
        [ObservableProperty]
        private string _speechToggleDescription = Localizer.getString("dashboard.tts.enabled.description");
        [ObservableProperty]
        private string _speechIcon = "DesktopSpeaker20";
        [ObservableProperty]
        private INotifyCollectionChangedSynchronizedView<string> _playerInfos =
            PartyYomiSettings.Instance.ChatSettings.PlayerInfos.CreateView(player => player.Name).ToNotifyCollectionChanged();
        [ObservableProperty]
        private INotifyCollectionChangedSynchronizedView<string> _enabledChatChannels =
            PartyYomiSettings.Instance.ChatSettings.EnabledChatChannels.CreateView(ch => ch.Name).ToNotifyCollectionChanged();

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

        [TraceMethod]
        [RelayCommand]
        private void OnSpeechToggle()
        {
            if (IsSpeechActive)
            {
                SpeechToggleState = Localizer.getString("dashboard.tts.enabled");
                SpeechToggleDescription = Localizer.getString("dashboard.tts.enabled.description");
                SpeechIcon = "DesktopSpeaker20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = true;
            }
            else
            {
                SpeechToggleState = Localizer.getString("dashboard.tts.disabled");
                SpeechToggleDescription = Localizer.getString("dashboard.tts.disabled.description");
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

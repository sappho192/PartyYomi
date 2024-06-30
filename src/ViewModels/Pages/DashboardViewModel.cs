using PartyYomi.Models.Settings;

namespace PartyYomi.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;
        [ObservableProperty]
        private bool _isSpeechActive = true;
        [ObservableProperty]
        private string _speechToggleState = "TTS On";
        [ObservableProperty]
        private string _speechIcon = "DesktopSpeaker20";

        public DashboardViewModel()
        {
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
                SpeechToggleState = "TTS On";
                SpeechIcon = "DesktopSpeaker20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = true;

            }
            else
            {
                SpeechToggleState = "TTS Off";
                SpeechIcon = "DesktopSpeakerOff20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = false;
            }
        }
    }
}

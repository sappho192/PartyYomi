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

        [RelayCommand]
        private void OnSpeechToggle()
        {
            if (IsSpeechActive)
            {
                SpeechToggleState = "TTS Off";
                SpeechIcon = "DesktopSpeakerOff20";
            }
            else
            {
                SpeechToggleState = "TTS On";
                SpeechIcon = "DesktopSpeaker20";
            }
            IsSpeechActive = !IsSpeechActive;
        }
    }
}

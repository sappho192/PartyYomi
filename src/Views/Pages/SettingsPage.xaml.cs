using PartyYomi.Helpers;
using PartyYomi.Models.Settings;
using PartyYomi.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PartyYomi.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        private bool _isInitialized;
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            _isInitialized = true;
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            ViewModel.SelectedPlayerIndex = listView.SelectedIndex;
        }

        private void tbName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var tbName = sender as System.Windows.Controls.TextBox;
            if (tbName != null && tbName.Text != string.Empty)
            {
                ViewModel.NewPlayerName = tbName.Text;
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            var comboBox = sender as System.Windows.Controls.ComboBox;
            ViewModel.SelectedUiLanguageIndex = comboBox.SelectedIndex;
            PartyYomiSettings.Instance.UiLanguages.CurrentLanguage = UILanguages.LanguageList[ViewModel.SelectedUiLanguageIndex];

            if (comboBox.IsVisible)
            {
                System.Windows.MessageBox.Show(Localizer.GetSpecificString(
                    "settings.general.language.comment", 
                    PartyYomiSettings.Instance.UiLanguages.CurrentLanguage.Code));
            }
        }

        private void GlobalVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as System.Windows.Controls.Slider;
            ViewModel.GlobalVolume = (int)slider.Value;
            PartyYomiSettings.Instance.VoiceSettings.GlobalVolume = ViewModel.GlobalVolume;
        }

        private void GlobalVolumeSlider_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.GlobalVolume = 100;
            PartyYomiSettings.Instance.VoiceSettings.GlobalVolume = 100;
        }

        private void GlobalRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as System.Windows.Controls.Slider;
            ViewModel.GlobalRate = (int)slider.Value;
            PartyYomiSettings.Instance.VoiceSettings.GlobalRate = ViewModel.GlobalRate;
        }

        private void GlobalRateSlider_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.GlobalRate = 0;
            PartyYomiSettings.Instance.VoiceSettings.GlobalRate = 0;
        }
    }
}

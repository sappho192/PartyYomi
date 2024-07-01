using ObservableCollections;
using PartyYomi.Models.Settings;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace PartyYomi.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        [ObservableProperty]
        private INotifyCollectionChangedSynchronizedView<string> _playerInfos =
            PartyYomiSettings.Instance.ChatSettings.PlayerInfos.CreateView(player => player.Name).ToNotifyCollectionChanged();

        [ObservableProperty]
        private int _selectedPlayerIndex = -1;

        [ObservableProperty]
        private string _newPlayerName = string.Empty;

        private readonly IContentDialogService _contentDialogService;
        public SettingsViewModel(IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"PartyYomi - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;
                    PartyYomiSettings.Instance.UiSettings.Theme = PartyYomiTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;
                    PartyYomiSettings.Instance.UiSettings.Theme = PartyYomiTheme.Dark;

                    break;
            }
        }

        [RelayCommand]
        private void OnUpdateNewPlayerName(object content)
        {
            NewPlayerName = content as string;
        }

        [RelayCommand]
        private async Task OnAddPlayerInfo(object content)
        {
            ContentDialogResult dialogResult = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "플레이어 이름을 적어주세요",
                    Content = content,
                    PrimaryButtonText = "확인",
                    CloseButtonText = "취소",
                }
            );

            var resultBool = dialogResult switch
            {
                ContentDialogResult.Primary => true,
                ContentDialogResult.Secondary => false,
                _ => false
            };

            if (!resultBool)
                return;

            var fullName = NewPlayerName.Trim();

            if (string.IsNullOrEmpty(fullName))
                return;

            // Check if the player already exists
            if (PlayerInfos.Any(name => name == fullName))
            {
                //System.Windows.MessageBox.Show("Player already exists", "Error", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                var errorDialog = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "오류",
                    Content = "이미 존재하는 플레이어입니다.",
                    CloseButtonText = "헉...!"
                };
                errorDialog.ShowDialogAsync();
                return;
            }

            PartyYomiSettings.Instance.ChatSettings.PlayerInfos.Add(new PlayerInfo { Name = fullName });
            //PlayerInfos = PartyYomiSettings.Instance.ChatSettings.PlayerInfos;
        }

        [RelayCommand]
        private void OnRemovePlayerInfo()
        {
            if (SelectedPlayerIndex < 0)
                return;
            PartyYomiSettings.Instance.ChatSettings.PlayerInfos.RemoveAt(SelectedPlayerIndex);
            //PlayerInfos = PartyYomiSettings.Instance.ChatSettings.PlayerInfos;
        }
    }
}

using MdXaml;
using ObservableCollections;
using Octokit;
using PartyYomi.Helpers;
using PartyYomi.Models.Settings;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private string _speechToggleState = Localizer.GetString("dashboard.tts.enabled");
        [ObservableProperty]
        private string _speechToggleDescription = Localizer.GetString("dashboard.tts.enabled.description");
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

            // run CheckUpdate() in different thread
            System.Windows.Application.Current.Dispatcher.InvokeAsync(CheckUpdate);
        }

        [TraceMethod]
        private void CheckUpdate()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var githubClient = new GitHubClient(new ProductHeaderValue("PartyYomi"));
            var latestRelease = githubClient.Repository.Release.GetLatest("sappho192", "partyyomi").Result;
            var latestVersion = new Version(latestRelease.TagName);

            if (currentVersion.CompareTo(latestVersion) >= 0)
            {
                Log.Information("PartyYomi is up to date");
            }
            else
            {
                Log.Information("PartyYomi is outdated");
                Log.Information($"Current version: {currentVersion}");
                Log.Information($"Latest version: {latestVersion}");

                AskToUpdate(currentVersion, latestRelease, latestVersion);
            }
        }

        private static void AskToUpdate(Version? currentVersion, Release latestRelease, Version latestVersion)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localizer.GetString("main.update.description"));
            sb.AppendLine();
            sb.AppendLine($"{Localizer.GetString("main.update.current_version")}{currentVersion.ToString(3)}");
            sb.AppendLine($"{Localizer.GetString("main.update.latest_version")}**{latestVersion.ToString(3)}**");
            sb.AppendLine();
            sb.AppendLine(latestRelease.Body);
            var updateContent = sb.ToString();
            var markdownEngine = new Markdown();
            FlowDocument document = markdownEngine.Transform(updateContent);
            document.FontFamily = new System.Windows.Media.FontFamily("sans-serif");
            var scrollViewer = new ScrollViewer
            {
                Content = new RichTextBox
                {
                    Document = document
                }
            };
            var updateMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = Localizer.GetString("main.update.title"),
                Content = scrollViewer,
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = Localizer.GetString("yes"),
                CloseButtonText = Localizer.GetString("no")
            };
            var result = updateMessageBox.ShowDialogAsync();
            if (result.Result == Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                Log.Information("User clicked Yes to update PartyYomi");
                var ps = new ProcessStartInfo(latestRelease.Assets[0].BrowserDownloadUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
        }

        [TraceMethod]
        [RelayCommand]
        private void OnSpeechToggle()
        {
            if (IsSpeechActive)
            {
                SpeechToggleState = Localizer.GetString("dashboard.tts.enabled");
                SpeechToggleDescription = Localizer.GetString("dashboard.tts.enabled.description");
                SpeechIcon = "DesktopSpeaker20";
                PartyYomiSettings.Instance.UiSettings.IsTtsEnabled = true;
            }
            else
            {
                SpeechToggleState = Localizer.GetString("dashboard.tts.disabled");
                SpeechToggleDescription = Localizer.GetString("dashboard.tts.disabled.description");
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

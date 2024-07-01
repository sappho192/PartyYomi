using PartyYomi.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PartyYomi.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
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
    }
}

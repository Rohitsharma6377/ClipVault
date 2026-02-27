using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClipVault.Views;

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "ClipVault";

            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
            }

            NavView.SelectedItem = NavView.MenuItems[0];
            Navigate("ClipboardListPage");
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
                Navigate("SettingsPage");
            else if (args.SelectedItem is NavigationViewItem item)
                Navigate(item.Tag?.ToString());
        }

        private void Navigate(string? tag)
        {
            switch (tag)
            {
                case "ClipboardListPage":
                    ContentFrame.Navigate(typeof(ClipboardListPage));
                    break;
                case "PinnedPage":
                    ContentFrame.Navigate(typeof(ClipboardListPage), "Pinned");
                    break;
                case "SettingsPage":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    break;
                case "PremiumPage":
                    ContentFrame.Navigate(typeof(PremiumPage));
                    break;
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var vm = App.MainViewModel;
            if (vm != null)
            {
                vm.SearchText = sender.Text;
                vm.SearchCommand?.Execute(null);
            }
        }
    }
}

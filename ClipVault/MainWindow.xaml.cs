using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClipVault.Views;
using System;

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
            {
                Navigate("SettingsPage");
            }
            else if (args.SelectedItem is NavigationViewItem item)
            {
                Navigate(item.Tag?.ToString());
            }
        }

        private void Navigate(string? tag)
        {
            if (tag == "ClipboardListPage")
                ContentFrame.Navigate(typeof(ClipboardListPage));
            else if (tag == "PinnedPage")
                ContentFrame.Navigate(typeof(ClipboardListPage), "Pinned");
            else if (tag == "SettingsPage")
                ContentFrame.Navigate(typeof(SettingsPage));
            else if (tag == "PremiumPage")
                ContentFrame.Navigate(typeof(PremiumPage));
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (App.MainViewModel != null)
            {
                App.MainViewModel.SearchText = sender.Text;
                App.MainViewModel.SearchCommand?.Execute(null);
            }
        }
    }
}

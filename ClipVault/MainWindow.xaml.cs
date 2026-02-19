using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Runtime.InteropServices; // For P/Invoke if needed
using WinRT; // For WindowId

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Enable Mica Backdrop
            if (MicaController.IsSupported())
            {
                this.SystemBackdrop = new MicaBackdrop();
            }
            else if (DesktopAcrylicController.IsSupported())
            {
                this.SystemBackdrop = new DesktopAcrylicBackdrop();
            }

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null); // Simple extension
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial page
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
                Navigate(item.Tag.ToString());
            }
        }

        private void Navigate(string tag)
        {
            switch (tag)
            {
                case "ClipboardListPage":
                    ContentFrame.Navigate(typeof(Views.ClipboardListPage));
                    break;
                case "PinnedPage":
                    // In a real app, maybe filter ItemsSource or pass parameter
                    // For now reuse same page but we'd need to tell VM to filter
                    // Or maintain separate view logic. 
                    // Let's just navigate to ClipboardListPage. VM handles filtering via search or we add filter property.
                    // For simplicity in this demo, PinnedPage just shows all but user can toggle pin filter.
                    // I'll create a PinnedItemsPage if needed, but reusing is better if I can pass parameter.
                    ContentFrame.Navigate(typeof(Views.ClipboardListPage), "Pinned");
                    break;
                case "PremiumPage":
                    // ContentFrame.Navigate(typeof(Views.PremiumPage));
                    break;
                case "SettingsPage":
                    ContentFrame.Navigate(typeof(Views.SettingsPage));
                    break;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            App.MainViewModel.SearchText = args.QueryText;
            App.MainViewModel.SearchCommand.Execute(null);
        }
    }
}

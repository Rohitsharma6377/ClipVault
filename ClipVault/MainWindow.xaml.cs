using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media; // Required for MicaBackdrop
using Microsoft.UI.Windowing;
using WinRT;

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Set Title and TitleBar
            this.Title = "ClipVault";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null);

            // Set Backdrop (Glass Effect)
            // Windows App SDK 1.3+ supports this simple API
            try
            {
                this.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };
            }
            catch (Exception ex)
            {
                // Fallback or log if on older OS (though SDK 1.6 should handle it gracefully or ignore)
                System.Diagnostics.Debug.WriteLine($"Backdrop failed: {ex.Message}");
                // Try Acrylic if Mica fails?
                try { this.SystemBackdrop = new DesktopAcrylicBackdrop(); } catch { }
            }

            // Set Window Size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 700));

            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Ensure size is enforced if needed, but AppWindow.Resize in constructor is usually enough.
            this.Activated -= MainWindow_Activated;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigate to home by default
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

        private void Navigate(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;

            switch (tag)
            {
                case "ClipboardListPage":
                    ContentFrame.Navigate(typeof(Views.ClipboardListPage));
                    break;
                case "PinnedPage":
                    ContentFrame.Navigate(typeof(Views.ClipboardListPage), "Pinned");
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

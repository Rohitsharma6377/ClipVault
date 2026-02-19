using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing; // For AppWindow
using System.Runtime.InteropServices;
using WinRT;

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Enable Mica Backdrop
            try
            {
                if (MicaController.IsSupported())
                {
                    this.SystemBackdrop = new MicaBackdrop();
                }
                else if (DesktopAcrylicController.IsSupported())
                {
                    this.SystemBackdrop = new DesktopAcrylicBackdrop();
                }
            }
            catch { /* Fail gracefully on older OS */ }

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null);

            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Set Size only once on first activation to avoid resizing loops or issues
            // But simple Resize call is usually safe.
            // Let's ensure we are on UI thread.

            // Simple check if already sized? We can't easily. 
            // Just resize on constructor or first activation is fine.
            // Moving to constructor was causing issues? Maybe. 
            // Let's try to set size here safely.

            try
            {
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
                var appWindow = AppWindow.GetFromWindowId(windowId);
                if (appWindow != null)
                {
                    // Check current size to avoid jitter
                    if (appWindow.Size.Width != 450 || appWindow.Size.Height != 700)
                    {
                        appWindow.Resize(new Windows.Graphics.SizeInt32(450, 700)); // Compact size
                    }
                }
            }
            catch { }

            // Unsubscribe to avoid repeated resizing
            this.Activated -= MainWindow_Activated;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
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

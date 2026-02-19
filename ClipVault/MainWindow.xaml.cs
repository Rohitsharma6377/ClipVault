using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "ClipVault";

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
            if (tag == "ClipboardListPage")
                ContentFrame.Navigate(typeof(Views.ClipboardListPage));
            else if (tag == "PinnedPage")
                ContentFrame.Navigate(typeof(Views.ClipboardListPage), "Pinned");
            else if (tag == "SettingsPage")
                ContentFrame.Navigate(typeof(Views.SettingsPage));
        }
    }
}

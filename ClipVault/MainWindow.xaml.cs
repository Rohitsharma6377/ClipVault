using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using WinRT;

namespace ClipVault
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Title = "ClipVault";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            // Set Backdrop
            try
            {
                this.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };
            }
            catch { }

            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 700));

            // Set initial navigation
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

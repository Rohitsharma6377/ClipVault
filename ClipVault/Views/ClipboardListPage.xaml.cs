using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ClipVault.ViewModels;
using ClipVault.Models;

namespace ClipVault.Views
{
    public sealed partial class ClipboardListPage : Page
    {
        public MainViewModel ViewModel { get; }

        public ClipboardListPage()
        {
            ViewModel = App.MainViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string param && param == "Pinned")
            {
                ViewModel.IsPinnedFilter = true;
                HeaderTitle.Text = "Pinned Clips";
            }
            else
            {
                ViewModel.IsPinnedFilter = false;
                HeaderTitle.Text = "Clipboard History";
            }

            ViewModel.LoadItemsAsync(true);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ClipboardItem item)
            {
                ViewModel.CopyItemCommand.Execute(item);
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is ClipboardItem item)
            {
                ViewModel.TogglePinCommand.Execute(item);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ClipboardItem item)
            {
                ViewModel.DeleteItemCommand.Execute(item);
            }
        }
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ClipVault.ViewModels;

namespace ClipVault.Views
{
    public sealed partial class ClipboardListPage : Page
    {
        public MainViewModel ViewModel { get; }

        public ClipboardListPage()
        {
            this.InitializeComponent();
            ViewModel = App.MainViewModel;
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string param && param == "Pinned")
            {
                ViewModel.IsPinnedFilter = true;
            }
            else
            {
                ViewModel.IsPinnedFilter = false;
            }

            // Reload with new filter
            ViewModel.LoadItems();
        }
    }
}

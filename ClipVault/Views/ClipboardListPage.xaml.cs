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
            ViewModel = App.MainViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter is string param && param == "Pinned")
            {
                HeaderTitle.Text = "Pinned Clips";
                ViewModel.LoadPinnedItems();
            }
            else
            {
                HeaderTitle.Text = "Clipboard History";
                ViewModel.LoadAllItems();
            }
        }
    }
}

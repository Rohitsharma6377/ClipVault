using Microsoft.UI.Xaml.Controls;
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
    }
}

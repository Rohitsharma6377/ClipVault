using Microsoft.UI.Xaml.Controls;
using ClipVault.ViewModels;
using ClipVault;

namespace ClipVault.Views
{
    public sealed partial class PremiumPage : Page
    {
        public MainViewModel ViewModel { get; }

        public PremiumPage()
        {
            ViewModel = App.MainViewModel;
            this.InitializeComponent();
        }
    }
}

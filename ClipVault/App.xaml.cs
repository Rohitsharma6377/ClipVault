using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClipVault.ViewModels;
using ClipVault.Helpers;

namespace ClipVault
{
    public sealed partial class App : Application
    {
        public static MainViewModel? MainViewModel { get; private set; }
        private Window? m_window;

        public App()
        {
            this.InitializeComponent();

            // Register converters in code to avoid XAML runtime resource crashes
            this.Resources["BooleanToVisibilityConverter"] = new BooleanToVisibilityConverter();
            this.Resources["BooleanToVisibilityInvertedConverter"] = new BooleanToVisibilityInvertedConverter();
            this.Resources["PremiumStatusConverter"] = new PremiumStatusConverter();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                MainViewModel = new MainViewModel();
                m_window = new MainWindow();
                m_window.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App launch error: {ex}");
            }
        }
    }
}

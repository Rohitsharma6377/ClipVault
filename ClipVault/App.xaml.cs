using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClipVault.ViewModels;

namespace ClipVault
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the error
            Debug.WriteLine($"Unhandled exception: {e.Message}");

            // Prevent crash if possible
            e.Handled = true;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
        public static MainViewModel MainViewModel { get; } = new MainViewModel(); // Keep static reference safe
    }
}

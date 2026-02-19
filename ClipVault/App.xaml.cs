using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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
            Debug.WriteLine($"Unhandled exception: {e.Message}");
            e.Handled = true;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
        public static MainViewModel MainViewModel { get; } = new MainViewModel();
    }
}

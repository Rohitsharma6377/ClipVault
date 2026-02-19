using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ClipVault.ViewModels;
using System;
using System.IO;

namespace ClipVault
{
    public partial class App : Application
    {
        public static MainViewModel MainViewModel { get; private set; }
        public static Window MainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
            MainViewModel = new MainViewModel();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}

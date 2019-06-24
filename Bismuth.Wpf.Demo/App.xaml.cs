using System.Windows;
using Bismuth.Wpf.Demo.ViewModels;

namespace Bismuth.Wpf.Demo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow { DataContext = new MainViewModel() };
            MainWindow.Show();
        }
    }
}

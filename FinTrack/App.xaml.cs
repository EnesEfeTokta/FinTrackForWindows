using FinTrack.Views;
using System.Windows;

namespace FinTrack
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new MainWindow().Show(); // Şimdlik böyle olsun
        }
    }
}
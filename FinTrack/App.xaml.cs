using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace FinTrack;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // Örnek kullanım
        string connStr = Configuration.GetConnectionString("PostgresConnection");
        MessageBox.Show("Bağlantı: " + connStr);
    }
}


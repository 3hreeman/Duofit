using Duofit.Views;

namespace Duofit;

/// <summary>
/// Application entry point
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}

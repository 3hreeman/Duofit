using Duofit.Views;

namespace Duofit;

/// <summary>
/// AppShell - Navigation container
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}

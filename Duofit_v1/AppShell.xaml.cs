using Duofit.Pages;

namespace Duofit;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for pages that are not declared as ShellContent
        Routing.RegisterRoute(nameof(StopwatchPage), typeof(StopwatchPage));
        Routing.RegisterRoute(nameof(MetronomePage), typeof(MetronomePage));
        Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));
    }
}

using Duofit.Pages;

namespace Duofit_v1;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for pages that are not declared as ShellContent
		Routing.RegisterRoute(nameof(StopwatchPage), typeof(StopwatchPage));
		Routing.RegisterRoute(nameof(MetronomePage), typeof(MetronomePage));
	}
}

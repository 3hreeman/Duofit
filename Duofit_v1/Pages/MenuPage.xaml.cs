using Duofit.Services;
using Microsoft.Maui.Controls;

namespace Duofit.Pages
{
	public partial class MenuPage : ContentPage
	{
		public MenuPage()
		{
			InitializeComponent();
		}

		void OnPageSizeChanged(object sender, System.EventArgs e)
		{
			if (Width <= 0 || Height <= 0)
				return;

			var gridSide = Math.Min(Width, Height);
			MenuGrid.WidthRequest = gridSide;
			MenuGrid.HeightRequest = gridSide;
			var horizontalMargin = Math.Max(0, (Width - gridSide) / 2);
			var verticalMargin = Math.Max(0, (Height - gridSide) / 2);
			MenuGrid.Margin = new Thickness(horizontalMargin, verticalMargin);
		}

		async void OnStopwatchClicked(object sender, System.EventArgs e)
		{
			await Shell.Current.GoToAsync(nameof(StopwatchPage));
		}

		async void OnMetronomeClicked(object sender, System.EventArgs e)
		{
			await Shell.Current.GoToAsync(nameof(MetronomePage));
		}
		
		async void OnTimerClicked(object sender, System.EventArgs e)
		{
			await Shell.Current.GoToAsync(nameof(TimerPage));
		}

		void OnComingSoonClicked(object sender, System.EventArgs e)
		{
			// ToastService.Show("추후 개발 예정입니다");
		}
	}
}

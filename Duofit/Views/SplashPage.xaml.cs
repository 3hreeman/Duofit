using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Duofit.Pages
{
    public partial class SplashPage : ContentPage
    {
        private readonly int _delayMilliseconds = 1000; // 1초 대기 시간
        public SplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // 1초 대기 후 메뉴 화면으로 전환
            await Task.Delay(_delayMilliseconds);
            // 앱 진입점(MainPage)을 MenuPage로 교체
            Application.Current.MainPage = new NavigationPage(new MenuPage());
        }
    }
}


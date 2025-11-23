using Duofit.ViewModels;

namespace Duofit.Views;

/// <summary>
/// MainPage code-behind - MVVM compliant
/// NO business logic or data manipulation here
/// Only initialization and ViewModel binding
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}

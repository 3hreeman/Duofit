using Duofit.ViewModels;

namespace Duofit.Views;

/// <summary>
/// MainPage code-behind - MVVM compliant
/// NO business logic or data manipulation here
/// Only initialization and ViewModel binding
/// </summary>
public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // Load data when page appears
        Loaded += async (s, e) => await viewModel.LoadWorkoutSessionsCommand.ExecuteAsync(null);
    }
}

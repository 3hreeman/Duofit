using CommunityToolkit.Mvvm.ComponentModel;

namespace Duofit.ViewModels;

/// <summary>
/// Base ViewModel class with common functionality
/// Uses CommunityToolkit.Mvvm for MVVM implementation
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    public bool IsNotBusy => !IsBusy;
}

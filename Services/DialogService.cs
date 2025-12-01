namespace Duofit.Services;

/// <summary>
/// Service for displaying dialogs and alerts
/// Abstracts UI framework dependencies from ViewModels
/// </summary>
public interface IDialogService
{
    Task DisplayAlertAsync(string title, string message, string cancel);
    Task<bool> DisplayConfirmAsync(string title, string message, string accept, string cancel);
}

public class DialogService : IDialogService
{
    public Task DisplayAlertAsync(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
        return Task.CompletedTask;
    }

    public Task<bool> DisplayConfirmAsync(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        return Task.FromResult(false);
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duofit.Models;
using Duofit.Services;
using System.Collections.ObjectModel;

namespace Duofit.ViewModels;

/// <summary>
/// ViewModel for the main page displaying workout sessions
/// Implements MVVM pattern with CommunityToolkit.Mvvm
/// All UI logic and data binding is handled here
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IWorkoutService _workoutService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<WorkoutSession> workoutSessions = new();

    [ObservableProperty]
    private WorkoutSession? selectedWorkoutSession;

    public MainViewModel(IWorkoutService workoutService, IDialogService dialogService)
    {
        _workoutService = workoutService;
        _dialogService = dialogService;
        Title = "Duofit - Workout Tracker";
    }

    /// <summary>
    /// Initialize the ViewModel by loading data
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadWorkoutSessionsAsync();
    }

    /// <summary>
    /// Load workout sessions asynchronously
    /// Uses async/await to prevent UI blocking
    /// </summary>
    [RelayCommand]
    private async Task LoadWorkoutSessionsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var sessions = await _workoutService.GetWorkoutSessionsAsync();
            WorkoutSessions.Clear();
            foreach (var session in sessions)
            {
                WorkoutSessions.Add(session);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlertAsync("Error", $"Failed to load workout sessions: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Add a new workout session
    /// </summary>
    [RelayCommand]
    private async Task AddWorkoutSessionAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            
            var newSession = new WorkoutSession
            {
                Name = "New Workout",
                Description = "Description of the workout",
                DurationMinutes = 30,
                SessionDate = DateTime.Today,
                Type = WorkoutType.Mixed
            };

            var createdSession = await _workoutService.CreateWorkoutSessionAsync(newSession);
            WorkoutSessions.Insert(0, createdSession);
            
            await _dialogService.DisplayAlertAsync("Success", "Workout session added successfully!", "OK");
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlertAsync("Error", $"Failed to add workout session: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Delete a workout session
    /// </summary>
    [RelayCommand]
    private async Task DeleteWorkoutSessionAsync(WorkoutSession session)
    {
        if (IsBusy || session == null)
            return;

        try
        {
            var confirm = await _dialogService.DisplayConfirmAsync(
                "Confirm Delete",
                $"Are you sure you want to delete '{session.Name}'?",
                "Yes",
                "No");

            if (!confirm)
                return;

            IsBusy = true;
            var success = await _workoutService.DeleteWorkoutSessionAsync(session.Id);
            
            if (success)
            {
                WorkoutSessions.Remove(session);
                await _dialogService.DisplayAlertAsync("Success", "Workout session deleted successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlertAsync("Error", $"Failed to delete workout session: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Refresh the workout sessions list
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadWorkoutSessionsAsync();
    }
}

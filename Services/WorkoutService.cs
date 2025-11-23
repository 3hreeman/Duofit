using Duofit.Models;

namespace Duofit.Services;

/// <summary>
/// Service for managing workout sessions
/// Implements business logic and data operations
/// </summary>
public interface IWorkoutService
{
    Task<List<WorkoutSession>> GetWorkoutSessionsAsync();
    Task<WorkoutSession?> GetWorkoutSessionByIdAsync(string id);
    Task<WorkoutSession> CreateWorkoutSessionAsync(WorkoutSession session);
    Task<WorkoutSession> UpdateWorkoutSessionAsync(WorkoutSession session);
    Task<bool> DeleteWorkoutSessionAsync(string id);
}

public class WorkoutService : IWorkoutService
{
    private const int SimulatedOperationDelayMs = 100;
    private const int SimulatedQuickOperationDelayMs = 50;
    
    private readonly List<WorkoutSession> _workoutSessions = new();

    public async Task<List<WorkoutSession>> GetWorkoutSessionsAsync()
    {
        // Simulate async operation
        await Task.Delay(SimulatedOperationDelayMs);
        return _workoutSessions.OrderByDescending(w => w.SessionDate).ToList();
    }

    public async Task<WorkoutSession?> GetWorkoutSessionByIdAsync(string id)
    {
        await Task.Delay(SimulatedQuickOperationDelayMs);
        return _workoutSessions.FirstOrDefault(w => w.Id == id);
    }

    public async Task<WorkoutSession> CreateWorkoutSessionAsync(WorkoutSession session)
    {
        await Task.Delay(SimulatedQuickOperationDelayMs);
        _workoutSessions.Add(session);
        return session;
    }

    public async Task<WorkoutSession> UpdateWorkoutSessionAsync(WorkoutSession session)
    {
        await Task.Delay(SimulatedQuickOperationDelayMs);
        var existingSession = _workoutSessions.FirstOrDefault(w => w.Id == session.Id);
        if (existingSession != null)
        {
            var index = _workoutSessions.IndexOf(existingSession);
            session.UpdatedAt = DateTime.UtcNow;
            _workoutSessions[index] = session;
        }
        return session;
    }

    public async Task<bool> DeleteWorkoutSessionAsync(string id)
    {
        await Task.Delay(SimulatedQuickOperationDelayMs);
        var session = _workoutSessions.FirstOrDefault(w => w.Id == id);
        if (session != null)
        {
            _workoutSessions.Remove(session);
            return true;
        }
        return false;
    }
}

namespace Duofit.Models;

/// <summary>
/// Represents a workout session
/// </summary>
public class WorkoutSession : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public DateTime SessionDate { get; set; } = DateTime.Today;
    public WorkoutType Type { get; set; }
}

public enum WorkoutType
{
    Cardio,
    Strength,
    Flexibility,
    Balance,
    Mixed
}

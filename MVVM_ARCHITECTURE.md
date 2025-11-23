# Duofit - .NET MAUI AI Assistant

## MVVM Architecture Guide

This project strictly follows the MVVM (Model-View-ViewModel) architecture pattern for .NET MAUI applications.

### Architecture Overview

```
┌─────────────────────────────────────────┐
│              View Layer                 │
│  (XAML + Code-behind for binding only) │
│         Views/MainPage.xaml             │
└──────────────┬──────────────────────────┘
               │ Data Binding
               ▼
┌─────────────────────────────────────────┐
│          ViewModel Layer                │
│   (Presentation Logic & Commands)       │
│    ViewModels/MainViewModel.cs          │
└──────────────┬──────────────────────────┘
               │ Uses
               ▼
┌─────────────────────────────────────────┐
│           Service Layer                 │
│      (Business Logic & Data)            │
│    Services/WorkoutService.cs           │
└──────────────┬──────────────────────────┘
               │ Uses
               ▼
┌─────────────────────────────────────────┐
│            Model Layer                  │
│         (Data Structures)               │
│      Models/WorkoutSession.cs           │
└─────────────────────────────────────────┘
```

### MVVM Principles Implemented

#### 1. View (XAML + Code-behind)
- **Location**: `Views/` folder
- **Purpose**: Defines the UI structure using XAML
- **Rules**:
  - ✅ ONLY XAML for UI definition
  - ✅ Data binding to ViewModel properties
  - ✅ Command binding for user actions
  - ❌ NO business logic in code-behind
  - ❌ NO data manipulation in code-behind
  - ❌ Code-behind ONLY for initialization and ViewModel binding

**Example**: `Views/MainPage.xaml`
```xaml
<Button Text="Add Workout"
        Command="{Binding AddWorkoutSessionCommand}"
        IsEnabled="{Binding IsNotBusy}" />
```

#### 2. ViewModel
- **Location**: `ViewModels/` folder
- **Purpose**: Manages View state and handles user interactions
- **Technology**: Uses `CommunityToolkit.Mvvm` for simplified MVVM implementation
- **Rules**:
  - ✅ All properties exposed for data binding (using `[ObservableProperty]`)
  - ✅ All user actions as ICommand (using `[RelayCommand]`)
  - ✅ Async/await for all operations to prevent UI blocking
  - ✅ Inherits from `ObservableObject` for property change notifications
  - ❌ NO direct UI manipulation
  - ❌ NO platform-specific code

**Example**: `ViewModels/MainViewModel.cs`
```csharp
[ObservableProperty]
private ObservableCollection<WorkoutSession> workoutSessions = new();

[RelayCommand]
private async Task AddWorkoutSessionAsync()
{
    // Business logic here
}
```

#### 3. Model
- **Location**: `Models/` folder
- **Purpose**: Represents data structures and business entities
- **Rules**:
  - ✅ Plain C# classes with properties
  - ✅ Data validation logic (if needed)
  - ✅ Business rules related to data
  - ❌ NO UI logic
  - ❌ NO direct database access

**Example**: `Models/WorkoutSession.cs`
```csharp
public class WorkoutSession : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public WorkoutType Type { get; set; }
}
```

#### 4. Services
- **Location**: `Services/` folder
- **Purpose**: Implements business logic and data operations
- **Rules**:
  - ✅ All methods are async
  - ✅ Implements interfaces for testability
  - ✅ Handles data persistence, API calls, etc.
  - ❌ NO UI dependencies

**Example**: `Services/WorkoutService.cs`
```csharp
public interface IWorkoutService
{
    Task<List<WorkoutSession>> GetWorkoutSessionsAsync();
    Task<WorkoutSession> CreateWorkoutSessionAsync(WorkoutSession session);
}
```

### Folder Structure

```
Duofit/
├── Models/                 # Data structures
│   ├── BaseModel.cs
│   └── WorkoutSession.cs
├── ViewModels/            # Presentation logic
│   ├── BaseViewModel.cs
│   └── MainViewModel.cs
├── Views/                 # UI (XAML)
│   ├── MainPage.xaml
│   └── MainPage.xaml.cs
├── Services/              # Business logic
│   └── WorkoutService.cs
├── Resources/             # Assets
│   ├── Fonts/
│   ├── Images/
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
├── Platforms/             # Platform-specific code
│   ├── Android/
│   ├── iOS/
│   ├── MacCatalyst/
│   └── Windows/
├── App.xaml              # Application resources
├── AppShell.xaml         # Navigation structure
└── MauiProgram.cs        # Dependency injection setup
```

### Key Technologies Used

1. **CommunityToolkit.Mvvm** (v8.2.2)
   - Simplifies MVVM implementation
   - Source generators for boilerplate code reduction
   - `[ObservableProperty]` attribute for automatic property change notification
   - `[RelayCommand]` attribute for automatic ICommand implementation

2. **Async/Await Pattern**
   - Prevents UI blocking
   - All service methods are async
   - All ViewModel commands that perform operations are async

3. **Dependency Injection**
   - Configured in `MauiProgram.cs`
   - Services registered as singletons or transient
   - ViewModels and Views registered for constructor injection

### Example: Adding a New Feature

To add a new feature following MVVM:

1. **Create Model** (`Models/NewFeature.cs`)
```csharp
public class NewFeature : BaseModel
{
    public string Name { get; set; } = string.Empty;
}
```

2. **Create Service Interface and Implementation** (`Services/INewFeatureService.cs`)
```csharp
public interface INewFeatureService
{
    Task<List<NewFeature>> GetFeaturesAsync();
}

public class NewFeatureService : INewFeatureService
{
    public async Task<List<NewFeature>> GetFeaturesAsync()
    {
        await Task.Delay(100); // Simulate async operation
        return new List<NewFeature>();
    }
}
```

3. **Create ViewModel** (`ViewModels/NewFeatureViewModel.cs`)
```csharp
public partial class NewFeatureViewModel : BaseViewModel
{
    private readonly INewFeatureService _service;
    
    [ObservableProperty]
    private ObservableCollection<NewFeature> features = new();
    
    public NewFeatureViewModel(INewFeatureService service)
    {
        _service = service;
    }
    
    [RelayCommand]
    private async Task LoadFeaturesAsync()
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            var items = await _service.GetFeaturesAsync();
            Features.Clear();
            foreach (var item in items)
                Features.Add(item);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

4. **Create View** (`Views/NewFeaturePage.xaml`)
```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:viewmodels="clr-namespace:Duofit.ViewModels"
             x:Class="Duofit.Views.NewFeaturePage"
             x:DataType="viewmodels:NewFeatureViewModel">
    
    <CollectionView ItemsSource="{Binding Features}">
        <!-- UI definition here -->
    </CollectionView>
</ContentPage>
```

5. **Register in DI Container** (`MauiProgram.cs`)
```csharp
builder.Services.AddSingleton<INewFeatureService, NewFeatureService>();
builder.Services.AddTransient<NewFeatureViewModel>();
builder.Services.AddTransient<NewFeaturePage>();
```

### Best Practices

1. **Always use async/await** for any operation that might take time
2. **Never block the UI thread** - use async commands
3. **Use ObservableCollection** for lists that change dynamically
4. **Implement IsBusy pattern** to show loading states
5. **Use dependency injection** for all services and ViewModels
6. **Keep Views pure** - no logic in code-behind
7. **Use Commands for all user actions** - no event handlers
8. **Use data binding** for all UI updates - no manual UI manipulation

### Common Mistakes to Avoid

❌ **Don't do this**:
```csharp
// In code-behind
private void Button_Clicked(object sender, EventArgs e)
{
    var data = LoadDataFromDatabase(); // ❌ Business logic in View
    myLabel.Text = data; // ❌ Direct UI manipulation
}
```

✅ **Do this instead**:
```csharp
// In ViewModel
[RelayCommand]
private async Task LoadDataAsync()
{
    Data = await _service.LoadDataAsync(); // ✅ In ViewModel
}

// In XAML
<Label Text="{Binding Data}" /> // ✅ Data binding
<Button Command="{Binding LoadDataCommand}" /> // ✅ Command binding
```

### Building and Running

Since MAUI workload is not available on this platform, this is a reference implementation. In a real development environment:

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run on specific platform
dotnet build -t:Run -f net8.0-android
dotnet build -t:Run -f net8.0-ios
dotnet build -t:Run -f net8.0-maccatalyst
```

### Summary

This Duofit project demonstrates a clean, maintainable MVVM architecture for .NET MAUI applications:

- **Separation of Concerns**: Clear boundaries between View, ViewModel, Model, and Services
- **Testability**: Services and ViewModels can be easily unit tested
- **Maintainability**: Changes in one layer don't affect others
- **Modern Practices**: Uses latest MVVM toolkit and async patterns
- **Scalability**: Easy to add new features following the established patterns

All code follows MVVM best practices with no business logic in Views, all data binding through ViewModels, and extensive use of async/await to prevent UI blocking.

using Duofit.Services;
using Duofit.ViewModels;
using Duofit.Views;
using Microsoft.Extensions.Logging;

namespace Duofit;

/// <summary>
/// MauiProgram - Application entry point
/// Configures services and MVVM dependencies
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services (Business Logic Layer)
        builder.Services.AddSingleton<IWorkoutService, WorkoutService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        // Register ViewModels (Presentation Logic Layer)
        builder.Services.AddTransient<MainViewModel>();

        // Register Views (UI Layer)
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

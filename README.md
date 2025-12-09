# Duofit
.NET MAUI Workout Tracker - AI-Driven Project with Strict MVVM Architecture

## Overview

Duofit is a cross-platform workout tracking application built with .NET MAUI following strict MVVM (Model-View-ViewModel) architecture principles.

## Architecture

This project implements a **complete MVVM architecture** with clear separation of concerns:

- **Views**: Pure XAML UI with NO code-behind logic
- **ViewModels**: Presentation logic using CommunityToolkit.Mvvm
- **Models**: Data structures and entities
- **Services**: Business logic and data operations

See [MVVM_ARCHITECTURE.md](MVVM_ARCHITECTURE.md) for detailed architecture documentation.

## Key Features

✅ **MVVM Compliant**: Strict adherence to MVVM patterns
✅ **Async/Await**: All operations use async/await to prevent UI blocking
✅ **Data Binding**: All UI updates through data binding
✅ **Command Pattern**: All user actions through ICommand
✅ **Dependency Injection**: Proper DI setup for services and ViewModels
✅ **CommunityToolkit.Mvvm**: Modern MVVM implementation with source generators

## Project Structure

```
Duofit/
├── Models/              # Data structures
├── ViewModels/          # Presentation logic
├── Views/               # XAML UI
├── Services/            # Business logic
├── Resources/           # Assets and styles
├── Platforms/           # Platform-specific code
└── MVVM_ARCHITECTURE.md # Architecture documentation
```

## Prerequisites

- .NET 8.0 SDK or later
- .NET MAUI workload installed
- Visual Studio 2022 (17.8+) or VS Code with MAUI extensions
- For development on macOS: Xcode and Android SDK
- For development on Windows: Windows SDK and Android SDK

## Installation

1. **Install .NET MAUI workload**:
```bash
dotnet workload install maui
```

2. **Clone the repository**:
```bash
git clone https://github.com/3hreeman/Duofit.git
cd Duofit
```

3. **Restore dependencies**:
```bash
dotnet restore
```

4. **Build the project**:
```bash
dotnet build
```

## Running the Application

### Android
```bash
dotnet build -t:Run -f net8.0-android
```

### iOS
```bash
dotnet build -t:Run -f net8.0-ios
```

### macOS (Mac Catalyst)
```bash
dotnet build -t:Run -f net8.0-maccatalyst
```

### Windows
```bash
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## Development Guidelines

### MVVM Rules

1. **Views** (XAML files):
   - ❌ NO business logic in code-behind
   - ✅ ONLY data binding to ViewModel
   - ✅ ONLY command binding for actions

2. **ViewModels**:
   - ✅ Use `[ObservableProperty]` for bindable properties
   - ✅ Use `[RelayCommand]` for commands
   - ✅ All operations must be async
   - ❌ NO direct UI manipulation

3. **Models**:
   - ✅ Plain data structures
   - ❌ NO UI dependencies

4. **Services**:
   - ✅ All methods must be async
   - ✅ Implement interfaces for testability
   - ❌ NO UI dependencies

### Adding New Features

See the [MVVM_ARCHITECTURE.md](MVVM_ARCHITECTURE.md) file for step-by-step guide on adding new features.

## Dependencies

- **Microsoft.Maui.Controls**: Core MAUI framework
- **CommunityToolkit.Mvvm** (v8.2.2): MVVM toolkit with source generators
- **Microsoft.Extensions.Logging.Debug**: Debug logging

## Testing

(Tests to be implemented following the MVVM structure)

```bash
dotnet test
```

## Contributing

When contributing to this project, please ensure:

1. Follow MVVM architecture strictly
2. Use async/await for all operations
3. Add XML documentation comments
4. Use meaningful variable and method names
5. Follow C# coding conventions

## License

(License information to be added)

## Contact

For questions or support, please open an issue on GitHub.

---

**Note**: This is an AI-driven project demonstrating best practices for .NET MAUI MVVM architecture.

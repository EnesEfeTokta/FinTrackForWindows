# FinTrack Development Guide

This guide provides detailed information for developers working on the FinTrack project.

## Project Overview

FinTrack is a modern personal finance management application built with WPF and .NET 8, following the MVVM architectural pattern.

## Quick Start

### Prerequisites
- Windows 10/11 (for WPF support)
- Visual Studio 2022 with WPF workload
- .NET 8.0 SDK

### Setup
1. Clone the repository
2. Run the setup script: `setup.bat` (Windows) or `setup.sh` (Unix)
3. Open `FinTrackForWindows.sln` in Visual Studio 2022
4. Build and run (F5)

## Project Structure

```
FinTrackForWindows/
├── FinTrackForWindows.sln        # Root solution file
├── setup.bat / setup.sh          # Project setup scripts
├── project.json                  # Project configuration
├── FinTrack/                     # Main application
│   ├── Core/                     # Core logic and base classes
│   ├── Data/                     # Entity Framework DbContext
│   ├── Dtos/                     # Data Transfer Objects
│   ├── Enums/                    # Application enumerations
│   ├── Helpers/                  # UI helpers and converters
│   ├── Models/                   # Domain models and entities
│   ├── Services/                 # Business logic services
│   ├── ViewModels/               # MVVM ViewModels
│   ├── Views/                    # WPF Windows and UserControls
│   ├── Styles/                   # XAML styles and resources
│   └── App.xaml.cs               # Application entry point
├── README.md                     # Project documentation
└── LICENSE                       # MIT License
```

## Architecture

### MVVM Pattern
The application follows the Model-View-ViewModel (MVVM) pattern:

- **Models**: Domain entities and data structures (`Models/`)
- **Views**: WPF UI components (`Views/`)
- **ViewModels**: Presentation logic and data binding (`ViewModels/`)

### Key Technologies
- **.NET 8.0**: Modern cross-platform framework
- **WPF**: Windows Presentation Foundation for rich UI
- **Entity Framework Core**: Object-relational mapping
- **SQLite**: Local database storage
- **CommunityToolkit.Mvvm**: MVVM framework
- **LiveCharts2**: Data visualization
- **Serilog**: Structured logging

## Development Workflow

### Building the Project
```bash
# Restore packages
dotnet restore FinTrack/FinTrackForWindows.csproj

# Build the project
dotnet build FinTrack/FinTrackForWindows.csproj

# Run the application (in Visual Studio)
# Press F5 or use the Start button
```

### Database Migrations
The application uses Entity Framework Core with SQLite:

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project FinTrack

# Update the database
dotnet ef database update --project FinTrack
```

### Adding New Features

1. **Create Model** (if needed): Add to `Models/`
2. **Create DTO** (if needed): Add to `Dtos/`
3. **Create Service**: Add business logic to `Services/`
4. **Create ViewModel**: Add presentation logic to `ViewModels/`
5. **Create View**: Add UI to `Views/`
6. **Register Services**: Update dependency injection in `App.xaml.cs`

### Code Style
- Follow C# naming conventions
- Use meaningful variable and method names
- Document public APIs with XML comments
- Keep methods focused and concise

## Key Features

### Financial Management
- Transaction tracking and categorization
- Budget management with alerts
- Account management
- Currency support

### Reporting
- Visual dashboards with charts
- Customizable reports
- Export capabilities (PDF, Excel)

### Security
- JWT-based authentication
- Secure local data storage
- Password protection

## Troubleshooting

### Common Issues

1. **Build fails on non-Windows platforms**
   - This is expected - WPF requires Windows
   - Use Windows or Windows VM for development

2. **Package restore issues**
   - Run `dotnet restore` manually
   - Clear NuGet cache: `dotnet nuget locals all --clear`

3. **Database issues**
   - Delete the local SQLite file to reset
   - Check Entity Framework migrations

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Testing

### Unit Tests
Currently, the project doesn't have automated tests. Consider adding:
- xUnit for unit testing
- Moq for mocking
- FluentAssertions for readable assertions

### Manual Testing
1. Run the application
2. Test key user workflows
3. Verify database operations
4. Test export functionality

## Performance Considerations

- Use async/await for database operations
- Implement lazy loading for large datasets
- Optimize XAML binding performance
- Profile memory usage for large datasets

## Deployment

The application is designed for local deployment:
1. Build in Release configuration
2. Publish with `dotnet publish`
3. Include .NET 8 runtime if needed
4. Distribute as a Windows application

---

For questions or support, please open an issue on GitHub.
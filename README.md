# FinTrack - WPF Personal Finance Tracker

FinTrack is a modern, user-friendly desktop application for personal finance management, built with WPF and .NET 8. It helps you track income, expenses, and investments to achieve your financial goals.

![License](https://img.shields.io/badge/license-GPL-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Status](https://img.shields.io/badge/status-In%20Development-yellow.svg)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology-Stack](#technology-stack)
- [Project-Structure](#project-structure)
- [Getting-Started](#getting-started)
- [Contributing](#contributing)
- [License](#license)

## Overview

FinTrack provides a comprehensive solution for tracking daily financial transactions, creating budgets, and generating insightful reports. It is developed using C# with a modern .NET 8 stack, utilizing a local SQLite database for secure data storage. The application follows the MVVM design pattern for a clean and maintainable codebase.

Please click on the image to watch the video on YouTube.

[<img src="https://t.ctcdn.com.br/gbO3hsV5DRUS3MYFIL0-vgDJtYk=/640x360/smart/i533291.png" width="50%">](https://youtu.be/0r24kuKRQ6A)

### Design Philosophy

-   **Simplicity**: An intuitive and straightforward user interface.
-   **Reliability**: Ensuring the security and integrity of your financial data.
-   **Performance**: A fast and responsive user experience.
-   **Modularity**: A clean architecture that is easy to extend.

## Features

### Core Features

-  **Transaction Tracking**: Categorize and monitor all your financial activities.
-  **Smart Reporting**: Generate reports to analyze financial trends.
-  **Budget Management**: Set budget limits for categories with alerts.
-  **Visual Analysis**: Interactive charts and a dashboard to visualize spending.
-  **Secure Local Storage**: Data is stored locally in an SQLite database.
-  **Modern UI**: A clean and responsive user interface built with WPF.

### Technical Features

-   **Modern Architecture**: Developed on **.NET 8**.
-   **MVVM Pattern**: Clean and separated code structure using **CommunityToolkit.Mvvm**.
-   **Database**: **Entity Framework Core** with a local **SQLite** database.
-   **Rich UI**: Built with **WPF** for a flexible user experience.
-   **Charting**: Interactive data visualizations with **LiveCharts2**.
-   **Logging**: Comprehensive logging with **Serilog**.
-   **Authentication**: JWT-based authentication for security.

## Technology Stack

### Backend & Core
-   **Framework**: .NET 8.0
-   **ORM**: Entity Framework Core
-   **Database**: SQLite
-   **MVVM Framework**: CommunityToolkit.Mvvm
-   **Logging**: Serilog
-   **JSON Serialization**: Newtonsoft.Json
-   **Authentication**: System.IdentityModel.Tokens.Jwt

### Frontend
-   **UI Framework**: WPF (Windows Presentation Foundation)
-   **Charting**: LiveChartsCore.SkiaSharpView.WPF
-   **Styling**: Custom modern styles (see `Styles/ModernStyles.xaml`)

### Development Tools
-   **IDE**: Visual Studio 2022
-   **Package Manager**: NuGet
-   **Version Control**: Git & GitHub
-   **CI/CD**: GitHub Actions

## Project Structure

The project is organized into a single solution with a clear folder structure following the MVVM pattern.

```
FinTrack/
├── Core/         # Core logic, services, and base classes (e.g., RelayCommand, SessionManager).
├── Data/         # Entity Framework DbContext.
├── Dtos/         # Data Transfer Objects for API/UI communication.
├── Enums/        # Application-specific enumerations.
├── Helpers/      # Converters and UI assistants (e.g., PasswordBoxAssistant).
├── Models/       # Domain models and entities.
├── Services/     # Business logic services (e.g., AuthService, ApiService).
├── ViewModels/   # ViewModels for each View, containing presentation logic.
├── Views/        # WPF Windows and UserControls (the UI).
├── Styles/       # XAML styles and resources.
├── App.xaml.cs   # Application entry point and setup.
└── FinTrack.csproj # Project file with dependencies.
```

## Getting Started

### Prerequisites

-   .NET 8.0 SDK
-   Visual Studio 2022 (with WPF workload)
-   Git

### Installation & Running

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/EnesEfeTokta/FinTrack.git
    cd FinTrack
    ```

2.  **Open in Visual Studio:**
    Open the `FinTrack/FinTrack.sln` file in Visual Studio.

3.  **Restore Dependencies:**
    Visual Studio should automatically restore the NuGet packages. If not, open the Package Manager Console and run:
    ```powershell
    Update-Package -reinstall
    ```

4.  **Run the Application:**
    Press `F5` or click the "Start" button in Visual Studio to build and run the project. The application will create and use an SQLite database in its local data directory.

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute, please follow these steps:

1.  **Fork** the repository.
2.  Create a new **feature branch** (`git checkout -b feature/YourAmazingFeature`).
3.  **Commit** your changes (`git commit -m 'Add some AmazingFeature'`).
4.  **Push** to the branch (`git push origin feature/YourAmazingFeature`).
5.  Open a **Pull Request**.

Please ensure your code adheres to the existing style and that you provide clear descriptions for your changes.

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.
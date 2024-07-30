# MudRoles

## Overview
MudRoles is a sample project demonstrating the use of Blazor with Identity and roles, integrated with MudBlazor for UI components. This project includes a Web API protected by role-based authorization and showcases how to filter data based on user roles.

## Features
- **Blazor Server**: Interactive web UI with Blazor.
- **MudBlazor**: Modern UI components for Blazor.
- **ASP.NET Core Identity**: User authentication and role management.
- **Role-Based Authorization**: Admin and User roles with specific access permissions.
- **Web API**: Protected endpoints with role-based filtering.

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or later with ASP.NET and web development workload.

## Getting Started

### Installation
1. **Clone the repository**:

```git clone https://github.com/LogneBudo/MudRoles.git
cd MudRoles```

2. **Restore NuGet packages**:

	<code>dotnet restore</code>

3. **Update the database**:

```dotnet ef database update```

### Configuration
1. **AppSettings**: Ensure your `appsettings.json` is configured correctly for your database and authentication settings.

### Running the Application
1. **Run the application**:

```dotnet run```


2. **Open in browser**: Navigate to `https://localhost:5001` to see the application in action.

## Project Structure
- **Client**: Blazor Server application with MudBlazor components.
- **Server**: ASP.NET Core Web API with Identity and role-based authorization.
- **Shared**: Shared models and services between Client and Server.

## Roles and Authorization
- **Admin Role**: Full access to all resources and administrative functionalities.
- **User Role**: Limited access to user-specific resources.

### Default Roles
Upon registration, new users are automatically assigned the "User" role. The "Admin" role must be assigned manually.

## Useful Links
- [MudBlazor Documentation](https://mudblazor.com/)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## Contributing
Contributions are welcome! Please open an issue or submit a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For any questions or feedback, please contact [Djovani](mailto:anupjitamang@gmail.com).


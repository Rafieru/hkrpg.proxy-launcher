# HKRPG Proxy-Launcher

A proxy tool for Anime Game with a simple Avalonia UI interface. Inspired by [Cultivation: Thorny Edition](https://github.com/Grasscutters/Cultivation)

![HKRPG Proxy Interface](https://i.imgur.com/tVEtr2K.png)

## Features

- 🔧 Easy-to-use modern graphical interface with Avalonia UI
- 🌐 Proxy configuration with IP and port settings
- 🖥️ Localhost mode for local development
- 📜 Connection logging with different log levels (debug, info, warning, error)
- 🎮 Game process monitoring and management
- 🚀 Automatic proxy startup and shutdown
- 🔒 Self-contained application (no dependencies needed)

## Installation

### Option 1: Download from Releases

1. Go to the [Releases](https://github.com/Rafieru/hkrpg.proxy-launcher/releases) page
2. Download the latest release zip file
3. Extract the zip file to your preferred location
4. Run `HkrpgProxy.Launcher.exe`

### Option 2: Build from Source

#### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10 or later

#### Build Steps

1. Clone the repository:
```bash
git clone https://github.com/Rafieru/hkrpg.proxy-launcher.git
cd hkrpg-proxy
```

2. Build the project:
```bash
dotnet publish -c Release -r win-x64
```

## Usage

### For Localhost Users (Running with Local Server)

1. Place the launcher in the same directory as your server files:
   - For single server: Place alongside `server.exe` or `*.jar`
   - For dual servers: Place alongside `gameserver.exe` and `sdkserver.exe`
2. Launch the application
3. Check "Use Localhost" option
4. Click "Start Server" button to launch your server
5. Set your game path by clicking the "Browse" button
6. Click "Launch" to start the game with proxy
7. (Optional) Enable connection logs to monitor traffic
8. (Optional) Enable debug logs for detailed monitoring

### For Remote Server Users

1. Launch the application
2. Uncheck the "Use Localhost" checkbox in the interface
3. Enter the server IP address and port
4. Set your game path by clicking the "Browse" button
5. Click "Launch" to start the game with proxy
6. (Optional) Enable connection logs to monitor traffic
7. (Optional) Enable debug logs for detailed monitoring

### Proxy Features

- **Domain Filtering**: Built-in lists for redirecting and blocking specific domains for 'otaku saves the world' game
- **URL Filtering**: Automatic blocking of telemetry and analytics endpoints
- **SSL/TLS Support**: Automatic handling of HTTPS connections
- **Process Monitoring**: Automatic proxy shutdown when game exits

## Technical Details

The project uses the following main technologies:
- Unobtanium.Web.Proxy for proxy functionality
- Avalonia UI for the cross-platform user interface
- .NET 8.0 framework
- MVVM architecture with CommunityToolkit.Mvvm

### Optimizations

The application has been optimized for minimal file size while maintaining full functionality:
- Full trimming enabled to remove unused code
- Compressed single-file deployment
- Disabled unnecessary .NET features
- Optimized for size with ILLink trimming
- Includes only the essential assemblies needed for operation

### Project Structure

- `ViewModels/`: MVVM pattern view models
- `Views/`: Avalonia UI views
- `src/HkrpgProxy.Core/`: Core proxy functionality
- `Models/`: Data models and configurations

## Troubleshooting

- If the proxy fails to initialize, check if the port is available
- Make sure to run the application with administrator privileges
- Check the connection logs for detailed error messages
- Verify that the game path is correctly set
- If SSL/TLS issues occur, check the certificate handling
- For localhost users: Ensure server files are in the same directory as the launcher
- For localhost users: Start the server before launching the game

## License

This project is licensed under the terms of the [LICENSE](LICENSE) file in the root of this repository.

## Acknowledgments

- [Unobtanium.Web.Proxy](https://github.com/svrooij/Unobtanium.Web.Proxy) for the proxy functionality
- [Avalonia UI](https://avaloniaui.net/) for the cross-platform UI framework
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) for the MVVM implementation
- [FireflySR.Tool.Proxy](https://git.xeondev.com/YYHEggEgg/FireflySR.Tool.Proxy) for the codebase

# HKRPG Proxy

A proxy tool for Anime Game with a simple Windows Forms interface. Inspired by [Cultivation: Thorny Edition](https://github.com/Grasscutters/Cultivation)

## Features

- 🔧 Easy-to-use graphical interface
- 🌐 Proxy configuration with IP and port settings
- 🖥️ Localhost mode for local development
- 📜 Connection logging (toggle on/off)
- 🎮 Game process monitoring and management
- 🚀 Automatic proxy startup and shutdown

## Installation

### Option 1: Download from Releases

1. Go to the [Releases](https://github.com/yourusername/hkrpg-proxy/releases) page
2. Download the latest release zip file
3. Extract the zip file to your preferred location
4. Run `hkrpg.proxy-launcher.exe`

### Option 2: Build from Source

#### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10 or later

#### Build Steps

1. Clone the repository:
```bash
git clone https://github.com/yourusername/hkrpg-proxy.git
cd hkrpg-proxy
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run
```

## Usage

1. Launch the application
2. Set your game path by clicking the "Browse" button
3. Configure proxy settings:
   - Check "Use Localhost" for local development (127.0.0.1)
   - Or enter a custom IP and port
4. Click "Save Settings" to apply proxy configuration
5. Click "Launch" to start the game with proxy
6. (Optional) Enable connection logs to monitor traffic

## Development

The project uses the following main dependencies:
- Titanium.Web.Proxy for proxy functionality
- Windows Forms for the user interface
- .NET 8.0 framework

To contribute:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Troubleshooting

- If the proxy fails to initialize, check if the port is available
- Make sure to run the application with administrator privileges
- Check the connection logs for detailed error messages
- Verify that the game path is correctly set

## License

[Add your license information here]

## Acknowledgments

- [Titanium.Web.Proxy](https://github.com/justcoding121/Titanium-Web-Proxy) for the proxy functionality
- [FireflySR.Tool.Proxy](https://git.xeondev.com/YYHEggEgg/FireflySR.Tool.Proxy) for the codebase

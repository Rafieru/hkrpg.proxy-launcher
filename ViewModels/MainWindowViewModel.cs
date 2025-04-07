using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HkrpgProxy.Avalonia.Models;
using HkrpgProxy.Core;
using System.Diagnostics;
using System.Timers;
using System.Collections.ObjectModel;

namespace HkrpgProxy.Avalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IniFile _config;
        private string _lastIpAddress = "127.0.0.1";
        private ProxyService? _proxyService;
        private Process? _gameProcess;
        private Process? _serverProcess;
        private Process? _sdkServerProcess;
        private System.Timers.Timer? _processCheckTimer;
        private bool IsInExitedEventHandler = false;

        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            HTTP
        }

        [ObservableProperty]
        private string _gamePath = string.Empty;

        [ObservableProperty]
        private bool _useLocalhost;

        [ObservableProperty]
        private string _ipAddress = "127.0.0.1";

        [ObservableProperty]
        private string _port = "21000";

        [ObservableProperty]
        private string _status = "Ready";

        [ObservableProperty]
        private bool _enableLogs;

        [ObservableProperty]
        private bool _showDebugLogs;

        [ObservableProperty]
        private string _logs = string.Empty;

        [ObservableProperty]
        private bool _isGameRunning;

        [ObservableProperty]
        private bool _isServerRunning;

        [ObservableProperty]
        private bool _isProxyRunning;

        [ObservableProperty]
        private bool _isIpAddressEnabled;

        public string ServerButtonText => IsServerRunning ? "Stop Server" : "Start Server";

        private Window? _window;

        public class LogEntry
        {
            public required string Timestamp { get; set; }
            public required string Message { get; set; }
            public required LogLevel Level { get; set; }
            public string Color => Level switch
            {
                LogLevel.DEBUG => "Gray",
                LogLevel.INFO => "Green",
                LogLevel.WARN => "Orange",
                LogLevel.ERROR => "Red",
                LogLevel.HTTP => "Blue",
                _ => "Black"
            };
        }

        private ObservableCollection<LogEntry> _logEntries = new();
        public ObservableCollection<LogEntry> LogEntries => _logEntries;

        public MainWindowViewModel()
        {
            _useLocalhost = true; // Set initial value before anything else
            var configPath = Path.Combine(GetExecutableDirectory(), "launcher.ini");
            _config = new IniFile(configPath);
            LoadSettings();
            InitializeProcessCheckTimer();
            UpdateIpAddressEnabled();
            SaveSettingsInternal(false); // Save initial settings
            
            // Check for running servers on startup
            CheckForRunningServers();

            // Register cleanup on application exit
            AppDomain.CurrentDomain.ProcessExit += (s, e) => CleanupOnExit();
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        private void LoadSettings()
        {
            try
            {
                GamePath = _config.Read("Settings", "GamePath", "");
                // Only override UseLocalhost if explicitly set to false in config
                string useLocalhostStr = _config.Read("Settings", "UseLocalhost", "true");
                if (useLocalhostStr.ToLower() == "false")
                {
                    UseLocalhost = false;
                }
                string customIp = _config.Read("Settings", "CustomDestinationHost", "127.0.0.1");
                IpAddress = UseLocalhost ? "Localhost" : customIp;
                Port = _config.Read("Settings", "DestinationPort", "21000");
                
                Log("Settings loaded", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Error loading settings: {ex.Message}", LogLevel.ERROR);
            }
        }

        private void UpdateIpAddressEnabled()
        {
            IsIpAddressEnabled = !UseLocalhost && !IsProxyRunning;
        }

        private void UpdateUIState(bool isRunning)
        {
            IsProxyRunning = isRunning;
            UpdateIpAddressEnabled();
            OnPropertyChanged(nameof(IsProxyRunning));
        }

        public void Log(string message, LogLevel level = LogLevel.INFO)
        {
            if (!EnableLogs) return;
            if (level == LogLevel.DEBUG && !ShowDebugLogs) return;

            var entry = new LogEntry
            {
                Timestamp = DateTime.Now.ToString("HH:mm:ss"),
                Message = message,
                Level = level
            };

            _logEntries.Insert(0, entry);
            OnPropertyChanged(nameof(LogEntries));
        }

        private void InitializeProxy()
        {
            try
            {
                var config = new ProxyConfig
                {
                    DestinationHost = UseLocalhost ? "127.0.0.1" : IpAddress,
                    DestinationPort = int.Parse(Port),
                    LastGamePath = GamePath
                };

                _proxyService = new ProxyService(UseLocalhost ? "127.0.0.1" : IpAddress, int.Parse(Port), config);
                
                // Subscribe to proxy events for logging
                _proxyService.ProxyServer.BeforeRequest += OnBeforeRequest;
                _proxyService.ProxyServer.ServerCertificateValidationCallback += OnCertificateValidation;

                UpdateUIState(true);
                Log($"Proxy initialized with destination: {(UseLocalhost ? "127.0.0.1" : IpAddress)}:{Port}", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Failed to initialize proxy: {ex.Message}", LogLevel.ERROR);
                throw;
            }
        }

        private async Task OnBeforeRequest(object sender, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
        {
            string url = e.HttpClient.Request.Url;
            string method = e.HttpClient.Request.Method;
            string formattedUrl = url.Length > 80 ? url.Substring(0, 77) + "..." : url;
            await Task.Run(() => Log($"[{method}] {formattedUrl}", LogLevel.HTTP));
        }

        private async Task OnCertificateValidation(object sender, Titanium.Web.Proxy.EventArguments.CertificateValidationEventArgs e)
        {
            e.IsValid = true;
            await Task.CompletedTask;
        }

        private void InitializeProcessCheckTimer()
        {
            _processCheckTimer = new System.Timers.Timer(1000); // Check every second
            _processCheckTimer.Elapsed += ProcessCheckTimer_Tick;
            _processCheckTimer.Start();
        }

        private void ProcessCheckTimer_Tick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            bool gameExited = false;

            try
            {
                if (_gameProcess != null)
                {
                    try
                    {
                        if (!ProcessExists(_gameProcess.Id))
                        {
                            gameExited = true;
                            Log("Game process not found by ID", LogLevel.DEBUG);
                        }
                        else
                        {
                            try
                            {
                                gameExited = _gameProcess.HasExited;
                                Log($"Game process check: HasExited={gameExited}", LogLevel.DEBUG);
                            }
                            catch (InvalidOperationException)
                            {
                                gameExited = true;
                                Log("Game process check failed with InvalidOperationException", LogLevel.DEBUG);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Game process check error: {ex.Message}", LogLevel.DEBUG);
                        gameExited = true;
                    }

                    if (gameExited)
                    {
                        Log("Game process marked as exited, cleaning up", LogLevel.DEBUG);
                        _gameProcess = null;
                        IsGameRunning = false;
                        Status = "Game stopped";
                        Log("Client disconnected", LogLevel.INFO);
                        
                        // Shutdown proxy when game exits
                        ShutdownProxy();
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Unexpected error in process check: {ex.GetType().Name} - {ex.Message}", LogLevel.ERROR);
                Log($"Stack trace: {ex.StackTrace}", LogLevel.DEBUG);
            }
        }

        private bool ProcessExists(int processId)
        {
            try
            {
                using (var process = Process.GetProcessById(processId))
                {
                    return true;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        [RelayCommand]
        private void LaunchGame()
        {
            if (string.IsNullOrEmpty(GamePath) || !File.Exists(GamePath))
            {
                Status = "Error: Game path is invalid";
                Log("Invalid game path", LogLevel.ERROR);
                return;
            }

            try
            {
                // Initialize and start proxy
                InitializeProxy();

                // Start game process
                _gameProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = GamePath,
                        UseShellExecute = true
                    }
                };
                
                _gameProcess.Start();
                IsGameRunning = true;
                Status = "Game running";
                Log("Game launched", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Status = $"Error: {ex.Message}";
                Log($"Failed to launch game: {ex.Message}", LogLevel.ERROR);
                ShutdownProxy();
                IsGameRunning = false;
            }
        }

        private void ShutdownProxy()
        {
            if (_proxyService != null)
            {
                try
                {
                    _proxyService.ProxyServer.BeforeRequest -= OnBeforeRequest;
                    _proxyService.ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                    _proxyService.Shutdown();
                    _proxyService = null;
                    UpdateUIState(false);
                    Log("Proxy shutdown", LogLevel.INFO);
                }
                catch (Exception ex)
                {
                    Log($"Failed to shutdown proxy: {ex.Message}", LogLevel.ERROR);
                }
            }
        }

        [RelayCommand]
        private void StopGame()
        {
            try
            {
                if (_gameProcess != null && !_gameProcess.HasExited)
                {
                    _gameProcess.Kill();
                    _gameProcess = null;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to stop game: {ex.Message}", LogLevel.ERROR);
            }
            finally
            {
                ShutdownProxy();
                IsGameRunning = false;
                Status = "Ready";
                Log("Game stopped", LogLevel.INFO);
            }
        }

        private string GetExecutableDirectory()
        {
            try
            {
                // Get the original executable path, not the temporary extraction path
                string exePath;
                
                // First try to get from process - this is the most reliable
                using (var process = Process.GetCurrentProcess())
                {
                    exePath = process.MainModule?.FileName ?? string.Empty;
                }
                
                if (string.IsNullOrEmpty(exePath))
                {
                    // Fallback to assembly location - might be temp path for single file
                    exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    
                    // If it's in temp, try to get from environment
                    if (exePath.Contains("\\Temp\\") || exePath.Contains("/Temp/"))
                    {
                        exePath = Environment.GetCommandLineArgs()[0];
                    }
                }
                
                var directory = Path.GetDirectoryName(exePath) ?? string.Empty;
                Log($"Original executable directory: {directory}", LogLevel.DEBUG);
                return directory;
            }
            catch (Exception ex)
            {
                Log($"Error getting executable directory: {ex.Message}", LogLevel.ERROR);
                return AppContext.BaseDirectory;
            }
        }

        private void CheckForRunningServers()
        {
            try
            {
                string originalDirectory = GetExecutableDirectory();
                Log($"Checking for running servers in temp directory: {AppContext.BaseDirectory}", LogLevel.DEBUG);
                Log($"Checking for running servers in original directory: {originalDirectory}", LogLevel.DEBUG);
                
                // First check in original directory
                if (CheckForRunningServersInDirectory(originalDirectory))
                {
                    return;
                }
                
                // Then check in temp directory
                CheckForRunningServersInDirectory(AppContext.BaseDirectory);
            }
            catch (Exception ex)
            {
                Log($"Error checking for running servers: {ex.Message}", LogLevel.ERROR);
                Log($"Stack trace: {ex.StackTrace}", LogLevel.DEBUG);
            }
        }
        
        private bool CheckForRunningServersInDirectory(string directory)
        {
            try
            {
                // Check for server.exe
                string serverPath = Path.Combine(directory, "server.exe");
                Log($"Checking for server.exe at: {serverPath}", LogLevel.DEBUG);
                if (File.Exists(serverPath))
                {
                    string processName = Path.GetFileNameWithoutExtension(serverPath);
                    Log($"Found server.exe, looking for process: {processName}", LogLevel.DEBUG);
                    Process[] processes = Process.GetProcessesByName(processName);
                    Log($"Found {processes.Length} processes with name {processName}", LogLevel.DEBUG);
                    if (processes.Length > 0)
                    {
                        _serverProcess = processes[0];
                        IsServerRunning = true;
                        OnPropertyChanged(nameof(ServerButtonText));
                        Log("Found running server process", LogLevel.INFO);
                        return true;
                    }
                }

                // Check for gameserver.exe and sdkserver.exe
                string gameServerPath = Path.Combine(directory, "gameserver.exe");
                string sdkServerPath = Path.Combine(directory, "sdkserver.exe");
                Log($"Checking for game server at: {gameServerPath}", LogLevel.DEBUG);
                Log($"Checking for SDK server at: {sdkServerPath}", LogLevel.DEBUG);
                
                if (File.Exists(gameServerPath) && File.Exists(sdkServerPath))
                {
                    string gameProcessName = Path.GetFileNameWithoutExtension(gameServerPath);
                    string sdkProcessName = Path.GetFileNameWithoutExtension(sdkServerPath);
                    Log($"Found both server files, looking for processes: {gameProcessName} and {sdkProcessName}", LogLevel.DEBUG);
                    
                    Process[] gameProcesses = Process.GetProcessesByName(gameProcessName);
                    Process[] sdkProcesses = Process.GetProcessesByName(sdkProcessName);
                    Log($"Found {gameProcesses.Length} game server processes and {sdkProcesses.Length} SDK server processes", LogLevel.DEBUG);
                    
                    if (gameProcesses.Length > 0 && sdkProcesses.Length > 0)
                    {
                        _serverProcess = gameProcesses[0];
                        _sdkServerProcess = sdkProcesses[0];
                        IsServerRunning = true;
                        OnPropertyChanged(nameof(ServerButtonText));
                        Log("Found running game server and SDK server processes", LogLevel.INFO);
                        return true;
                    }
                }

                // Check for .jar files
                string[] jarFiles = Directory.GetFiles(directory, "*.jar");
                Log($"Found {jarFiles.Length} .jar files in directory", LogLevel.DEBUG);
                if (jarFiles.Length > 0)
                {
                    string jarProcessName = Path.GetFileNameWithoutExtension(jarFiles[0]);
                    Log($"Looking for Java process running: {jarProcessName}", LogLevel.DEBUG);
                    Process[] processes = Process.GetProcessesByName("java");
                    Log($"Found {processes.Length} Java processes", LogLevel.DEBUG);
                    foreach (Process process in processes)
                    {
                        try
                        {
                            if (process.MainModule?.FileName?.Contains(jarProcessName) == true)
                            {
                                _serverProcess = process;
                                IsServerRunning = true;
                                OnPropertyChanged(nameof(ServerButtonText));
                                Log("Found running Java server process", LogLevel.INFO);
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"Error checking Java process: {ex.Message}", LogLevel.DEBUG);
                            continue;
                        }
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Log($"Error checking for running servers in directory {directory}: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        [RelayCommand]
        private void StartServer()
        {
            if (IsServerRunning)
            {
                Log("Server is already running, stopping it", LogLevel.DEBUG);
                StopServer();
                return;
            }

            try
            {
                string originalDirectory = GetExecutableDirectory();
                Log($"Starting server from temp directory: {AppContext.BaseDirectory}", LogLevel.DEBUG);
                Log($"Starting server from original directory: {originalDirectory}", LogLevel.DEBUG);
                
                // First check original directory
                if (TryStartServerFromDirectory(originalDirectory))
                {
                    return;
                }
                
                // Then check temp directory
                if (TryStartServerFromDirectory(AppContext.BaseDirectory))
                {
                    return;
                }
                
                Log("No valid server executables found in any location", LogLevel.ERROR);
                Status = "Error: No server files found";
            }
            catch (Exception ex)
            {
                Log($"Failed to start server: {ex.Message}", LogLevel.ERROR);
                Log($"Stack trace: {ex.StackTrace}", LogLevel.DEBUG);
                Status = $"Error: {ex.Message}";
            }
        }
        
        private bool TryStartServerFromDirectory(string directory)
        {
            try
            {
                string serverPath = Path.Combine(directory, "server.exe");
                Log($"Checking for server.exe at: {serverPath}", LogLevel.DEBUG);
                if (File.Exists(serverPath))
                {
                    Log("Found server.exe, starting single server", LogLevel.DEBUG);
                    StartSingleServer(serverPath);
                    return true;
                }

                string gameServerPath = Path.Combine(directory, "gameserver.exe");
                string sdkServerPath = Path.Combine(directory, "sdkserver.exe");
                Log($"Checking for game server at: {gameServerPath}", LogLevel.DEBUG);
                Log($"Checking for SDK server at: {sdkServerPath}", LogLevel.DEBUG);

                if (File.Exists(gameServerPath) && File.Exists(sdkServerPath))
                {
                    Log("Found both server files, starting dual servers", LogLevel.DEBUG);
                    StartDualServers(gameServerPath, sdkServerPath);
                    return true;
                }

                string[] jarFiles = Directory.GetFiles(directory, "*.jar");
                Log($"Found {jarFiles.Length} .jar files in directory", LogLevel.DEBUG);
                if (jarFiles.Length > 0)
                {
                    Log($"Starting Java server from: {jarFiles[0]}", LogLevel.DEBUG);
                    StartSingleServer(jarFiles[0]);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Log($"Error trying to start server from directory {directory}: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        private void StartSingleServer(string serverPath)
        {
            try
            {
                ProcessStartInfo startInfo;
                if (serverPath.EndsWith(".jar"))
                {
                    startInfo = new ProcessStartInfo
                    {
                        FileName = "java",
                        Arguments = $"-jar \"{serverPath}\"",
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                }
                else
                {
                    startInfo = new ProcessStartInfo
                    {
                        FileName = serverPath,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                }

                _serverProcess = new Process { StartInfo = startInfo };
                _serverProcess.EnableRaisingEvents = true;
                _serverProcess.Exited += ServerProcess_Exited;
                _serverProcess.Start();

                IsServerRunning = true;
                OnPropertyChanged(nameof(ServerButtonText));
                Status = $"Server running ({Path.GetFileName(serverPath)})";
                Log($"Started server: {Path.GetFileName(serverPath)}", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Failed to start single server: {ex.Message}", LogLevel.ERROR);
                Status = $"Error: {ex.Message}";
            }
        }

        private void StartDualServers(string gameServerPath, string sdkServerPath)
        {
            try
            {
                _serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gameServerPath,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Minimized
                    }
                };
                _serverProcess.EnableRaisingEvents = true;
                _serverProcess.Exited += ServerProcess_Exited;
                _serverProcess.Start();

                _sdkServerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = sdkServerPath,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Minimized
                    }
                };
                _sdkServerProcess.EnableRaisingEvents = true;
                _sdkServerProcess.Exited += ServerProcess_Exited;
                _sdkServerProcess.Start();

                IsServerRunning = true;
                OnPropertyChanged(nameof(ServerButtonText));
                Status = "Servers running (Game + SDK)";
                Log("Started Game Server and SDK Server", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Failed to start dual servers: {ex.Message}", LogLevel.ERROR);
                Status = $"Error: {ex.Message}";
            }
        }

        private void StopServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit();
                    Log("Server process exited", LogLevel.INFO);
                }

                if (_sdkServerProcess != null && !_sdkServerProcess.HasExited)
                {
                    _sdkServerProcess.Kill();
                    _sdkServerProcess.WaitForExit();
                }

                _serverProcess = null;
                _sdkServerProcess = null;
                IsServerRunning = false;
                OnPropertyChanged(nameof(ServerButtonText));
                Status = "Server stopped";
                
                // Only log this if we're not already inside the ServerProcess_Exited handler
                if (!IsInExitedEventHandler)
                {
                    Log("All server processes stopped", LogLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to stop server: {ex.Message}", LogLevel.ERROR);
                Status = $"Error: {ex.Message}";
            }
        }

        private void ServerProcess_Exited(object? sender, EventArgs e)
        {
            if (sender is Process process && process == _serverProcess)
            {
                IsInExitedEventHandler = true;
                StopServer();
                IsInExitedEventHandler = false;
            }
        }

        partial void OnGamePathChanged(string value)
        {
            Log($"GamePath changing to: {value}", LogLevel.DEBUG);
            if (!string.IsNullOrEmpty(value))
            {
                _config.Write("Settings", "GamePath", value);
                SaveSettingsInternal(false);
            }
            Log($"GamePath changed and saved", LogLevel.DEBUG);
        }

        partial void OnIpAddressChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                SaveSettingsInternal(false);
            }
        }

        partial void OnPortChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                SaveSettingsInternal(false);
            }
        }

        partial void OnUseLocalhostChanged(bool value)
        {
            if (value)
            {
                _lastIpAddress = IpAddress;
                IpAddress = "Localhost";
            }
            else
            {
                IpAddress = _config.Read("Settings", "CustomDestinationHost", "127.0.0.1");
            }
            UpdateIpAddressEnabled();
            SaveSettingsInternal(false);
        }

        partial void OnEnableLogsChanged(bool value)
        {
            if (value)
            {
                Log("Log enabled", LogLevel.INFO);
            }
        }

        partial void OnShowDebugLogsChanged(bool value)
        {
            if (value)
            {
                Log("Debug logging enabled", LogLevel.INFO);
            }
            else
            {
                Log("Debug logging disabled", LogLevel.INFO);
            }
        }

        private void SaveSettingsInternal(bool logSave = true)
        {
            try
            {
                _config.Write("Settings", "GamePath", GamePath);
                _config.Write("Settings", "UseLocalhost", UseLocalhost.ToString());
                if (!UseLocalhost && IpAddress != "Localhost")
                {
                    _config.Write("Settings", "CustomDestinationHost", IpAddress);
                }
                _config.Write("Settings", "DestinationPort", Port);

                if (logSave)
                {
                    string hostInfo = UseLocalhost ? "127.0.0.1" : IpAddress;
                    Status = $"Settings saved. Host: {hostInfo}, Port: {Port}";
                    Log($"Settings saved. Host: {hostInfo}, Port: {Port}", LogLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                Status = $"Error saving settings: {ex.Message}";
                Log($"Failed to save settings: {ex.Message}", LogLevel.ERROR);
            }
        }

        [RelayCommand]
        public void SaveSettings()
        {
            SaveSettingsInternal(true);
        }

        [RelayCommand]
        private async Task BrowseGamePath()
        {
            if (_window == null) return;

            try
            {
                var files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Game Executable",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { new FilePickerFileType("Executable") { Patterns = new[] { "*.exe" } } }
                });

                if (files.Count > 0)
                {
                    var file = files[0];
                    if (file.Path.IsFile)
                    {
                        GamePath = file.Path.LocalPath;
                        SaveSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to browse for game: {ex.Message}", LogLevel.ERROR);
            }
        }

        private void CleanupOnExit()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit(5000); // Wait up to 5 seconds for the process to exit
                }

                if (_sdkServerProcess != null && !_sdkServerProcess.HasExited)
                {
                    _sdkServerProcess.Kill();
                    _sdkServerProcess.WaitForExit(5000);
                }

                if (_processCheckTimer != null)
                {
                    _processCheckTimer.Stop();
                    _processCheckTimer.Dispose();
                }

                Log("Application cleanup completed", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Error during cleanup: {ex.Message}", LogLevel.ERROR);
            }
        }

        public void OnWindowClosing()
        {
            CleanupOnExit();
        }
    }
}

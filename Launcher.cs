using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Titanium.Web.Proxy.EventArguments;
using hkrpg.proxy.Properties;
using Titanium.Web.Proxy;

namespace hkrpg.proxy
{
    public partial class Launcher : Form
    {
        private Process? _gameProcess;
        private Process? _serverProcess;
        private Process? _sdkServerProcess;
        private ProxyService? proxyService;
        private string lastIpAddress = "127.0.0.1";
        private IniFile _config;

        public Launcher()
        {
            InitializeComponent();
            _config = new IniFile(Path.Combine(Application.StartupPath, "launcher.ini"));
            LoadSettings();
            InitializeProcessCheckTimer();
        }

        private enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            HTTP
        }

        private void InitializeProxy()
        {
            var config = new ProxyConfig
            {
                DestinationHost = Settings.Default.DestinationHost,
                DestinationPort = Settings.Default.DestinationPort,
                ProxyBindPort = 8080
            };

            proxyService = new ProxyService(config.DestinationHost, config.DestinationPort, config);
            proxyService.ProxyServer.BeforeRequest += OnBeforeRequest;
            proxyService.ProxyServer.ServerCertificateValidationCallback += OnCertificateValidationAsync;
            Log("Proxy initialized", LogLevel.INFO);
        }

        private void LoadSettings()
        {
            // Load settings from INI file
            gamePathBox.Text = _config.Read("Settings", "GamePath", "");
            ipBox.Text = _config.Read("Settings", "DestinationHost", "127.0.0.1");
            portBox.Text = _config.Read("Settings", "DestinationPort", "21000");

            UpdateIpBoxState();
            UpdateServerVisibility();

            // Hide debug logs checkbox by default
            enableDebugLogsCheckBox.Visible = false;
            enableDebugLogsCheckBox.Checked = false;
        }

        private void SaveSettings(bool logSave = true)
        {
            try
            {
                _config.Write("Settings", "GamePath", gamePathBox.Text);
                _config.Write("Settings", "DestinationHost", ipBox.Text);
                _config.Write("Settings", "DestinationPort", portBox.Text);

                if (logSave)
                {
                    Log("Settings saved", LogLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                Log($"Error saving settings: {ex.Message}", LogLevel.ERROR);
            }
        }

        private void InitializeProcessCheckTimer()
        {
            _processCheckTimer?.Start();
        }

        private void ProcessCheckTimer_Tick(object? sender, EventArgs e)
        {
            bool gameExited = false;
            bool serverExited = false;
            bool sdkServerExited = false;

            try
            {
                if (_gameProcess != null)
                {
                    try
                    {
                        // First check if the process exists by ID
                        if (!ProcessExists(_gameProcess.Id))
                        {
                            gameExited = true;
                            Log("Game process not found by ID", LogLevel.DEBUG);
                        }
                        else
                        {
                            // If process exists, check if it has exited
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
                        Log("Client disconnected", LogLevel.INFO);
                        
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => UpdateUIState(false)));
                        }
                        else
                        {
                            UpdateUIState(false);
                        }
                        statusLabel?.Invoke((MethodInvoker)delegate
                        {
                            statusLabel.Text = "Game stopped";
                        });
                    }
                }

                // Check server process independently of game process
                if (_serverProcess != null)
                {
                    try
                    {
                        if (!ProcessExists(_serverProcess.Id))
                        {
                            serverExited = true;
                            Log("Server process not found by ID", LogLevel.DEBUG);
                        }
                        else
                        {
                            try
                            {
                                serverExited = _serverProcess.HasExited;
                                Log($"Server process check: HasExited={serverExited}", LogLevel.DEBUG);
                            }
                            catch (InvalidOperationException)
                            {
                                serverExited = true;
                                Log("Server process check failed with InvalidOperationException", LogLevel.DEBUG);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Server process check error: {ex.Message}", LogLevel.DEBUG);
                        serverExited = true;
                    }

                    if (serverExited)
                    {
                        Log("Server process marked as exited, cleaning up", LogLevel.DEBUG);
                        _serverProcess = null;
                        serverStatusLabel?.Invoke((MethodInvoker)delegate
                        {
                            serverStatusLabel.Text = "Server: Stopped";
                        });
                    }
                }

                if (_sdkServerProcess != null)
                {
                    try
                    {
                        if (!ProcessExists(_sdkServerProcess.Id))
                        {
                            sdkServerExited = true;
                            Log("SDK Server process not found by ID", LogLevel.DEBUG);
                        }
                        else
                        {
                            try
                            {
                                sdkServerExited = _sdkServerProcess.HasExited;
                                Log($"SDK Server process check: HasExited={sdkServerExited}", LogLevel.DEBUG);
                            }
                            catch (InvalidOperationException)
                            {
                                sdkServerExited = true;
                                Log("SDK Server process check failed with InvalidOperationException", LogLevel.DEBUG);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"SDK Server process check error: {ex.Message}", LogLevel.DEBUG);
                        sdkServerExited = true;
                    }

                    if (sdkServerExited)
                    {
                        Log("SDK Server process marked as exited, cleaning up", LogLevel.DEBUG);
                        _sdkServerProcess = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Unexpected error in process check: {ex.GetType().Name} - {ex.Message}", LogLevel.DEBUG);
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

        private void UpdateServerVisibility()
        {
            startServerButton.Visible = localhostCheckBox.Checked;
            serverStatusLabel.Visible = localhostCheckBox.Checked;
        }

        private void UpdateIpBoxState()
        {
            ipBox.Enabled = !localhostCheckBox.Checked;
        }

        private void UpdateUIState(bool isRunning)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUIState(isRunning)));
                return;
            }

            try
            {
                Log($"Updating UI state: isRunning={isRunning}", LogLevel.DEBUG);
                
                launchButton.Enabled = !isRunning;
                stopButton.Enabled = isRunning;
                gamePathBox.Enabled = !isRunning;
                browseButton.Enabled = !isRunning;
                ipBox.Enabled = !isRunning && !localhostCheckBox.Checked;
                portBox.Enabled = !isRunning;
                saveButton.Enabled = !isRunning;
                localhostCheckBox.Enabled = !isRunning;
                enableLogsCheckBox.Enabled = !isRunning;
                startServerButton.Enabled = true;
                
                statusLabel.Text = isRunning ? "Game and proxy running" : "Ready";
                
                Log("UI state update completed", LogLevel.DEBUG);
            }
            catch (Exception ex)
            {
                Log($"UI state update failed: {ex.Message}", LogLevel.DEBUG);
            }
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Game Executable|StarRail.exe",
                Title = "Select Game Executable"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gamePathBox.Text = dialog.FileName;
                SaveSettings();
            }
        }

        private void LaunchButton_Click(object? sender, EventArgs e)
        {
            try
            {
                Log("Starting the Game", LogLevel.INFO);
                
                // Stop the process check timer during launch
                _processCheckTimer?.Stop();
                
                // Initialize proxy first
                InitializeProxy();
                
                // Create and start game process
                _gameProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gamePathBox.Text,
                        UseShellExecute = true
                    }
                };
                
                Log("Starting game process", LogLevel.DEBUG);
                _gameProcess.Start();

                // Wait a short time for the process to start
                Thread.Sleep(1000);

                // Verify the process is actually running
                if (_gameProcess.HasExited)
                {
                    throw new Exception("Game process exited immediately after starting");
                }

                // Update UI state
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateUIState(true)));
                }
                else
                {
                    UpdateUIState(true);
                }
                
                statusLabel?.Invoke((MethodInvoker)delegate
                {
                    statusLabel.Text = "Game started";
                });
                
                // Save settings without logging
                SaveSettings(false);
                
                // Restart the process check timer
                _processCheckTimer?.Start();
                
                Log("Game launch completed successfully", LogLevel.DEBUG);
            }
            catch (Exception ex)
            {
                Log($"Game launch failed: {ex.Message}", LogLevel.DEBUG);
                MessageBox.Show($"Failed to start game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Reset UI state on failure
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateUIState(false)));
                }
                else
                {
                    UpdateUIState(false);
                }
                
                // Restart the process check timer even on failure
                _processCheckTimer?.Start();
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            if (_gameProcess != null)
            {
                try
                {
                    _gameProcess.Kill();
                    _gameProcess = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to stop game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UpdateUIState(false);
                }
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (!localhostCheckBox.Checked)
            {
                lastIpAddress = ipBox.Text;
                Settings.Default.DestinationHost = ipBox.Text;
            }

            if (int.TryParse(portBox.Text, out int port))
            {
                Settings.Default.DestinationPort = port;
            }
            else
            {
                MessageBox.Show("Please enter a valid port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveSettings();
        }

        private void StartServerButton_Click(object? sender, EventArgs e)
        {
            if (_serverProcess == null || _serverProcess.HasExited)
            {
                StartServer();
            }
            else
            {
                StopServer();
            }
        }

        private void LocalhostCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (localhostCheckBox.Checked)
            {
                lastIpAddress = ipBox.Text;
                ipBox.Text = "Localhost";
                Settings.Default.DestinationHost = "127.0.0.1";
                SaveSettings();
            }
            else
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    StopServer();
                }

                ipBox.Text = lastIpAddress;
                Settings.Default.DestinationHost = lastIpAddress;
                SaveSettings();
            }
            UpdateIpBoxState();
            UpdateServerVisibility();
        }

        private void EnableLogsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool showLogs = enableLogsCheckBox.Checked;

            logLabel.Visible = showLogs;
            logBox.Visible = showLogs;
            enableDebugLogsCheckBox.Visible = showLogs;

            if (showLogs)
            {
                Log("Connection logging enabled", LogLevel.INFO);
                // Clear debug logs checkbox when enabling logs
                enableDebugLogsCheckBox.Checked = false;
            }
            else
            {
                logBox.Clear();
                // Hide and uncheck debug logs when disabling logs
                enableDebugLogsCheckBox.Visible = false;
                enableDebugLogsCheckBox.Checked = false;
            }
        }

        private void EnableDebugLogsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (enableDebugLogsCheckBox.Checked)
            {
                Log("Debug logging enabled", LogLevel.INFO);
            }
        }

        private void StartServer()
        {
            try
            {
                Log("Starting server process", LogLevel.DEBUG);
                string serverPath = Path.Combine(Application.StartupPath, "server.exe");
                if (File.Exists(serverPath))
                {
                    Log($"Found single server executable: {serverPath}", LogLevel.DEBUG);
                    StartSingleServer(serverPath);
                    return;
                }

                string gameServerPath = Path.Combine(Application.StartupPath, "gameserver.exe");
                string sdkServerPath = Path.Combine(Application.StartupPath, "sdkserver.exe");

                if (File.Exists(gameServerPath) && File.Exists(sdkServerPath))
                {
                    Log($"Found dual server executables: {gameServerPath}, {sdkServerPath}", LogLevel.DEBUG);
                    StartDualServers(gameServerPath, sdkServerPath);
                    return;
                }

                string[] jarFiles = Directory.GetFiles(Application.StartupPath, "*.jar");
                if (jarFiles.Length > 0)
                {
                    Log($"Found JAR file: {jarFiles[0]}", LogLevel.DEBUG);
                    StartSingleServer(jarFiles[0]);
                    return;
                }

                Log("No valid server executables found", LogLevel.DEBUG);
                MessageBox.Show("No valid server executables found. Please make sure server files are in the same directory.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log($"Error starting server: {ex.Message}", LogLevel.DEBUG);
                MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Server start error: {ex.Message}", LogLevel.ERROR);
            }
        }

        private void StartSingleServer(string serverPath)
        {
            Log($"Starting single server: {serverPath}", LogLevel.DEBUG);
            ProcessStartInfo startInfo;
            if (serverPath.EndsWith(".jar"))
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-jar \"{serverPath}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Log("Starting JAR server with Java", LogLevel.DEBUG);
            }
            else
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = serverPath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Log("Starting native server executable", LogLevel.DEBUG);
            }

            // Set launcher to be topmost temporarily
            bool wasTopMost = this.TopMost;
            this.TopMost = true;
            this.Focus();

            _serverProcess = new Process { StartInfo = startInfo };
            _serverProcess.EnableRaisingEvents = true;
            _serverProcess.Exited += ServerProcess_Exited;
            _serverProcess.Start();
            Log($"Server process started with ID: {_serverProcess.Id}", LogLevel.DEBUG);

            // Reset topmost state after a short delay
            Task.Delay(500).ContinueWith(t => 
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.TopMost = wasTopMost));
                }
                else
                {
                    this.TopMost = wasTopMost;
                }
            });

            startServerButton.Text = "Stop Server";
            serverStatusLabel.Text = $"Server: Running ({Path.GetFileName(serverPath)})";
            Log($"Started server: {Path.GetFileName(serverPath)}", LogLevel.INFO);
        }

        private void StartDualServers(string gameServerPath, string sdkServerPath)
        {
            // Set launcher to be topmost temporarily
            bool wasTopMost = this.TopMost;
            this.TopMost = true;
            this.Focus();

            _serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gameServerPath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
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
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            _sdkServerProcess.EnableRaisingEvents = true;
            _sdkServerProcess.Exited += ServerProcess_Exited;
            _sdkServerProcess.Start();

            // Reset topmost state after a short delay
            Task.Delay(500).ContinueWith(t => 
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.TopMost = wasTopMost));
                }
                else
                {
                    this.TopMost = wasTopMost;
                }
            });

            startServerButton.Text = "Stop Servers";
            serverStatusLabel.Text = "Servers: Running (Game + SDK)";
            Log("Started Game Server and SDK Server", LogLevel.INFO);
        }

        private void StopServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit();
                }

                if (_sdkServerProcess != null && !_sdkServerProcess.HasExited)
                {
                    _sdkServerProcess.Kill();
                    _sdkServerProcess.WaitForExit();
                }

                Log("All server processes stopped", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Log($"Server stop error: {ex.Message}", LogLevel.ERROR);
            }
            finally
            {
                _serverProcess = null;
                _sdkServerProcess = null;
                startServerButton.Text = "Start Server";
                serverStatusLabel.Text = "Server: Stopped";
            }
        }

        private void ServerProcess_Exited(object? sender, EventArgs e)
        {
            Log("Server process exited event received", LogLevel.DEBUG);
            if (InvokeRequired)
            {
                Invoke(new Action(() => ServerProcess_Exited(sender, e)));
                return;
            }

            // Only stop the server if it's the actual process that exited
            if (sender is Process process && process == _serverProcess)
            {
                StopServer();
                Log("Server process exited", LogLevel.INFO);
            }
        }

        private async Task OnBeforeRequest(object sender, SessionEventArgs e)
        {
            try
            {
                string? url = e.HttpClient.Request.Url;
                string? method = e.HttpClient.Request.Method;

                if (url == null || method == null)
                {
                    await Task.Run(() => Log($"Received request with null {(url == null ? "URL" : "Method")}", LogLevel.WARN));
                    return;
                }

                string formattedUrl = url;
                if (url.Length > 80)
                {
                    formattedUrl = url.Substring(0, 77) + "...";
                }

                string logMessage = $"[{method}] {formattedUrl}";
                await Task.Run(() => Log(logMessage, LogLevel.HTTP));
            }
            catch (Exception ex)
            {
                await Task.Run(() => Log($"Error in OnBeforeRequest: {ex.Message}", LogLevel.ERROR));
            }
        }

        private async Task OnCertificateValidationAsync(object sender, CertificateValidationEventArgs e)
        {
            e.IsValid = true;
            await Task.CompletedTask;
        }

        private void Log(string message, LogLevel level = LogLevel.INFO)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => Log(message, level)));
                return;
            }

            if (!enableLogsCheckBox.Checked)
            {
                return;
            }

            // Skip debug logs if debug logging is disabled
            if (level == LogLevel.DEBUG && !enableDebugLogsCheckBox.Checked)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string levelTag = level switch
            {
                LogLevel.DEBUG => "[DEBUG]",
                LogLevel.INFO => "[INFO]",
                LogLevel.WARN => "[WARN]",
                LogLevel.ERROR => "[ERROR]",
                LogLevel.HTTP => "[HTTP]",
                _ => "[INFO]"
            };
            
            string formattedMessage = $"[{timestamp}] {levelTag} {message}";

            if (logBox.Text.Length > 0 && !logBox.Text.EndsWith(Environment.NewLine))
            {
                logBox.AppendText(Environment.NewLine);
            }

            // Store current selection and color
            int start = logBox.TextLength;
            logBox.AppendText(formattedMessage + Environment.NewLine);
            int length = formattedMessage.Length;

            // Apply color based on log level
            logBox.Select(start, length);
            logBox.SelectionColor = level switch
            {
                LogLevel.DEBUG => Color.Gray,
                LogLevel.INFO => Color.Green,
                LogLevel.WARN => Color.Orange,
                LogLevel.ERROR => Color.Red,
                LogLevel.HTTP => Color.Blue,
                _ => logBox.ForeColor
            };

            // Reset selection
            logBox.SelectionStart = logBox.TextLength;
            logBox.SelectionLength = 0;
            logBox.SelectionColor = logBox.ForeColor;

            while (logBox.Lines.Length > 1000)
            {
                var lines = logBox.Lines.Skip(1).ToArray();
                logBox.Lines = lines;
            }

            logBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopServer();
            base.OnFormClosing(e);

            _processCheckTimer?.Stop();
            _processCheckTimer?.Dispose();

            if (_gameProcess != null)
            {
                try
                {
                    _gameProcess.Kill();
                }
                catch { }
            }

            if (proxyService != null)
            {
                try
                {
                    proxyService.ProxyServer.BeforeRequest -= OnBeforeRequest;
                    proxyService.ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidationAsync;
                    proxyService.Shutdown();
                }
                catch { }
            }
        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Launcher_Load(object sender, EventArgs e)
        {

        }
    }
} 
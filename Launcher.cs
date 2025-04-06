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

        public Launcher()
        {
            InitializeComponent();
            LoadSettings();
            InitializeProcessCheckTimer();
        }

        private enum LogLevel
        {
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
            if (Settings.Default.GamePath != null)
            {
                gamePathBox.Text = Settings.Default.GamePath;
            }

            ipBox.Text = Settings.Default.DestinationHost;
            portBox.Text = Settings.Default.DestinationPort.ToString();
            UpdateIpBoxState();
            UpdateServerVisibility();
        }

        private void SaveSettings(bool logSave = true)
        {
            Settings.Default.GamePath = gamePathBox.Text;
            Settings.Default.DestinationHost = ipBox.Text;
            if (int.TryParse(portBox.Text, out int port))
            {
                Settings.Default.DestinationPort = port;
            }
            Settings.Default.Save();
            if (logSave)
            {
                Log("Settings saved", LogLevel.INFO);
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
                    // Add a small grace period after process start
                    if (_gameProcess.StartTime.AddSeconds(2) > DateTime.Now)
                    {
                        return;  // Skip check if process just started
                    }

                    try
                    {
                        // Try to get process by ID to check if it still exists
                        Process.GetProcessById(_gameProcess.Id);
                        try
                        {
                            gameExited = _gameProcess.HasExited;
                        }
                        catch
                        {
                            gameExited = true;
                        }
                    }
                    catch
                    {
                        // Process not found by ID, meaning it's closed
                        gameExited = true;
                    }

                    if (gameExited)
                    {
                        _gameProcess = null;
                        Log("[SYSTEM] Client disconnected", LogLevel.INFO);
                        
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

                if (_serverProcess != null)
                {
                    try
                    {
                        serverExited = _serverProcess.HasExited;
                    }
                    catch (InvalidOperationException)
                    {
                        serverExited = true;
                    }

                    if (serverExited)
                    {
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
                        sdkServerExited = _sdkServerProcess.HasExited;
                    }
                    catch (InvalidOperationException)
                    {
                        sdkServerExited = true;
                    }

                    if (sdkServerExited)
                    {
                        _sdkServerProcess = null;
                    }
                }
            }
            catch (Exception)
            {
                // If any other unexpected error occurs, assume everything has stopped
                _gameProcess = null;
                _serverProcess = null;
                _sdkServerProcess = null;

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

                serverStatusLabel?.Invoke((MethodInvoker)delegate
                {
                    serverStatusLabel.Text = "Server: Stopped";
                });
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
                InitializeProxy();
                _gameProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gamePathBox.Text,
                        UseShellExecute = true
                    }
                };
                _gameProcess.Start();
                statusLabel.Text = "Game started";
                SaveSettings(false);  // Don't log when launching
                UpdateUIState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (showLogs)
            {
                Log("Connection logging enabled", LogLevel.INFO);
            }
            else
            {
                logBox.Clear();
            }
        }

        private void StartServer()
        {
            try
            {
                string serverPath = Path.Combine(Application.StartupPath, "server.exe");
                if (File.Exists(serverPath))
                {
                    StartSingleServer(serverPath);
                    return;
                }

                string gameServerPath = Path.Combine(Application.StartupPath, "gameserver.exe");
                string sdkServerPath = Path.Combine(Application.StartupPath, "sdkserver.exe");

                if (File.Exists(gameServerPath) && File.Exists(sdkServerPath))
                {
                    StartDualServers(gameServerPath, sdkServerPath);
                    return;
                }

                string[] jarFiles = Directory.GetFiles(Application.StartupPath, "*.jar");
                if (jarFiles.Length > 0)
                {
                    StartSingleServer(jarFiles[0]);
                    return;
                }

                MessageBox.Show("No valid server executables found. Please make sure server files are in the same directory.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Server start error: {ex.Message}", LogLevel.ERROR);
            }
        }

        private void StartSingleServer(string serverPath)
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
                    WindowStyle = ProcessWindowStyle.Normal
                };
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
            }

            // Set launcher to be topmost temporarily
            bool wasTopMost = this.TopMost;
            this.TopMost = true;
            this.Focus();

            _serverProcess = new Process { StartInfo = startInfo };
            _serverProcess.EnableRaisingEvents = true;
            _serverProcess.Exited += ServerProcess_Exited;
            _serverProcess.Start();

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
            if (InvokeRequired)
            {
                Invoke(new Action(() => ServerProcess_Exited(sender, e)));
                return;
            }

            StopServer();
            Log("Server process exited", LogLevel.INFO);
        }

        private async Task OnBeforeRequest(object sender, SessionEventArgs e)
        {
            try
            {
                string url = e.HttpClient.Request.Url;
                string method = e.HttpClient.Request.Method;

                string formattedUrl = url;
                if (url.Length > 80)
                {
                    formattedUrl = url.Substring(0, 77) + "...";
                }

                string logMessage = $"[{method}] {formattedUrl}";
                await Task.Run(() => Log(logMessage, LogLevel.HTTP));
            }
            catch { }
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

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string levelTag = level switch
            {
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
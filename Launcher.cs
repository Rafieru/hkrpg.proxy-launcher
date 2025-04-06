using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Text.Json;
using System.IO;
using Titanium.Web.Proxy;
using hkrpg.proxy.Properties;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;

namespace hkrpg.proxy
{
    public partial class Launcher : Form
    {
        private Process? gameProcess;
        private ProxyService? proxyService;
        private Button launchButton = null!;
        private Button stopButton = null!;
        private Button browseButton = null!;
        private TextBox gamePathBox = null!;
        private TextBox ipBox = null!;
        private TextBox portBox = null!;
        private TextBox logBox = null!;
        private Label statusLabel = null!;
        private CheckBox localhostCheckBox = null!;
        private System.Windows.Forms.Timer? _processCheckTimer;
        private string lastIpAddress = "127.0.0.1"; // Store the last used IP address
        private Button saveButton = null!;
        private CheckBox enableLogsCheckBox = null!;

        public Launcher()
        {
            InitializeComponents();
            InitializeProxy();
            LoadSettings();
            InitializeProcessCheckTimer();
        }

        private void LoadSettings()
        {
            // Load settings from application settings
            gamePathBox.Text = Settings.Default.GamePath;
            lastIpAddress = Settings.Default.DestinationHost; // Store the initial IP
            ipBox.Text = Settings.Default.DestinationHost;
            portBox.Text = Settings.Default.DestinationPort.ToString();
            
            // Set checkbox to checked by default
            localhostCheckBox.Checked = true;
            // This will trigger LocalhostCheckBox_CheckedChanged which will set the display to "Localhost"
            UpdateIpBoxState();
        }

        private void SaveSettings()
        {
            // Save settings to application settings
            Settings.Default.GamePath = gamePathBox.Text;
            Settings.Default.DestinationHost = ipBox.Text;
            if (int.TryParse(portBox.Text, out int port))
            {
                Settings.Default.DestinationPort = port;
            }
            Settings.Default.Save();
        }

        private void InitializeProxy()
        {
            try
            {
                // Shutdown existing proxy if it exists
                if (proxyService != null)
                {
                    try
                    {
                        proxyService.ProxyServer.BeforeRequest -= OnBeforeRequest;
                        proxyService.ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                        proxyService.Shutdown();
                    }
                    catch { }
                }

                var config = new ProxyConfig
                {
                    DestinationHost = localhostCheckBox.Checked ? "127.0.0.1" : Settings.Default.DestinationHost,
                    DestinationPort = Settings.Default.DestinationPort
                };

                // Initialize the collections
                config.RedirectDomains.AddRange(new[] { 
                    "api-os-takumi.mihoyo.com", 
                    "hkrpg-api-os.hoyoverse.com", 
                    "hkrpg-sdk-os.hoyoverse.com" 
                });

                config.AlwaysIgnoreDomains.AddRange(new[] { 
                    "overseauspider.yuanshen.com" 
                });

                config.BlockUrls.Add("https://sg-public-data-api.hoyoverse.com/device-fp/api/getFp");

                config.ForceRedirectOnUrlContains.AddRange(new[] { 
                    "hkrpg-api-os.hoyoverse.com", 
                    "hkrpg-sdk-os.hoyoverse.com" 
                });

                // Create proxy service with settings
                proxyService = new ProxyService(
                    config.DestinationHost,
                    config.DestinationPort,
                    config);

                // Subscribe to proxy events
                if (proxyService.ProxyServer != null)
                {
                    proxyService.ProxyServer.BeforeRequest += OnBeforeRequest;
                    proxyService.ProxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
                }

                statusLabel.Text = "Proxy initialized";
                Log("Proxy service initialized");
                Log($"Listening for connections to {config.DestinationHost}:{config.DestinationPort}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing proxy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Proxy initialization failed";
                Log($"Error: {ex.Message}");
            }
        }

        private async Task OnBeforeRequest(object sender, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
        {
            try
            {
                string url = e.HttpClient.Request.Url;
                string method = e.HttpClient.Request.Method;
                
                // Format the URL to be more readable
                string formattedUrl = url;
                if (url.Length > 80)
                {
                    // If URL is too long, truncate it and add ellipsis
                    formattedUrl = url.Substring(0, 77) + "...";
                }

                // Create a clean log format
                string logMessage = $"[{method}] {formattedUrl}";
                
                // Log in a separate task to avoid blocking
                await Task.Run(() => Log(logMessage));
            }
            catch { }
        }

        private async Task OnCertificateValidation(object sender, Titanium.Web.Proxy.EventArguments.CertificateValidationEventArgs e)
        {
            e.IsValid = true;
            await Task.CompletedTask;
        }

        private void Log(string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => Log(message)));
                return;
            }

            // Only log if logging is enabled
            if (!enableLogsCheckBox.Checked)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string formattedMessage = $"[{timestamp}] {message}";

            // Add extra newline if this is a new type of message
            if (logBox.Text.Length > 0 && !logBox.Text.EndsWith(Environment.NewLine))
            {
                logBox.AppendText(Environment.NewLine);
            }

            logBox.AppendText(formattedMessage + Environment.NewLine);

            // Keep only the last 1000 lines to prevent memory issues
            while (logBox.Lines.Length > 1000)
            {
                var lines = logBox.Lines.Skip(1).ToArray();
                logBox.Lines = lines;
            }

            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        private void InitializeComponents()
        {
            this.Text = "HKRPG Launcher";
            this.Size = new Size(500, 280);  // Reduced initial height since logs are hidden
            this.MinimumSize = new Size(500, 280);  // Minimum size without logs

            // Game path input
            var gamePathLabel = new Label
            {
                Text = "Game Path:",
                Location = new Point(20, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            gamePathBox = new TextBox
            {
                Location = new Point(20, 40),
                Size = new Size(350, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            browseButton = new Button
            {
                Text = "Browse",
                Location = new Point(380, 40),
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            browseButton.Click += BrowseButton_Click;

            // Localhost checkbox
            localhostCheckBox = new CheckBox
            {
                Text = "Use Localhost (127.0.0.1)",
                Location = new Point(20, 80),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            localhostCheckBox.CheckedChanged += LocalhostCheckBox_CheckedChanged;

            // IP and Port inputs
            var ipLabel = new Label
            {
                Text = "IP Address:",
                Location = new Point(20, 110),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            ipBox = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(150, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            ipBox.TextChanged += IpBox_TextChanged;

            var portLabel = new Label
            {
                Text = "Port:",
                Location = new Point(190, 110),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            portBox = new TextBox
            {
                Location = new Point(190, 130),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            portBox.TextChanged += PortBox_TextChanged;

            // Save button
            saveButton = new Button
            {
                Text = "Save Settings",
                Location = new Point(300, 130),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            saveButton.Click += SaveButton_Click;

            // Launch and Stop buttons
            launchButton = new Button
            {
                Text = "Launch",
                Location = new Point(20, 170),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            launchButton.Click += LaunchButton_Click;

            stopButton = new Button
            {
                Text = "Stop",
                Location = new Point(130, 170),
                Size = new Size(100, 30),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            stopButton.Click += StopButton_Click;

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(240, 175),
                Size = new Size(220, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Enable logs checkbox - moved below buttons
            enableLogsCheckBox = new CheckBox
            {
                Text = "Enable Connection Logs",
                Location = new Point(20, 210),
                AutoSize = true,
                Checked = false,  // Disabled by default
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            enableLogsCheckBox.CheckedChanged += EnableLogsCheckBox_CheckedChanged;

            // Log box and label
            var logLabel = new Label
            {
                Text = "Connection Log:",
                Location = new Point(20, 235),
                AutoSize = true,
                Visible = false,  // Hidden by default
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            logBox = new TextBox
            {
                Location = new Point(20, 255),
                Size = new Size(440, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Consolas", 9f),
                Visible = false,  // Hidden by default
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                gamePathLabel, gamePathBox, browseButton,
                localhostCheckBox,
                ipLabel, ipBox, portLabel, portBox,
                saveButton,
                launchButton, stopButton, statusLabel,
                enableLogsCheckBox,
                logLabel, logBox
            });
            
            // Initialize IP box state
            UpdateIpBoxState();
        }

        private void LocalhostCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (localhostCheckBox.Checked)
            {
                // Store current IP before changing display
                lastIpAddress = ipBox.Text;
                
                // Change display to "Localhost"
                ipBox.Text = "Localhost";
                
                // Use 127.0.0.1 for the actual proxy
                Settings.Default.DestinationHost = "127.0.0.1";
                SaveSettings();
                
                // Reinitialize proxy with localhost
                InitializeProxy();
            }
            else
            {
                // Restore the previous IP
                ipBox.Text = lastIpAddress;
                Settings.Default.DestinationHost = lastIpAddress;
                SaveSettings();
                
                // Reinitialize proxy with the previous IP
                InitializeProxy();
            }
            UpdateIpBoxState();
        }

        private void UpdateIpBoxState()
        {
            ipBox.Enabled = !localhostCheckBox.Checked;
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Select Game Executable",
                FileName = gamePathBox.Text
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gamePathBox.Text = dialog.FileName;
                SaveSettings();
            }
        }

        private void IpBox_TextChanged(object? sender, EventArgs e)
        {
            // Remove auto-save functionality
        }

        private void PortBox_TextChanged(object? sender, EventArgs e)
        {
            // Remove auto-save functionality
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
            InitializeProxy();
            Log("Settings saved and proxy reinitialized");
        }

        private void LaunchButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gamePathBox.Text))
            {
                MessageBox.Show("Please select the game executable first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (proxyService == null)
            {
                MessageBox.Show("Proxy service is not initialized. Please restart the application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Start game
                gameProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gamePathBox.Text,
                        UseShellExecute = true
                    }
                };
                gameProcess.Start();
                statusLabel.Text = "Game started";
                SaveSettings();

                UpdateUIState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopButton_Click(null, EventArgs.Empty);
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            if (gameProcess != null)
            {
                try
                {
                    gameProcess.Kill();
                    gameProcess = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (proxyService != null)
            {
                try
                {
                    // Unsubscribe from events
                    proxyService.ProxyServer.BeforeRequest -= OnBeforeRequest;
                    proxyService.ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                    proxyService.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping proxy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            UpdateUIState(false);
        }

        private void Launcher_FormClosing(object? sender, FormClosingEventArgs e)
        {
            StopButton_Click(null, EventArgs.Empty);
            SaveSettings();
        }

        private void ProcessCheckTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (gameProcess != null)
                {
                    bool hasExited;
                    try
                    {
                        hasExited = gameProcess.HasExited;
                    }
                    catch (InvalidOperationException)
                    {
                        // Process is no longer valid
                        hasExited = true;
                    }

                    if (hasExited)
                    {
                        gameProcess = null;
                        UpdateUIState(false);
                    }
                }
            }
            catch (Exception)
            {
                // If any other error occurs, assume the process is gone
                gameProcess = null;
                UpdateUIState(false);
            }
        }

        private void InitializeProcessCheckTimer()
        {
            _processCheckTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Check every second
            };
            _processCheckTimer.Tick += ProcessCheckTimer_Tick;
            _processCheckTimer.Start();
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
            statusLabel.Text = isRunning ? "Game started" : "Ready";
        }

        private void EnableLogsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool showLogs = enableLogsCheckBox.Checked;
            
            // Find the log label and update visibility
            var logLabel = Controls.Cast<Control>()
                .FirstOrDefault(c => c is Label && c.Text == "Connection Log:");
            if (logLabel != null)
            {
                logLabel.Visible = showLogs;
            }

            // Update log box visibility
            logBox.Visible = showLogs;
            
            // Adjust form size
            if (showLogs)
            {
                this.MinimumSize = new Size(500, 450);  // Larger minimum size with logs
                this.Size = new Size(500, 450);  // Expand window for logs
                Log("Connection logging enabled");
            }
            else
            {
                this.Size = new Size(500, 280);  // Collapse window without logs
                this.MinimumSize = new Size(500, 280);  // Smaller minimum size without logs
                logBox.Clear();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            _processCheckTimer?.Stop();
            _processCheckTimer?.Dispose();
            
            if (gameProcess != null)
            {
                try
                {
                    gameProcess.Kill();
                }
                catch { }
            }

            if (proxyService != null)
            {
                try
                {
                    // Unsubscribe from events
                    proxyService.ProxyServer.BeforeRequest -= OnBeforeRequest;
                    proxyService.ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                    proxyService.Shutdown();
                }
                catch { }
            }
        }
    }
} 
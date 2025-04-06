namespace hkrpg.proxy
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer _processCheckTimer;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _processCheckTimer = new System.Windows.Forms.Timer(components);
            gamePathBox = new TextBox();
            browseButton = new Button();
            localhostCheckBox = new CheckBox();
            ipBox = new TextBox();
            portBox = new TextBox();
            statusLabel = new Label();
            logBox = new TextBox();
            launchButton = new Button();
            stopButton = new Button();
            saveButton = new Button();
            startServerButton = new Button();
            serverStatusLabel = new Label();
            enableLogsCheckBox = new CheckBox();
            gamePathLabel = new Label();
            ipLabel = new Label();
            portLabel = new Label();
            logLabel = new Label();
            SuspendLayout();
            // 
            // _processCheckTimer
            // 
            _processCheckTimer.Interval = 1000;
            _processCheckTimer.Tick += ProcessCheckTimer_Tick;
            // 
            // gamePathBox
            // 
            gamePathBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gamePathBox.Location = new Point(20, 40);
            gamePathBox.Name = "gamePathBox";
            gamePathBox.Size = new Size(350, 23);
            gamePathBox.TabIndex = 0;
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseButton.Location = new Point(380, 40);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(80, 23);
            browseButton.TabIndex = 1;
            browseButton.Text = "Browse";
            browseButton.Click += BrowseButton_Click;
            // 
            // localhostCheckBox
            // 
            localhostCheckBox.AutoSize = true;
            localhostCheckBox.Checked = true;
            localhostCheckBox.CheckState = CheckState.Checked;
            localhostCheckBox.Location = new Point(20, 80);
            localhostCheckBox.Name = "localhostCheckBox";
            localhostCheckBox.Size = new Size(155, 19);
            localhostCheckBox.TabIndex = 2;
            localhostCheckBox.Text = "Use Localhost (127.0.0.1)";
            localhostCheckBox.CheckedChanged += LocalhostCheckBox_CheckedChanged;
            // 
            // ipBox
            // 
            ipBox.Location = new Point(20, 130);
            ipBox.Name = "ipBox";
            ipBox.Size = new Size(150, 23);
            ipBox.TabIndex = 3;
            // 
            // portBox
            // 
            portBox.Location = new Point(190, 130);
            portBox.Name = "portBox";
            portBox.Size = new Size(100, 23);
            portBox.TabIndex = 4;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(20, 210);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(128, 15);
            statusLabel.TabIndex = 5;
            statusLabel.Text = "Ready: Proxy Initialized";
            // 
            // logBox
            // 
            logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logBox.BackColor = Color.White;
            logBox.Font = new Font("Consolas", 9F);
            logBox.Location = new Point(20, 281);
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = ScrollBars.Vertical;
            logBox.Size = new Size(440, 142);
            logBox.TabIndex = 6;
            logBox.Visible = false;
            logBox.TextChanged += logBox_TextChanged;
            // 
            // launchButton
            // 
            launchButton.Location = new Point(20, 170);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(100, 30);
            launchButton.TabIndex = 7;
            launchButton.Text = "Launch";
            launchButton.Click += LaunchButton_Click;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new Point(130, 170);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(100, 30);
            stopButton.TabIndex = 8;
            stopButton.Text = "Stop";
            stopButton.Click += StopButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(300, 130);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(100, 23);
            saveButton.TabIndex = 9;
            saveButton.Text = "Save Settings";
            saveButton.Click += SaveButton_Click;
            // 
            // startServerButton
            // 
            startServerButton.Location = new Point(240, 170);
            startServerButton.Name = "startServerButton";
            startServerButton.Size = new Size(100, 30);
            startServerButton.TabIndex = 10;
            startServerButton.Text = "Start Server";
            startServerButton.Visible = false;
            startServerButton.Click += StartServerButton_Click;
            // 
            // serverStatusLabel
            // 
            serverStatusLabel.AutoSize = true;
            serverStatusLabel.Location = new Point(240, 210);
            serverStatusLabel.Name = "serverStatusLabel";
            serverStatusLabel.Size = new Size(89, 15);
            serverStatusLabel.TabIndex = 11;
            serverStatusLabel.Text = "Server: Stopped";
            serverStatusLabel.Visible = false;
            // 
            // enableLogsCheckBox
            // 
            enableLogsCheckBox.AutoSize = true;
            enableLogsCheckBox.Location = new Point(20, 240);
            enableLogsCheckBox.Name = "enableLogsCheckBox";
            enableLogsCheckBox.Size = new Size(154, 19);
            enableLogsCheckBox.TabIndex = 12;
            enableLogsCheckBox.Text = "Enable Connection Logs";
            enableLogsCheckBox.CheckedChanged += EnableLogsCheckBox_CheckedChanged;
            // 
            // gamePathLabel
            // 
            gamePathLabel.AutoSize = true;
            gamePathLabel.Location = new Point(20, 20);
            gamePathLabel.Name = "gamePathLabel";
            gamePathLabel.Size = new Size(68, 15);
            gamePathLabel.TabIndex = 13;
            gamePathLabel.Text = "Game Path:";
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Location = new Point(20, 110);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(65, 15);
            ipLabel.TabIndex = 14;
            ipLabel.Text = "IP Address:";
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(190, 110);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(32, 15);
            portLabel.TabIndex = 15;
            portLabel.Text = "Port:";
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.Location = new Point(19, 262);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(95, 15);
            logLabel.TabIndex = 16;
            logLabel.Text = "Connection Log:";
            logLabel.Visible = false;
            // 
            // Launcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 435);
            Controls.Add(logLabel);
            Controls.Add(portLabel);
            Controls.Add(ipLabel);
            Controls.Add(gamePathLabel);
            Controls.Add(enableLogsCheckBox);
            Controls.Add(serverStatusLabel);
            Controls.Add(startServerButton);
            Controls.Add(saveButton);
            Controls.Add(stopButton);
            Controls.Add(launchButton);
            Controls.Add(logBox);
            Controls.Add(statusLabel);
            Controls.Add(portBox);
            Controls.Add(ipBox);
            Controls.Add(localhostCheckBox);
            Controls.Add(browseButton);
            Controls.Add(gamePathBox);
            MinimumSize = new Size(500, 430);
            Name = "Launcher";
            Text = "HKRPG Launcher";
            Load += Launcher_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox gamePathBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.CheckBox localhostCheckBox;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button launchButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Label serverStatusLabel;
        private System.Windows.Forms.CheckBox enableLogsCheckBox;
        private System.Windows.Forms.Label gamePathLabel;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label logLabel;
    }
} 
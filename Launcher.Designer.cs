namespace HkrpgProxy.Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            _processCheckTimer = new System.Windows.Forms.Timer(components);
            gamePathBox = new TextBox();
            browseButton = new Button();
            localhostCheckBox = new CheckBox();
            ipBox = new TextBox();
            portBox = new TextBox();
            statusLabel = new Label();
            logBox = new RichTextBox();
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
            enableDebugLogsCheckBox = new CheckBox();
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
            gamePathBox.Font = new Font("Consolas", 9F);
            gamePathBox.Location = new Point(20, 40);
            gamePathBox.Name = "gamePathBox";
            gamePathBox.Size = new Size(350, 22);
            gamePathBox.TabIndex = 0;
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseButton.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            browseButton.Location = new Point(382, 40);
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
            localhostCheckBox.Font = new Font("Consolas", 9F);
            localhostCheckBox.Location = new Point(20, 80);
            localhostCheckBox.Name = "localhostCheckBox";
            localhostCheckBox.Size = new Size(201, 18);
            localhostCheckBox.TabIndex = 2;
            localhostCheckBox.Text = "Use Localhost (127.0.0.1)";
            localhostCheckBox.CheckedChanged += LocalhostCheckBox_CheckedChanged;
            // 
            // ipBox
            // 
            ipBox.Font = new Font("Consolas", 9F);
            ipBox.Location = new Point(20, 130);
            ipBox.Name = "ipBox";
            ipBox.Size = new Size(150, 22);
            ipBox.TabIndex = 3;
            // 
            // portBox
            // 
            portBox.Font = new Font("Consolas", 9F);
            portBox.Location = new Point(190, 130);
            portBox.Name = "portBox";
            portBox.Size = new Size(100, 22);
            portBox.TabIndex = 4;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Consolas", 9F);
            statusLabel.Location = new Point(20, 210);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(175, 14);
            statusLabel.TabIndex = 5;
            statusLabel.Text = "Ready";
            // 
            // logBox
            // 
            logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logBox.BackColor = Color.White;
            logBox.Font = new Font("Consolas", 9F);
            logBox.ForeColor = Color.White;
            logBox.Location = new Point(20, 284);
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            logBox.Size = new Size(442, 139);
            logBox.TabIndex = 6;
            logBox.Text = "";
            logBox.Visible = false;
            // 
            // launchButton
            // 
            launchButton.Font = new Font("Consolas", 9F);
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
            stopButton.Font = new Font("Consolas", 9F);
            stopButton.Location = new Point(130, 170);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(100, 30);
            stopButton.TabIndex = 8;
            stopButton.Text = "Stop";
            stopButton.Click += StopButton_Click;
            // 
            // saveButton
            // 
            saveButton.Font = new Font("Consolas", 9F);
            saveButton.Location = new Point(300, 130);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(100, 23);
            saveButton.TabIndex = 9;
            saveButton.Text = "Save Settings";
            saveButton.Click += SaveButton_Click;
            // 
            // startServerButton
            // 
            startServerButton.Font = new Font("Consolas", 9F);
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
            serverStatusLabel.Font = new Font("Consolas", 9F);
            serverStatusLabel.Location = new Point(240, 210);
            serverStatusLabel.Name = "serverStatusLabel";
            serverStatusLabel.Size = new Size(112, 14);
            serverStatusLabel.TabIndex = 11;
            serverStatusLabel.Text = "Server: Stopped";
            serverStatusLabel.Visible = false;
            // 
            // enableLogsCheckBox
            // 
            enableLogsCheckBox.AutoSize = true;
            enableLogsCheckBox.Font = new Font("Consolas", 9F);
            enableLogsCheckBox.Location = new Point(20, 240);
            enableLogsCheckBox.Name = "enableLogsCheckBox";
            enableLogsCheckBox.Size = new Size(103, 18);
            enableLogsCheckBox.TabIndex = 12;
            enableLogsCheckBox.Text = "Enable Logs";
            enableLogsCheckBox.CheckedChanged += EnableLogsCheckBox_CheckedChanged;
            // 
            // gamePathLabel
            // 
            gamePathLabel.AutoSize = true;
            gamePathLabel.Font = new Font("Consolas", 9F);
            gamePathLabel.Location = new Point(20, 20);
            gamePathLabel.Name = "gamePathLabel";
            gamePathLabel.Size = new Size(77, 14);
            gamePathLabel.TabIndex = 13;
            gamePathLabel.Text = "Game Path:";
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Font = new Font("Consolas", 9F);
            ipLabel.Location = new Point(20, 110);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(84, 14);
            ipLabel.TabIndex = 14;
            ipLabel.Text = "IP Address:";
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Font = new Font("Consolas", 9F);
            portLabel.Location = new Point(190, 110);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(42, 14);
            portLabel.TabIndex = 15;
            portLabel.Text = "Port:";
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.Font = new Font("Consolas", 9F);
            logLabel.Location = new Point(20, 262);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(42, 14);
            logLabel.TabIndex = 16;
            logLabel.Text = "Logs:";
            logLabel.Visible = false;
            // 
            // enableDebugLogsCheckBox
            // 
            enableDebugLogsCheckBox.AutoSize = true;
            enableDebugLogsCheckBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            enableDebugLogsCheckBox.ForeColor = Color.Brown;
            enableDebugLogsCheckBox.Location = new Point(130, 239);
            enableDebugLogsCheckBox.Name = "enableDebugLogsCheckBox";
            enableDebugLogsCheckBox.Size = new Size(243, 18);
            enableDebugLogsCheckBox.TabIndex = 12;
            enableDebugLogsCheckBox.Text = "Show Debug Logs (For Developer)";
            enableDebugLogsCheckBox.UseVisualStyleBackColor = true;
            enableDebugLogsCheckBox.CheckedChanged += EnableDebugLogsCheckBox_CheckedChanged;
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
            Controls.Add(enableDebugLogsCheckBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(500, 430);
            Name = "Launcher";
            Text = "HKRPG.Proxy Launcher";
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
        private System.Windows.Forms.RichTextBox logBox;
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
        private System.Windows.Forms.CheckBox enableDebugLogsCheckBox;
    }
} 
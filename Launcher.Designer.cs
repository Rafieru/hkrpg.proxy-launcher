namespace hkrpg.proxy
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.gamePathBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.localhostCheckBox = new System.Windows.Forms.CheckBox();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.logBox = new System.Windows.Forms.TextBox();
            this.launchButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.startServerButton = new System.Windows.Forms.Button();
            this.serverStatusLabel = new System.Windows.Forms.Label();
            this.enableLogsCheckBox = new System.Windows.Forms.CheckBox();
            this.gamePathLabel = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.logLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // gamePathBox
            // 
            this.gamePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamePathBox.Location = new System.Drawing.Point(20, 40);
            this.gamePathBox.Name = "gamePathBox";
            this.gamePathBox.Size = new System.Drawing.Size(350, 23);
            this.gamePathBox.TabIndex = 0;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(380, 40);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(80, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse";
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // localhostCheckBox
            // 
            this.localhostCheckBox.AutoSize = true;
            this.localhostCheckBox.Checked = true;
            this.localhostCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localhostCheckBox.Location = new System.Drawing.Point(20, 80);
            this.localhostCheckBox.Name = "localhostCheckBox";
            this.localhostCheckBox.Size = new System.Drawing.Size(150, 19);
            this.localhostCheckBox.TabIndex = 2;
            this.localhostCheckBox.Text = "Use Localhost (127.0.0.1)";
            this.localhostCheckBox.CheckedChanged += new System.EventHandler(this.LocalhostCheckBox_CheckedChanged);
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point(20, 130);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(150, 23);
            this.ipBox.TabIndex = 3;
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(190, 130);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 23);
            this.portBox.TabIndex = 4;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(20, 210);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(108, 15);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Ready: Proxy Initialized";
            // 
            // logBox
            // 
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.BackColor = System.Drawing.Color.White;
            this.logBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logBox.Location = new System.Drawing.Point(20, 290);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(440, 150);
            this.logBox.TabIndex = 6;
            this.logBox.Visible = false;
            // 
            // launchButton
            // 
            this.launchButton.Location = new System.Drawing.Point(20, 170);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(100, 30);
            this.launchButton.TabIndex = 7;
            this.launchButton.Text = "Launch";
            this.launchButton.Click += new System.EventHandler(this.LaunchButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(130, 170);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(100, 30);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(300, 130);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Save Settings";
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // startServerButton
            // 
            this.startServerButton.Location = new System.Drawing.Point(240, 170);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(100, 30);
            this.startServerButton.TabIndex = 10;
            this.startServerButton.Text = "Start Server";
            this.startServerButton.Visible = false;
            this.startServerButton.Click += new System.EventHandler(this.StartServerButton_Click);
            // 
            // serverStatusLabel
            // 
            this.serverStatusLabel.AutoSize = true;
            this.serverStatusLabel.Location = new System.Drawing.Point(240, 210);
            this.serverStatusLabel.Name = "serverStatusLabel";
            this.serverStatusLabel.Size = new System.Drawing.Size(85, 15);
            this.serverStatusLabel.TabIndex = 11;
            this.serverStatusLabel.Text = "Server: Stopped";
            this.serverStatusLabel.Visible = false;
            // 
            // enableLogsCheckBox
            // 
            this.enableLogsCheckBox.AutoSize = true;
            this.enableLogsCheckBox.Location = new System.Drawing.Point(20, 240);
            this.enableLogsCheckBox.Name = "enableLogsCheckBox";
            this.enableLogsCheckBox.Size = new System.Drawing.Size(140, 19);
            this.enableLogsCheckBox.TabIndex = 12;
            this.enableLogsCheckBox.Text = "Enable Connection Logs";
            this.enableLogsCheckBox.CheckedChanged += new System.EventHandler(this.EnableLogsCheckBox_CheckedChanged);
            // 
            // gamePathLabel
            // 
            this.gamePathLabel.AutoSize = true;
            this.gamePathLabel.Location = new System.Drawing.Point(20, 20);
            this.gamePathLabel.Name = "gamePathLabel";
            this.gamePathLabel.Size = new System.Drawing.Size(67, 15);
            this.gamePathLabel.TabIndex = 13;
            this.gamePathLabel.Text = "Game Path:";
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(20, 110);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(65, 15);
            this.ipLabel.TabIndex = 14;
            this.ipLabel.Text = "IP Address:";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(190, 110);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(32, 15);
            this.portLabel.TabIndex = 15;
            this.portLabel.Text = "Port:";
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(20, 270);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(93, 15);
            this.logLabel.TabIndex = 16;
            this.logLabel.Text = "Connection Log:";
            this.logLabel.Visible = false;
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.gamePathLabel);
            this.Controls.Add(this.enableLogsCheckBox);
            this.Controls.Add(this.serverStatusLabel);
            this.Controls.Add(this.startServerButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.launchButton);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.localhostCheckBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.gamePathBox);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "Launcher";
            this.Text = "HKRPG Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();
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
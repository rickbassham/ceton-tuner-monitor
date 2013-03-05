namespace CetonMonitor
{
    partial class ControlForm
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
            this.label1 = new System.Windows.Forms.Label();
            this._serviceController = new System.ServiceProcess.ServiceController();
            this.label2 = new System.Windows.Forms.Label();
            this._tuners = new System.Windows.Forms.TextBox();
            this._saveSettings = new System.Windows.Forms.Button();
            this._installService = new System.Windows.Forms.Button();
            this._startService = new System.Windows.Forms.Button();
            this._warning = new System.Windows.Forms.Label();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._launchPerfMon = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ceton infiniTV 4 Performance Monitor";
            // 
            // _serviceController
            // 
            this._serviceController.ServiceName = "CetonMonitorService";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tuner IP Addresses (one per line):";
            // 
            // _tuners
            // 
            this._tuners.Location = new System.Drawing.Point(16, 55);
            this._tuners.Multiline = true;
            this._tuners.Name = "_tuners";
            this._tuners.Size = new System.Drawing.Size(180, 76);
            this._tuners.TabIndex = 2;
            // 
            // _saveSettings
            // 
            this._saveSettings.Location = new System.Drawing.Point(16, 137);
            this._saveSettings.Name = "_saveSettings";
            this._saveSettings.Size = new System.Drawing.Size(180, 23);
            this._saveSettings.TabIndex = 3;
            this._saveSettings.Text = "Save Settings";
            this._saveSettings.UseVisualStyleBackColor = true;
            this._saveSettings.Click += new System.EventHandler(this._saveSettings_Click);
            // 
            // _installService
            // 
            this._installService.Location = new System.Drawing.Point(16, 166);
            this._installService.Name = "_installService";
            this._installService.Size = new System.Drawing.Size(180, 23);
            this._installService.TabIndex = 4;
            this._installService.Text = "Install Service";
            this._installService.UseVisualStyleBackColor = true;
            this._installService.Click += new System.EventHandler(this._installService_Click);
            // 
            // _startService
            // 
            this._startService.Location = new System.Drawing.Point(16, 195);
            this._startService.Name = "_startService";
            this._startService.Size = new System.Drawing.Size(180, 23);
            this._startService.TabIndex = 5;
            this._startService.Text = "Start Service";
            this._startService.UseVisualStyleBackColor = true;
            this._startService.Click += new System.EventHandler(this._startService_Click);
            // 
            // _warning
            // 
            this._warning.AutoSize = true;
            this._warning.Location = new System.Drawing.Point(13, 221);
            this._warning.MaximumSize = new System.Drawing.Size(180, 0);
            this._warning.Name = "_warning";
            this._warning.Size = new System.Drawing.Size(0, 13);
            this._warning.TabIndex = 6;
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Interval = 1000;
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // _launchPerfMon
            // 
            this._launchPerfMon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._launchPerfMon.Location = new System.Drawing.Point(16, 262);
            this._launchPerfMon.Name = "_launchPerfMon";
            this._launchPerfMon.Size = new System.Drawing.Size(180, 23);
            this._launchPerfMon.TabIndex = 7;
            this._launchPerfMon.Text = "Launch Perf Mon";
            this._launchPerfMon.UseVisualStyleBackColor = true;
            this._launchPerfMon.Click += new System.EventHandler(this._launchPerfMon_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 297);
            this.Controls.Add(this._launchPerfMon);
            this.Controls.Add(this._warning);
            this.Controls.Add(this._startService);
            this.Controls.Add(this._installService);
            this.Controls.Add(this._saveSettings);
            this.Controls.Add(this._tuners);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ControlForm";
            this.Text = "Ceton infiniTV 4 Performance Monitor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.ServiceProcess.ServiceController _serviceController;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _tuners;
        private System.Windows.Forms.Button _saveSettings;
        private System.Windows.Forms.Button _installService;
        private System.Windows.Forms.Button _startService;
        private System.Windows.Forms.Label _warning;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Button _launchPerfMon;
    }
}
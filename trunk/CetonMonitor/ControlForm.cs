using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Collections.Specialized;
using System.Diagnostics;

namespace CetonMonitor
{
    public partial class ControlForm : Form
    {
        private bool _serviceInstalled = false;
        private ServiceControllerStatus _status = ServiceControllerStatus.Stopped;

        public ControlForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _tuners.Lines = new List<string>(Settings.Default.CetonTuners.Cast<string>()).ToArray();
            _tuners.Select(0, 0);

            RefreshStatus();
        }

        private void RefreshStatus()
        {
            _serviceInstalled = false;
            _status = ServiceControllerStatus.Stopped;

            try
            {
                _serviceController.Refresh();

                _status = _serviceController.Status;
                _serviceInstalled = true;
            }
            catch
            {
            }

            if (_serviceInstalled)
            {
                _startService.Enabled = true;
                _installService.Text = "Uninstall Service";

                if (_status == ServiceControllerStatus.Running)
                {
                    _startService.Text = "Stop Service";
                }
                else
                {
                    _startService.Text = "Start Service";
                }
            }
            else
            {
                _startService.Enabled = false;
                _installService.Text = "Install Service";
            }
        }

        private void _saveSettings_Click(object sender, EventArgs e)
        {
            _warning.Text = "You need to stop and start the service to see your new changes.";

            Settings.Default.CetonTuners.Clear();

            foreach (var line in _tuners.Lines)
            {
                Settings.Default.CetonTuners.Add(line);
            }

            Settings.Default.Save();

            RefreshStatus();
        }

        private void _installService_Click(object sender, EventArgs e)
        {
            if (_serviceInstalled)
            {
                ApplicationManager.Uninstall();
            }
            else
            {
                ApplicationManager.Install();
            }

            RefreshStatus();
        }

        private void _startService_Click(object sender, EventArgs e)
        {
            if (_serviceInstalled)
            {
                if (_status == ServiceControllerStatus.Running)
                    _serviceController.Stop();
                else
                    _serviceController.Start();
            }

            RefreshStatus();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void _launchPerfMon_Click(object sender, EventArgs e)
        {
            Process.Start("perfmon.exe");
        }
    }
}

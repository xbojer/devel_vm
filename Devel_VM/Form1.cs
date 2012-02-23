﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Devel_VM
{
    public partial class fMain : Form
    {

        

        public fMain()
        {
            InitializeComponent();
            zasobnik.Visible = true;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowMe();
            }
            base.WndProc(ref m);
        }
        private void ShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            // make our form jump to the top of everything
            TopMost = true;
            // set it back to whatever it was
            TopMost = top;
        }
        
        private void showBaloon(String msg, String title, int priority)
        {
            ToolTipIcon ico;

            switch(priority) {
                case 1:
                    ico = ToolTipIcon.Info;
                    break;
                case 2:
                    ico = ToolTipIcon.Warning;
                    break;
                case 3:
                    ico = ToolTipIcon.Error;
                    break;
                default:
                    ico = ToolTipIcon.None;
                    break;
            }

            zasobnik.ShowBalloonTip(3500, title, msg, ico);
        }

        #region Tray options
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tAutoStart.Enabled = false;
            Program.VM.Start();
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.Restart();
        }
        private void softstopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.PowerOff(false);
        }
        private void stoppoweroffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.PowerOff(true);
        }

        private void zasobnik_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        #endregion

        #region Form control
        private void fMain_Load(object sender, EventArgs e)
        {
            #if !DEBUG
            Hide();
            Visible = false;
            WindowState = FormWindowState.Minimized;
            button1.Enabled = false;
            #endif

            Program.NL = new Network_listener();

            Program.VM.OnVmEvent += new VirtualMachine.VmEvent(this.showBaloon);
        }
        private void bHide_Click(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion

        #region Timers' Events
        private void tState_Tick(object sender, EventArgs e)
        {
            Program.VM.Tick();
            
        }
        private void tUpdateState_Tick(object sender, EventArgs e)
        {
            lState.Text = Program.VM.Status.ToString();
            if(Program.VM.Status == VirtualMachine.State.On) {
                tAutoStart.Enabled = false;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            int result = Program.VM.getVersion();
            MessageBox.Show(result.ToString());
            
        }

        private void restartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String result = Program.VM.exec("/bin/sh", "/opt/fotka/bin/control_httpd restart").Trim();
            if (result != "OK")
            {
                showBaloon("Error while restarting HTTPD: "+ result, "HTTPD", 3);
            }
            else
            {
                showBaloon("Service restarted", "HTTPD", 1);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String result = Program.VM.exec("/bin/sh", "/opt/fotka/bin/control_httpd reload").Trim();
            if (result != "OK")
            {
                showBaloon("Error while reloading HTTPD: " + result, "HTTPD", 3);
            }
            else
            {
                showBaloon("Service reloaded", "HTTPD", 1);
            }
        }

        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String result = Program.VM.exec("/bin/sh", "/opt/fotka/bin/control_httpd start").Trim();
            if (result != "OK")
            {
                showBaloon("Error while starting HTTPD: " + result, "HTTPD", 3);
            }
            else
            {
                showBaloon("Service started", "HTTPD", 1);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String result = Program.VM.exec("/bin/sh", "/opt/fotka/bin/control_httpd reload").Trim();
            if (result != "OK")
            {
                showBaloon("Error while stopping HTTPD: " + result, "HTTPD", 3);
            }
            else
            {
                showBaloon("Service stopped", "HTTPD", 1);
            }
        }
    }
    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}

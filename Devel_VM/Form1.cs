using System;
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
                WmShowMe();
            }
            if (m.Msg == NativeMethods.WM_UPDATING)
            {
                WmUpdating();
            }
            base.WndProc(ref m);
        }
        private void WmShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            bool top = TopMost; // get our current "TopMost" value (ours will always be false though)
            TopMost = true; // make our form jump to the top of everything
            TopMost = top; // set it back to whatever it was
        }
        private void WmUpdating()
        {
            showBaloon("Updating...", "Beta Manager Updater", 1);
        }
        
        private void showBaloon(String msg, String title, int priority)
        {
            MethodInvoker method = delegate
            {
                ToolTipIcon ico;

                switch (priority)
                {
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
            };

            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
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

            Program.NL.OnInfo += delegate(string auth, string msg)
            {
                showBaloon(msg, "Info od admina "+auth, 1);
            };
            Program.NL.OnError += delegate(string auth, string msg)
            {
                showBaloon(msg, "WAŻNE (" + auth + ")", 2);
            };
            Program.NL.OnPing += delegate(string auth, string msg)
            {
                //showBaloon(msg, "WAŻNE (" + auth + ")", 2);
                Packet p = new Packet();
                p.dataIdentifier = Packet.DataIdentifier.Pong;
                p.message = auth + "+" + msg;
                Network_Broadcast.send(p);
            };

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
            switch (Program.VM.Status)
            {
                case VirtualMachine.State.On:
                    tAutoStart.Enabled = false;
                    hTTPDToolStripMenuItem.Enabled = false;
                    bStart.Enabled = startToolStripMenuItem.Enabled = false;
                    bSoftStop.Enabled = softstopToolStripMenuItem.Enabled = false;
                    bStopPower.Enabled = stoppoweroffToolStripMenuItem.Enabled = true;
                    restartToolStripMenuItem.Enabled = true;
                    break;
                case VirtualMachine.State.Operational:
                    tAutoStart.Enabled = false;
                    hTTPDToolStripMenuItem.Enabled = true;
                    bStart.Enabled = startToolStripMenuItem.Enabled = false;
                    bSoftStop.Enabled = softstopToolStripMenuItem.Enabled = true;
                    bStopPower.Enabled = stoppoweroffToolStripMenuItem.Enabled = true;
                    restartToolStripMenuItem.Enabled = true;
                    break;
                default://off?
                    hTTPDToolStripMenuItem.Enabled = false;
                    bStart.Enabled = startToolStripMenuItem.Enabled = true;
                    bSoftStop.Enabled = softstopToolStripMenuItem.Enabled = false;
                    bStopPower.Enabled = stoppoweroffToolStripMenuItem.Enabled = false;
                    restartToolStripMenuItem.Enabled = false;
                    break;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //int result = Program.VM.getVersion();
            //string result = Program.identity;
            //MessageBox.Show(result.ToString());
            Packet p = new Packet();
            p.dataIdentifier = Packet.DataIdentifier.Pong;
            p.message = "Debug";
            Network_Broadcast.send(p);

            //Properties.Settings.Default.Save();

            Program.VM.Uninstall();
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
        public static readonly int WM_UPDATING = RegisterWindowMessage("WM_UPDATING");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}

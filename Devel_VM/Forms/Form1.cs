using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Devel_VM
{
    public partial class fMain : Form
    {
        private bool allowshowdisplay = false;
        private bool mainbaseinit = true;
        public fMain()
        {
            InitializeComponent();
            zasobnik.Visible = true;
            Visible = false;
#if DEBUG
            showToolStripMenuItem.Visible = true;
            toolStripSeparator1.Visible = true;
#endif
        }
        protected override void SetVisibleCore(bool value)
        {
            if (!allowshowdisplay && mainbaseinit)
            {
                mainbaseinit = false;
                Program.NL.OnInfo += delegate(string auth, string msg)
                {
                    showBaloon(auth + ": " + msg, "Beta Manager: Informator", 1);
                };
                Program.NL.OnError += delegate(string auth, string msg)
                {
                    showBaloon(String.Format("!!! $1 ($2) !!!", auth, msg), "Beta Manager: Informator", 2);
                };
                Program.VM.OnVmEvent += new VirtualMachine.VmEvent(showBaloon);
                Program.VM.initMachine();
            }
            base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
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
            showBaloon("Trwa aktualizacja programu", "Beta Manager: Aktualizator", 1);
        }
        
        public void showBaloon(String msg, String title, int priority)
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
            this.allowshowdisplay = true;
            this.Visible = !this.Visible;
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
#if DEBUG
            Show();
#endif
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
            button2.Text = Program.VM.Status.ToString();
            zasobnik.Text = "Beta Manager - " + Program.VM.MachineName + " " + Program.VM.Status.ToString();
            switch (Program.VM.Status)
            {
                case VirtualMachine.State.On:
                    tAutoStart.Enabled = false;
                    hTTPDToolStripMenuItem.Enabled = false;
                    toolStripMenuItem2.Enabled = true;
                    toolStripMenuItem2.Text = "Restart";
                    toolStripMenuItem3.Text = "Wyłącz";
                    toolStripMenuItem3.Enabled = toolStripMenuItem3.Visible = true;
                    usuńObrazToolStripMenuItem.Enabled = pobierzNaNowoObrazToolStripMenuItem.Enabled = false;
                    break;
                case VirtualMachine.State.Operational:
                    tAutoStart.Enabled = false;
                    hTTPDToolStripMenuItem.Enabled = true;
                    toolStripMenuItem2.Enabled = true;
                    toolStripMenuItem2.Text = "Restart";
                    toolStripMenuItem3.Text = "Wyślij sygnał wyłączenia";
                    toolStripMenuItem3.Enabled = toolStripMenuItem3.Visible = true;
                    usuńObrazToolStripMenuItem.Enabled = pobierzNaNowoObrazToolStripMenuItem.Enabled = false;
                    break;
                default://off?
                    hTTPDToolStripMenuItem.Enabled = false;
                    toolStripMenuItem2.Enabled = true;
                    toolStripMenuItem2.Text = "Uruchom";
                    toolStripMenuItem3.Enabled = toolStripMenuItem3.Visible = false;
                    usuńObrazToolStripMenuItem.Enabled = pobierzNaNowoObrazToolStripMenuItem.Enabled = true;
                    break;
            }

            if (Program.UpdateNeeded)
            {
                toolStripMenuItem7.Text = "Aktualizuj program";
            }
            else
            {
                toolStripMenuItem7.Text = "Aktualizuj obraz";
            }
            toolStripMenuItem7.Visible = (Program.VM.UpdateNeeded || Program.UpdateNeeded);

        }
        #endregion
        #region HTTPD Control
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
            /*String test = Program.VM.exec("/bin/echo", "/tmp/asdfg");
            showBaloon(test, "debug", 1);
            return;*/
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
            String result = Program.VM.exec("/bin/sh", "/opt/fotka/bin/control_httpd stop").Trim();
            if (result != "OK")
            {
                showBaloon("Error while stopping HTTPD: " + result, "HTTPD", 3);
            }
            else
            {
                showBaloon("Service stopped", "HTTPD", 1);
            }
        }
        #endregion
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            switch (Program.VM.Status)
            {
                case VirtualMachine.State.On:
                    Program.VM.Restart();
                    break;
                case VirtualMachine.State.Operational:
                    Program.VM.Restart();
                    break;
                default://off?
                    tAutoStart.Enabled = false;
                    Program.VM.Start();
                    break;
            }
            
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            switch (Program.VM.Status)
            {
                case VirtualMachine.State.On:
                    Program.VM.PowerOff(true);
                    break;
                case VirtualMachine.State.Operational:
                    Program.VM.PowerOff(false);
                    break;
                default://off?
                    break;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Program.DBG.Show();
        }
        private void tAutoStart_Tick(object sender, EventArgs e)
        {
            tAutoStart.Enabled = false;
            Program.VM.Start();
        }
        private void informacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.AboutBox1 ab = new Forms.AboutBox1();
            ab.Show();
        }
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            tAutoStart.Enabled = false;
            if (Program.UpdateNeeded)
            {
                showBaloon("Uruchamianie instalatora programu", "BetaManager: Aktualizacja", 1);
                Program.Update();
            }
            else
            {
                Program.VM.PowerOff(true);
                Program.VM.Uninstall(Program.VM.MachineName);
                Program.VM.Install();
                Program.VM.Rename(Program.VM.MachineName + "_installing", Program.VM.MachineName);
                Program.VM.reInit();
            }
        }
        private void sprawdźAktulizacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.checkVersion())
            {
                if (Program.VM.checkVersion(false))//aktualna
                {
                    showBaloon("Aplikacja i obraz są aktualne.", "BetaManager: Aktualizacja", 1);
                }
                else
                {
                    showBaloon("Obraz jest nieaktualny.", "BetaManager: Aktualizacja", 2);
                }
            }
            else
            {
                showBaloon("Aplikacja wymaga aktualizacji.", "BetaManager: Aktualizacja", 2);
            }
        }
        private void usuńObrazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tAutoStart.Enabled = false;
            Program.VM.PowerOff(true);
            Program.VM.Uninstall(Program.VM.MachineName);
        }

        private void tUpdateAutocheck_Tick(object sender, EventArgs e)
        {
            tUpdateAutocheck.Interval = 3600000;
            SilentUpdateCheck();
        }

        void SilentUpdateCheck()
        {
            if (Program.checkVersion())
            {
                if (!Program.VM.checkVersion(false))
                {
                    showBaloon("Obraz jest nieaktualny.", "BetaManager: Aktualizacja", 2);
                }
            }
            else
            {
                showBaloon("Aplikacja wymaga aktualizacji.", "BetaManager: Aktualizacja", 2);
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

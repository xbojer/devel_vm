using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using Devel_VM.Classes;
using Devel_VM.Forms;
using System.Collections.Generic;
using System.Diagnostics;

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
                Program.LogEvents += new Program.LogEvent(showBaloon);
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
            Program.Log("Trwa aktualizacja programu", "Devel VM Manager: Aktualizator", 1);
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
            zasobnik.Text = "Devel VM Manager - " + Program.VM.MachineName + " " + Program.VM.Status.ToString();
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
        internal bool vmHTTPDoperation(string op, bool verbose = true) {
            String result = Program.VM.exec("/bin/bash", "-c '/opt/fotka/bin/control_httpd " + op + "'").Trim();
            if (verbose)
            {
                if (result != "OK")
                {
                    Program.Log("Error while " + op + "ing HTTPD: " + result, "HTTPD", 3);
                }
                else
                {
                    Program.Log("Service " + op + "ed", "HTTPD", 1);
                }
            }
            return (result == "OK");
        }
        private void restartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            vmHTTPDoperation("restart");
        }
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vmHTTPDoperation("reload");
        }
        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            vmHTTPDoperation("start");
        }
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vmHTTPDoperation("stop");
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
                Program.Log("Uruchamianie instalatora programu", "Devel VM Manager: Aktualizacja", 1);
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
        private void sprawdzAktulizacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.checkVersion())
            {
                if (Program.VM.checkVersion(false))//aktualna
                {
                    Program.Log("Aplikacja i obraz są aktualne.", "Devel VM Manager: Aktualizacja", 1);
                }
                else
                {
                    Program.Log("Obraz jest nieaktualny.", "Devel VM Manager: Aktualizacja", 2);
                }
            }
            else
            {
                Program.Log("Aplikacja wymaga aktualizacji.", "Devel VM Manager: Aktualizacja", 2);
            }
        }
        private void usunObrazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tAutoStart.Enabled = false;
            Program.VM.PowerOff(true);
            Program.VM.Uninstall(Program.VM.MachineName);
        }

        private void tUpdateAutocheck_Tick(object sender, EventArgs e)
        {
            tUpdateAutocheck.Interval = 3600000;
            updateWWWtree();
            updateNodeJStree();
            updateDeamonsTree();
            Program.silentCheckVersion();
        }
        private void updateWWWtree()
        {
            bETAToolStripMenuItem.Visible = false;
            bETAToolStripMenuItem.DropDownItems.Clear();

            Dictionary<string, Dictionary<string, string>> data = Scanner.getSiteUrls();
            foreach (string k in data.Keys)
            {
                ToolStripMenuItem domain = (ToolStripMenuItem)bETAToolStripMenuItem.DropDownItems.Add(k);
                Dictionary<string, string> entries;

                if (data.TryGetValue(k, out entries))
                {
                    foreach (string bk in entries.Keys)
                    {
                        string uri;
                        if (entries.TryGetValue(bk, out uri))
                        {
                            if (bk == "@")
                            {
                                domain.Click += new EventHandler(delegate(object sender, EventArgs e)
                                {
                                    openURL(uri);
                                });
                                domain.ToolTipText = uri;
                            }
                            else
                            {
                                ToolStripItem branche = domain.DropDownItems.Add(bk);
                                branche.Click += new EventHandler(delegate(object sender, EventArgs e)
                                {
                                    openURL(uri);
                                });
                                branche.ToolTipText = uri;
                            }
                        }
                    }
                }
            }
            if (bETAToolStripMenuItem.DropDownItems.Count > 0)
            {
                bETAToolStripMenuItem.Visible = true;
            }
        }
        private void updateNodeJStree()
        {
            NodeJSstripMenu.Visible = false;
            NodeJSstripMenu.DropDownItems.Clear();

            Dictionary<string, Dictionary<string, string>> data = Scanner.getNodeApps();
            foreach (string k in data.Keys)
            {
                ToolStripMenuItem domain = (ToolStripMenuItem)NodeJSstripMenu.DropDownItems.Add(k);
                Dictionary<string, string> entries;

                if (data.TryGetValue(k, out entries))
                {
                    foreach (string bk in entries.Keys)
                    {
                        string uri;
                        if (entries.TryGetValue(bk, out uri))
                        {
                            ToolStripItem branche = domain.DropDownItems.Add(bk);
                            branche.Click += new EventHandler(delegate(object sender, EventArgs e)
                            {
                                openNodeCMD(uri);
                            });
                            branche.ToolTipText = uri;
                        }
                    }
                }
            }
            if (NodeJSstripMenu.DropDownItems.Count > 0)
            {
                NodeJSstripMenu.Visible = true;
            }
        }
        private void updateDeamonsTree()
        {
            DeamonsStripMenu.Visible = false;
            DeamonsStripMenu.DropDownItems.Clear();

            Dictionary<string, Dictionary<string, string>> data = Scanner.getDaemonsInstances();
            foreach (string k in data.Keys)
            {
                ToolStripMenuItem domain = (ToolStripMenuItem)DeamonsStripMenu.DropDownItems.Add(k);
                Dictionary<string, string> entries;

                if (data.TryGetValue(k, out entries))
                {
                    foreach (string bk in entries.Keys)
                    {
                        string uri;
                        if (entries.TryGetValue(bk, out uri))
                        {
                            ToolStripItem branche = domain.DropDownItems.Add(bk);
                            branche.Click += new EventHandler(delegate(object sender, EventArgs e)
                            {
                                openNodeCMD(uri);
                            });
                            branche.ToolTipText = uri;
                        }
                    }
                }
            }
            if (DeamonsStripMenu.DropDownItems.Count > 0)
            {
                DeamonsStripMenu.Visible = true;
            }
        }
        private void openURL(string uri)
        {
            Process.Start(uri);
        }
        private void openNodeCMD(string cmd)
        {
            String result = Program.VM.exec("/bin/bash", cmd, true).Trim();
            if (result == "")
            {
                Program.Log("Empty response :(", "Executor", 2);
            }
            else
            {
                Program.Log(result, "Executor", 0);
            }
            return;
        }
        private void aLFAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(@"\\ALPHA");
        }
        private void bETA100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(@"\\BETA");
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputBoxResult r = InputBox.Show("Polecenie do uruchomienia", "Devel VM Manager: Exec");
            if (r.ReturnCode == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(r.Text))
                {
                    String result = Program.VM.exec("/bin/bash", "r.Text", true).Trim();
                    if (result == "")
                    {
                        Program.Log("Empty response :(", "Executor", 2);
                    }
                    else
                    {
                        Program.Log(result, "Executor", 0);
                    }
                }
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

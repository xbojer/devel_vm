using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using Devel_VM.Forms;
using System.Linq;

namespace Devel_VM
{
    static class Program
    {
        static public VirtualMachine VM;
        static public Network_listener NL;
        static public Forms.Debug DBG;
        static public string identity = "NOT YET KNOWN";
        static public string username = "NOT YET KNOWN";

        static Mutex mutex = new Mutex(true, "mutex_beta_manager_devel_vm_runonce");
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.GetCommandLineArgs().Contains<string>("/installed"))
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
                Properties.Settings.Default.Save();
                Application.Restart();
                return;
            }
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (Properties.Settings.Default.UpgradeSettings)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeSettings = false;
                    Properties.Settings.Default.Save();
                }

                if (Environment.GetCommandLineArgs().Contains<string>("/reset"))
                {
                    Properties.Settings.Default.Reset();
                    Properties.Settings.Default.Save();
                }

                identity = getIdentity(Environment.GetCommandLineArgs().Contains<string>("/auth"));

                LogEvents += new LogEvent(NetworkLog);

                VM = new VirtualMachine();

                DBG = new Forms.Debug();
                VM.OnVmEvent += new VirtualMachine.VmEvent(DBG.debugLog);
                VM.OnVmEvent += new VirtualMachine.VmEvent(Log);

                NL = new Network_listener();
                NL.OnPing += delegate(string auth, string msg)
                {
                    Packet p = new Packet();
                    p.dataIdentifier = Packet.DataIdentifier.Pong;
                    p.message = auth + "[" + msg +"]:T["+ DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds+"]";
                    Network_Broadcast.send(p);
                };
                NL.OnInfo += delegate(string auth, string msg)
                {
                    Log(auth + ": " + msg, "Devel VM Manager: Informator", 1);
                };
                NL.OnError += delegate(string auth, string msg)
                {
                    Log(String.Format("!!! {1} ({0}) !!!", auth, msg), "Devel VM Manager: Informator", 2);
                };
                NL.OnVersion += delegate(string auth, string msg)
                {
                    Packet p = new Packet();
                    p.dataIdentifier = Packet.DataIdentifier.Debug;
                    p.message = "AppVer: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    p.message += " / ImgVer: " + VM.getVersion();
                    Network_Broadcast.send(p);
                };

                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                Application.Run(new fMain());
                mutex.ReleaseMutex();
            }
            else
            {
                if(Environment.GetCommandLineArgs().Contains<string>("/r")) {
                    Process[] prcs = Process.GetProcessesByName("Devel_VM");
                    foreach (Process prc in prcs)
                    {
                        if (Process.GetCurrentProcess().Id != prc.Id)
                        {
                            prc.Kill();
                        }
                    }
                    prcs = Process.GetProcessesByName("Beta_Manager");
                    foreach (Process prc in prcs)
                    {
                        if (Process.GetCurrentProcess().Id != prc.Id)
                        {
                            prc.Kill();
                        }
                    }
                    prcs = Process.GetProcessesByName("VBoxSVC");
                    foreach (Process prc in prcs) prc.Kill();
                    prcs = Process.GetProcessesByName("VBoxHeadless");
                    foreach (Process prc in prcs) prc.Kill();
                    Application.Restart();
                }

                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero
                );
            }
        }

        #region Logging
        public delegate void LogEvent(String msg, String title, int priority);
        public static LogEvent LogEvents;
        public static void Log(String msg, string title, int priority)
        {
            if (LogEvents != null)
            {
                LogEvents(msg, title, priority);
            }
        }
        public static void NetworkLog(String msg, String title, int priority)
        {
            string prio;
            switch (priority)
            {
                case 1:
                    prio = "Info";
                    break;
                case 2:
                    prio = "Warning";
                    break;
                case 3:
                    prio = "Error";
                    break;
                default:
                    prio = "-";
                    break;
            }

            Packet p = new Packet();
            p.dataIdentifier = Packet.DataIdentifier.Debug;
            p.message = title + ":" + prio + ":" + msg;
            Network_Broadcast.send(p);
        }
        #endregion
        #region Version checking
        internal static bool UpdateNeeded = false;
        public static void silentCheckVersion()
        {
            if (Program.checkVersion())
            {
                if (!Program.VM.checkVersion(false))
                {
                    Log("Obraz jest nieaktualny.", "Devel VM Manager: Aktualizacja", 2);
                }
            }
            else
            {
                Log("Aplikacja wymaga aktualizacji.", "Devel VM Manager: Aktualizacja", 2);
            }
        }
        internal static bool checkVersion()
        {
            try
            {
                Version newVersion = new Version(getRemoteVersion());
                Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (curVersion.CompareTo(newVersion) < 0)
                {
                    UpdateNeeded = true;
                }
                else
                {
                    UpdateNeeded = false;
                }
            }
            catch (ArgumentException)
            {
                UpdateNeeded = false;
            }
            return !UpdateNeeded;
        }
        public static string getRemoteVersion()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Properties.Settings.Default.path_imgver))
                {
                    String line;
                    if ((line = sr.ReadLine()) == null) return "0";//img ver
                    if ((line = sr.ReadLine()) == null) return "0";//app ver
                    return line;
                }
            }
            catch (Exception) { }
            return "0";
        }
        #endregion
        internal static void Update()
        {
            string ur = Properties.Settings.Default.path_updater;
            string ura = Properties.Settings.Default.path_updater_args + " " + getRemoteVersion().Replace(".", "_");
            VM.PowerOff(true, true);
            Process.Start(ur, ura);
            Application.Exit();
        }
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            VM.Destroy();
            Log("Application Exiting", "MAIN", 0);
        }
        static public string getMACAddress()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Properties.Settings.Default.path_macaddr))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] data = line.Split("#".ToArray());
                        if (data.Length != 2) continue;
                        if (data[1] == username)
                        {
                            sr.Close();
                            return data[0];
                        }
                    }
                }
            }
            catch (Exception) { }
            return "-";
        }
        static string getIdentity(bool forget = false)
        {
            if (forget)
            {
                Properties.Settings.Default.User = "";
                Properties.Settings.Default.Save();
            }

            username = Properties.Settings.Default.User;

            if (String.IsNullOrEmpty(username))
            {
                InputBoxResult r = InputBox.Show("Wklej link z hashem do robota", "Devel VM Manager: Auth");
                if (r.ReturnCode == DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(r.Text))
                    {
                        if(r.Text.Trim()=="offline") username = "OfflineMode";
                        else username = Robot.getUsernameByLink(r.Text.Trim());
                    }
                }
            }

            if (String.IsNullOrEmpty(username) || Robot.user_unknown == username)
            {
                MessageBox.Show("Błąd autoryzacji!");
                if (Application.MessageLoop == true)
                {
                    Application.Exit();
                }
                else
                {
                    System.Environment.Exit(1);
                }
                throw new Exception("Błąd autoryzacji!");
            }

            if (username != Properties.Settings.Default.User && username != Robot.user_unknown && username != "OfflineMode")
            {
                Properties.Settings.Default.User = username;
                Properties.Settings.Default.Save();
            }

            string host = Dns.GetHostName();
            List<string> ips = new List<string>();

            IPAddress[] lips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in lips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (ip.ToString().StartsWith(Properties.Settings.Default.vm_settings_networkprefix))
                        ips.Add(ip.ToString());
                }
            }
            return String.Format("{0} ({1} / {2})", username, host, string.Join(",", ips.ToArray()));
        }

    }
}

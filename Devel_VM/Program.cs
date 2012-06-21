﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using Devel_VM.Forms;

namespace Devel_VM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static public VirtualMachine VM;
        static public Network_listener NL;
        static public Forms.Debug DBG;
        static public string identity = "NOT YET KNOWN";
        static public string username = "NOT YET KNOWN";
        static public Preview PREV;

        static Mutex mutex = new Mutex(true, "mutex_beta_manager_devel_vm_runonce");
        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                identity = getIdentity();

                PREV = new Preview();

                VM = new VirtualMachine();

                DBG = new Forms.Debug();
                VM.OnVmEvent += new VirtualMachine.VmEvent(DBG.debugLog);

                NL = new Network_listener();
                NL.OnPing += delegate(string auth, string msg)
                {
                    Packet p = new Packet();
                    p.dataIdentifier = Packet.DataIdentifier.Pong;
                    p.message = auth + "+" + msg;
                    Network_Broadcast.send(p);
                };
                
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                Application.Run(new fMain());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance jump on top of all the other windows
                string[] cla = Environment.GetCommandLineArgs();
                foreach (string item in cla)
                {
                    if (item.Trim() == "/r")
                    {
                        Process[] prcs = Process.GetProcessesByName("Beta_Manager");
                        foreach (Process prc in prcs)
                        {
                            if (Process.GetCurrentProcess().Id != prc.Id)
                            {
                                prc.Kill();
                            }
                        }
                        Application.Restart();
                    }
                }

                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero
                );
            }
        }

        internal static bool UpdateNeeded = false;
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
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (VM.MachineReady.getReadyOffline())
            {
                //VM.PowerOff(true, true);
                if(VM.TTY != null) VM.TTY.Stop();
            }
        }
        static string getIdentity()
        {
            username = Properties.Settings.Default.User;

            if (String.IsNullOrEmpty(username))
            {
                InputBoxResult r = InputBox.Show("Wklej link z hashem do robota", "BetaManager: Auth");
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
                    if(ip.ToString().StartsWith("10."))
                        ips.Add(ip.ToString());
                }
            }
            return String.Format("{0} ({1} / {2})", username, host, string.Join(",", ips.ToArray()));
        }
        internal static void Update()
        {
            string ur = Properties.Settings.Default.path_updater;
            string ura = Properties.Settings.Default.path_updater_args;
            Process.Start(ur, ura);
            VM.PowerOff(true, true);
            Application.Exit();
        }
    }
}

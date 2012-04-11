using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;

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

        static Mutex mutex = new Mutex(true, "mutex_beta_manager_devel_vm_runonce");
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                identity = getIdentity();
                VM = new VirtualMachine();
#if !DEBUG
                (new Thread(new ThreadStart(updater.go))).Start();
#endif
                DBG = new Forms.Debug();
                Application.Run(new fMain());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero
                );
            }
        }

        static string getIdentity()
        {
            username = Properties.Settings.Default.User;
            username = "";

            if (String.IsNullOrEmpty(username))
            {
                InputBoxResult r = InputBox.Show("Wklej link z hashem do robota", "BetaManager: Auth");
                if (r.ReturnCode == DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(r.Text))
                    {
                        username = Robot.getUsernameByLink(r.Text.Trim());
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

            if (username != Properties.Settings.Default.User && username != Robot.user_unknown)
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
    }
}

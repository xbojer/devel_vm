using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Net;

namespace Devel_VM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static public VirtualMachine VM;
        static public Network_listener NL;
        static public string identity = "NOT YET KNOWN";

        static Mutex mutex = new Mutex(true, "mutex_beta_manager_devel_vm_runonce");
        [STAThread]
        static void Main()
        {
            identity = getIdentity();
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                VM = new VirtualMachine();

                (new Thread(new ThreadStart(updater.go))).Start();

                
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
            string user = Properties.Settings.Default.User;
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
            return String.Format("{0} ({1} / {2})", user, host, string.Join(",", ips.ToArray()));
        }
    }
}

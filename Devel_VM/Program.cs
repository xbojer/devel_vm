using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Devel_VM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static public VirtualMachine VM;
        static public Network_listener NL;
        static Mutex mutex = new Mutex(true, "mutex_beta_manager_devel_vm_runonce");
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                VM = new VirtualMachine();

                (new Thread(new ThreadStart(updater.go))).Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
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
    }
}

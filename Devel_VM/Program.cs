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

        [STAThread]
        static void Main()
        {
            VM = new VirtualMachine();

            //(new Thread(new ThreadStart(updater.go))).Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fMain());
        }
    }
}

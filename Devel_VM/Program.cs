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
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            (new Thread(new ThreadStart(updater.go))).Start();
            Application.Run(new fMain());
        }
    }
}

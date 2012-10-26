using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace DVMinstaller
{
    static class Program
    {
        public static bool is64BitProcess = (IntPtr.Size == 8);
        public static bool is64BitOperatingSystem = is64BitProcess || Is64BitOperatingSystem();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new f());
        }

        public static int executeLastCode = -1;
        public static string execute(string exe, string args, bool systemdir = true, bool detached = false)
        {
            string r = "";
            Process MyProc = new Process();
            MyProc.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            MyProc.StartInfo.FileName = exe;
            MyProc.StartInfo.UseShellExecute = false;
            MyProc.StartInfo.RedirectStandardError = !detached;
            MyProc.StartInfo.RedirectStandardInput = !detached;
            MyProc.StartInfo.RedirectStandardOutput = !detached;
            if (detached)
            {
                MyProc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
            else
            {
                MyProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            MyProc.StartInfo.Arguments = args;

            MyProc.Start();
            if (!detached)
            {
                MyProc.WaitForExit();
                r = MyProc.StandardOutput.ReadToEnd();
                executeLastCode = MyProc.ExitCode;
                MyProc.Close();
            }
            return r;
        }

        public static Dictionary<string, string> getSharedFolders()
        {

            Dictionary<string, string> sharedFolders = new Dictionary<string, string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_share");
            foreach (ManagementObject share in searcher.Get())
            {
                string type = share["Type"].ToString();
                if (type == "0") // 0 = DiskDrive (1 = Print Queue, 2 = Device, 3 = IPH)
                {
                    string name = share["Name"].ToString(); //getting share name
                    string path = share["Path"].ToString(); //getting share path
                    //string caption = share["Caption"].ToString(); //getting share description
                    sharedFolders.Add(name, path);
                }
            }
            return sharedFolders;
        }
        public static string getRemoteVersion()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"\\ALPHA\instale\Devel_beta\current_version.txt"))
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
        public static void Exterminate()
        {
            if (Application.MessageLoop == true)
            {
                Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }

        /// <summary>
        /// The function determines whether the current operating system is a 
        /// 64-bit operating system.
        /// </summary>
        /// <returns>
        /// The function returns true if the operating system is 64-bit; 
        /// otherwise, it returns false.
        /// </returns>
        public static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
            {
                return true;
            }
            else  // 32-bit programs run on both 32-bit and 64-bit Windows
            {
                // Detect whether the current process is a 32-bit process 
                // running on a 64-bit system.
                bool flag;
                return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    IsWow64Process(GetCurrentProcess(), out flag)) && flag);
            }
        }

        /// <summary>
        /// The function determins whether a method exists in the export 
        /// table of a certain module.
        /// </summary>
        /// <param name="moduleName">The name of the module</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns>
        /// The function returns true if the method specified by methodName 
        /// exists in the export table of the module specified by moduleName.
        /// </returns>
        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)]string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
    }
}

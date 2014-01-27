using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Management;
using System.Net;
using vbAccelerator.Components.Shell;

namespace DVMinstaller
{
    public partial class f : Form
    {
        bool CloseLock = false;
        string remoteVer = "0";
        string tempDir = "";
        string remoteBaseUrl = @"https://raw.github.com/xbojer/devel_vm/master/Devel_VM/publish/Application%20Files/";
        string vboxInstallBase = @"\\alpha\public\ADMINISTRACJA\devel\VirtualBox-";
        List<string> filenames = new List<string>();

        string vboxVer = "4.3.6";

        string mainexe = "";

        bool rebootRequired = false;

        public f()
        {
            InitializeComponent();
        }
        public void log(string t)
        {
            txt.Text = t;
            refresh();
        }
        public void refresh()
        {
            Update();
            Refresh();
            Application.DoEvents();
        }
        private void f_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = CloseLock;
                if (e.Cancel) MessageBox.Show("Nie można teraz przerwać!");
            }
        }
        private void f_Shown(object sender, EventArgs e)
        {
            log("Initializing...");
            killer();
            checkVB();
            cleanup();
            checkUser();
            checkSourceDir();
            getCurrentVersion();
            getFileList();
            getTempDir();
            downloadFiles();
            copyFiles();
            finalize();
            Close();
        }

        private void getTempDir()
        {
            log("Preparing temporary directory...");
            tempDir = System.IO.Path.GetTempPath();
            tempDir = Path.Combine(tempDir, ".develvminstaller");
            if (Directory.Exists(tempDir))
            {
                log("Cleaning [" + tempDir + "]...");
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);
        }
        private void finalize()
        {
            log("Clearing temporary directory...");
            Directory.Delete(tempDir, true);

            log("Adding autostart reg entry...");
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (regkey != null)
            {
                regkey.SetValue("Devel VM", mainexe);
                regkey.Close();
            }
            
            string smDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            if (Directory.Exists(Path.Combine(smDir, "Devel VM")))
            {
                log("Clearing [" + Path.Combine(smDir, "Devel VM") + "]...");
                Directory.Delete(Path.Combine(smDir, "Devel VM"), true);
            }
            smDir = Path.Combine(smDir, "Devel VM");
            log("Creating [" + Path.Combine(smDir, "Devel VM") + "]...");
            Directory.CreateDirectory(smDir);

            log("Create StartMenu shortcut...");
            try
            {
                ShellLink link = new ShellLink();
                link.Target = mainexe;
                link.WorkingDirectory = Path.GetDirectoryName(mainexe);
                link.IconPath = mainexe;
                link.IconIndex = 0;
                link.Description = "Devel VM Manager";
                link.Save(Path.Combine(smDir, "Devel VM.lnk"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create DeskTop Shortcut...\r\n" +
                                "App_Path = " + mainexe + "\r\n" +
                                "Exception: " + ex.Message.ToString(),
                                "ShortCut Creation Error:");
                Program.Exterminate();
            }

            pb.Value = 100;
            

            if (rebootRequired)
            {
                log("Finished! Reboot required...");
                MessageBox.Show("Uruchom ponownie komputer aby korzystać z Devela! W razie problemów z uruchomieniem maszyny po restarcie, skontaktuj się z administratorami.", "Devel VM Installer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                log("Finished! Starting app...");
                Program.execute(mainexe, "/r", true, true);
            }
        }
        private void copyFiles()
        {
            int step = 10 / filenames.Count;
            string installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Devel VM");

            log("Verifying directory [" + installDir + "]...");
            if (Directory.Exists(installDir))
            {
                log("Removing [" + installDir + "]...");
                Directory.Delete(installDir, true);
            }
            log("Creating directory [" + installDir + "]...");
            //DirectorySecurity sec = new DirectorySecurity(Environment.GetFolderPath(Environment.SpecialFolder.Personal), AccessControlSections.All);
            try
            {
                Directory.CreateDirectory(installDir/*, sec*/);
            }
            catch (Exception)
            {
                log("Unable to create " + installDir + " :(");
                MessageBox.Show("Unable to create " + installDir + " ! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Exterminate();
            }

            foreach (string fn in filenames)    
            {
                log("Copying [" + fn + "]...");
                System.IO.File.Copy(Path.Combine(tempDir, fn), Path.Combine(installDir, fn), true);
                if (Path.GetExtension(Path.Combine(installDir, fn)) == ".exe")
                {
                    mainexe = Path.Combine(installDir, fn);
                }
                pb.Value += step;
                refresh();
            }
            if (mainexe == "")
            {
                log("No executable found :(");
                MessageBox.Show("No executable file found in release! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Exterminate();
            }
            pb.Value = 95;
            refresh();
        }
        private void downloadFiles()
        {
            log("Downloading " + filenames.Count.ToString() + " files...");
            int step = 70 / filenames.Count;
            foreach (string fn in filenames)
            {
                log("Downloading [" + fn + "]...");
                string url = remoteBaseUrl + @"Devel_VM_" + remoteVer.Replace(".", "_") + "/" + fn;
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, Path.Combine(tempDir, fn));
                pb.Value += step;
                refresh();
            }
            pb.Value = 85;
            refresh();
        }
        private void getFileList()
        {
            log("Fetching app file list...");
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(remoteBaseUrl + @"Devel_VM_" + remoteVer.Replace(".", "_") + "/filelist.txt");
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            Stream responseStream = httpResponse.GetResponseStream();
            using (StreamReader sr = new StreamReader(responseStream))
            {
                string line;
                filenames.Clear();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "filelist.txt" && line.Contains("."))
                        filenames.Add(line);
                }
            }
            responseStream.Close();
            pb.Value = 15;
            refresh();
        }
        private void getCurrentVersion()
        {
            log("Fetching current version...");
            remoteVer = Program.getRemoteVersion();
            if (remoteVer == "0")
            {
                MessageBox.Show("Could not fetch version! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Exterminate();
            }
            log("Current version: " + remoteVer);
        }
        private void checkSourceDir()
        {
            string dir = @"C:\DEVEL";
            string shareName = "DEVEL";
            log("Verifying directory [" + dir + "]...");
            if (!Directory.Exists(dir))
            {
                log("Creating directory [" + dir + "]...");
                try
                {
                    DirectorySecurity sec = new DirectorySecurity(Environment.GetFolderPath(Environment.SpecialFolder.Personal), AccessControlSections.All);
                    sec.AddAccessRule(new FileSystemAccessRule(
                        "vbox",
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow
                    ));
                    Directory.CreateDirectory(dir, sec);
                }
                catch (Exception)
                {
                    try
                    {
                        log("Retry Creating directory [" + dir + "]...");
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception)
                    {
                        log("Unable to create " + dir + " :(");
                        MessageBox.Show("Unable to create " + dir + " ! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Program.Exterminate();
                    }
                }
                
            }

            log("Verifying share [" + shareName + "]...");
            Win32Share oldshare = Win32Share.GetNamedShare(shareName);
            if (oldshare != null)
            {
                if (oldshare.Path.ToLower() != dir.ToLower())
                {
                    log("Share incorrect! Fixing...");
                    oldshare.Delete();
                }
                else
                {
                    return;
                }
            }
            else
            {
                log("Share not found! Creating...");
            }
            string debug = Program.execute("net", "share " + shareName + "=" + dir + " /GRANT:vbox,FULL");
            oldshare = Win32Share.GetNamedShare(shareName);
            if (oldshare == null)
            {
                log("Could not share directory " + debug);
                MessageBox.Show("Could not share [" + dir + "] directory! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Exterminate();
            }
            /*Win32Share.MethodStatus createResult = Win32Share.Create(dir, shareName, Win32Share.ShareType.DiskDrive, 20, "Source code", null);
            if (createResult != Win32Share.MethodStatus.Success){}*/
        }
        private void checkUser()
        {
            string username = "vbox";
            log("Verifying user vbox...");
            bool ok = false;
            try
            {
                ok = ((SecurityIdentifier)(new NTAccount(username)).Translate(typeof(SecurityIdentifier))).IsAccountSid();
            }
            catch (IdentityNotMappedException) { }
            if (!ok)
            {
                log("User vbox not found. Creating...");
                Program.execute("net", @" user " + username + " 123qwe /ADD /EXPIRES:NEVER /PASSWORDCHG:NO /PASSWORDREQ:YES");
                try
                {
                    ok = ((SecurityIdentifier)(new NTAccount(username)).Translate(typeof(SecurityIdentifier))).IsAccountSid();
                }
                catch (IdentityNotMappedException) { ok = false; }
                if (!ok)
                {
                    log("Error creating user!");
                    MessageBox.Show("Could not create user vbox! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.Exterminate();
                }
            }
            log("Verifying if user " + username + " is hidden...");
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", true).CreateSubKey("SpecialAccounts").CreateSubKey("UserList");
            if (regkey != null)
            {
                log("Checking registry for previous entry...");
                int entry = (int)regkey.GetValue("vbox", 1);
                if (entry == 0)
                {
                    log("Registry entry found and correct...");
                }
                else
                {
                    log("Fixing registry entry...");
                    regkey.SetValue("vbox", 0);
                }
                regkey.Close();
            }
            else
            {
                log("Error while accessing registry...");
            }
        }
        private void killer()
        {
            log("Killing all instances of used processes...");
            string[] pnames = { "VirtualBox", "VBoxHeadless", "VBoxSVC", "VBoxNetDHCP", "Beta_Manager", "Devel_VM" };
            foreach (string pname in pnames)
                foreach (Process prc in Process.GetProcessesByName(pname))
                    if (Process.GetCurrentProcess().Id != prc.Id)
                        prc.Kill();
        }
        private void cleanup()
        {
            log("Checking previous installations...");
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (regkey != null)
            {
                log("Checking registry for startup entry...");
                String entry = (String)regkey.GetValue("BetaManager", null);
                if (entry != null)
                {
                    log("Removing old startup entry from registry...");
                    regkey.DeleteValue("BetaManager", false);
                }
                regkey.Close();
            }

            log("Checking known folders for old files...");
            String UserDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..");
            if (Directory.Exists(Path.Combine(UserDir, "Devel_VM")))
            {
                log("Removing [" + Path.Combine(UserDir, "Devel_VM") + "]...");
                Directory.Delete(Path.Combine(UserDir, "Devel_VM"), true);
            }
            if (Directory.Exists(Path.Combine(UserDir, "Beta Manager")))
            {
                log("Removing [" + Path.Combine(UserDir, "Beta Manager") + "]...");
                Directory.Delete(Path.Combine(UserDir, "Beta Manager"), true);
            }

            if (Directory.Exists(Path.Combine(UserDir, "Devel VM")))
            {
                log("Removing [" + Path.Combine(UserDir, "Devel VM") + "]...");
                Directory.Delete(Path.Combine(UserDir, "Devel VM"), true);
            }

            log("Checking Start menu...");
            string smDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            if (Directory.Exists(Path.Combine(smDir, "Beta Manager")))
            {
                log("Removing [" + Path.Combine(smDir, "Beta Manager") + "]...");
                Directory.Delete(Path.Combine(smDir, "Beta Manager"), true);
            }
            if (Directory.Exists(Path.Combine(smDir, "Devel VM")))
            {
                log("Removing [" + Path.Combine(smDir, "Devel VM") + "]...");
                Directory.Delete(Path.Combine(smDir, "Devel VM"), true);
            }
        }
        bool vbchecklasttry = false;
        private void checkVB()
        {
            bool ok = false;
            log("Checking VirtualBox...");
            RegistryKey vbox_regkey = Registry.LocalMachine.OpenSubKey(@"Software\Oracle\VirtualBox", false);
            if (vbox_regkey != null)
            {
                String vbox_version = (String)vbox_regkey.GetValue("VersionExt", null);
                if (vbox_version != null)
                {
                    log("VirtualBox " + vbox_version + " found...");
                    if (vbox_version != vboxVer) //TODO: Add grabbing correct version
                    {
                        log("Wrong VirtualBox version!");
                    }
                    else
                    {
                        ok = true;
                    }
                }
                else
                {
                    log("VirtualBox installation currupted...");
                }
                vbox_regkey.Close();
            }
            else
            {
                log("VirtualBox not found...");
            }

            if (!ok)
            {
                if (vbchecklasttry)
                {
                    log("VirtualBox installation not verified!");
                    MessageBox.Show("Be sure to install right version of VirtualBox! Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.Exterminate();
                }
                else
                {
                    MessageBox.Show("Wymagana jest aktualizacja VirtualBoxa. Uruchomiona zostanie instalacja wymaganej wersji. Po instalacji VB i Devela WYMAGANE będzie ponowne uruchomienie komputera!", "Devel VM Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rebootRequired = true;
                    InstallVB();
                }
            }

        }
        private void InstallVB()
        {
            log("Starting VirtualBox installer...");
            try
            {
                Program.execute(vboxInstallBase + vboxVer + ".exe", @"");
            }
            catch (Exception)
            {
                log("VirtualBox installer problem!");
                MessageBox.Show("Installer not found. Be sure to install right version of VirtualBox. Exterminate!", "Devel VM Installer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Exterminate();
            }
            
            vbchecklasttry = true;
            checkVB();
        }
    }
}

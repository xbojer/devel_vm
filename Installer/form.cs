﻿using System;
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

namespace DVMinstaller
{
    public partial class f : Form
    {
        bool CloseLock = false;
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
            downloadFiles();
            copyFiles();
            cleanupTemp();
            finalize();

            String DestFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        }

        private void finalize()
        {
            //throw new NotImplementedException();
        }

        private void cleanupTemp()
        {
            //throw new NotImplementedException();
        }

        private void copyFiles()
        {
            //throw new NotImplementedException();
        }

        private void downloadFiles()
        {
            //throw new NotImplementedException();
        }

        private void getFileList()
        {
            //throw new NotImplementedException();
        }

        private void getCurrentVersion()
        {
            //throw new NotImplementedException();
        }

        private void checkSourceDir()
        {
            string dir = @"C:\DEVEL";
            string shareName = "DEVEL";
            log("Verifying directory [" + dir + "]...");
            if (!Directory.Exists(dir))
            {
                log("Creating directory [" + dir + "]...");
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
            string[] pnames = { "Devel_VM", "Beta_Manager", "VirtualBox", "VBoxSVC", "VBoxHeadless" };
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

            log("Checking Start menu...");
            string smDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            if (Directory.Exists(Path.Combine(smDir, "Beta Manager")))
            {
                log("Removing [" + Path.Combine(smDir, "Beta Manager") + "]...");
                Directory.Delete(Path.Combine(smDir, "Beta Manager"), true);
            }
        }

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
                    if (vbox_version != "4.2.2") //TODO: Add grabbing correct version
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
                InstallVB();
            }

        }

        private void InstallVB()
        {
            throw new NotImplementedException();
        }
    }
}
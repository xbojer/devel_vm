using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VirtualBox;

namespace Devel_VM
{
    class VirtualMachine
    {
        private String ImgPath;
        private String MachineName;
        private String VbApiVersion = "4_1";
        
        public class MachineReadiness
        {
            public bool VersionRemote = true;
            public bool VersionLocal = true;
            public bool Installed = false;
            public bool API = false;

            public bool getReadyOnline()
            {
                return API && Installed && VersionLocal && VersionRemote;
            }
            public bool getReadyOffline()
            {
                return API && Installed && VersionRemote;
            }
        }
        public enum State
        {
            Off,
            Busy,
            On,
            Operational,
            Error,
            Unknown
        }

        public MachineReadiness MachineReady = new MachineReadiness();
        public State Status;

        private VirtualBox.VirtualBox vb;
        private IMachine Machine;
        public Session Session;

        #region Events
        private VBoXEventL1 EvListener;
        private VBoxEventType[] EvTypes = { VBoxEventType.VBoxEventType_Any };

        public delegate void VmEvent(String msg, String title, int priority);
        public VmEvent OnVmEvent;
        public void OnEvent(String msg, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, "BetaManager: VM", priority);
            }
        }
        #endregion

        #region Init
        public VirtualMachine()
        {
            MachineName = Properties.Settings.Default.vm_name;
            ImgPath = Properties.Settings.Default.path_image.Replace("{vm_name}", MachineName);
            
        }

        public void initMachine()
        {
            vb = new VirtualBox.VirtualBox();
            if (VbApiVersion != vb.APIVersion)
            {
                OnEvent("Niezgodna wersja API", 3);
                return;
            }
            MachineReady.API = true;
            Session = new Session();

            while (!MachineReady.Installed)
            {
                Application.DoEvents();
                try
                {
                    Machine = vb.FindMachine(MachineName);
                    MachineReady.Installed = true;
                }
                catch (Exception)
                {
                    try
                    {
                        IMachine tmpmach = vb.FindMachine(MachineName + "_installing");
                        Rename(MachineName + "_installing", MachineName);
                    }
                    catch (Exception)
                    {
                        Install();
                    }
                }
            }
            Program.NL.OnReset += delegate(string auth)
            {
                Packet p = new Packet();
                p.dataIdentifier = Packet.DataIdentifier.Pong;
                p.message = "Resetting";
                Network_Broadcast.send(p);
                this.Restart();
            };
        }
        #endregion

        #region Machine locking
        private void relock()
        {
            unlock();
            if (Session.State == SessionState.SessionState_Unlocked)
            {
                try
                {
                    Machine.LockMachine(Session, LockType.LockType_Shared);
                }
                catch (Exception)
                {
                    OnEvent("Nie udało się założyć locka", 0);
                    #if DEBUG
                    throw;
                    #endif
                }
            }
        }
        private void unlock()
        {
            if (Session == null) return;
            if (Session.State == SessionState.SessionState_Locked)
            {
#if DEBUG
                OnEvent("Zdejmowanie locka ok", 0);
#endif
                Session.UnlockMachine();
            }
            else
            {
#if DEBUG
                OnEvent("Zdejmowanie locka nieudane", 0);
#endif
            }
        }
        #endregion
        void MachineConfig()
        {
            if (!MachineReady.getReadyOffline() || Status != State.Off) return;
            try
            {
                relock();
                Session.Machine.MemorySize = 768;
                Session.Machine.VRAMSize = 24;
                Session.Machine.SetBootOrder(1, DeviceType.DeviceType_HardDisk);
                Session.Machine.SetBootOrder(2, DeviceType.DeviceType_Null);
                Session.Machine.SetBootOrder(3, DeviceType.DeviceType_Null);
                Session.Machine.SetBootOrder(4, DeviceType.DeviceType_Null);
                if (vb.Host.ProcessorOnlineCount > 1 && Properties.Settings.Default.vm_multicore) //performance degradation with more than one core
                {
                    Session.Machine.CPUCount = 2;
                    if (vb.Host.ProcessorOnlineCount > 2)
                    {
                        Session.Machine.CPUExecutionCap = 100;
                    }
                    else
                    {
                        Session.Machine.CPUExecutionCap = 80;
                    }
                }
                else
                {
                    Session.Machine.CPUCount = 1;
                    Session.Machine.CPUExecutionCap = 100;
                }
                Session.Machine.SaveSettings();
#if DEBUG
                OnEvent("Config ok", 0);
#endif
            }
            catch (Exception)
            {
                OnEvent("Wystąpił problem podczas ustawiania maszyny", 2);
            }
            unlock();
        }
        #region Machine state control
        public int getVersion(bool offline = true)
        {
            int v = -1;
            string ver = "0";
            if (MachineReady.API && MachineReady.Installed && offline)
            {
                relock();
                ver = Session.Machine.GetExtraData("BM/Version");
            }
            else
            {
                if (!MachineReady.getReadyOffline() || Status != State.Operational) return 0;
                ver = Program.VM.exec("/bin/cat", "/etc/devel_version").Trim();
            }
            MachineReady.VersionLocal = int.TryParse(ver, out v);
            return v;
        }
        public void Start()
        {
            if (!MachineReady.getReadyOffline())
            {
                OnEvent("Maszyna nie jest gotowa (Start)", 3);
                return;
            }
            relock();
            if (Machine.State != MachineState.MachineState_Running)
            {
                if (Machine.State != MachineState.MachineState_PoweredOff && Machine.State != MachineState.MachineState_Aborted)
                {
                    Session.Console.PowerDown();
                }
                unlock();
                MachineConfig();
                OnEvent("Uruchamianie maszyny", 1);
                Machine.LaunchVMProcess(Session, "headless", "VBETAM=1").WaitForCompletion(-1);
            }
            else
            {
                OnEvent("Maszyna już uruchomiona", 1);
            }
            AssureEvents();
        }
        public void PowerOff(bool kill)
        {
            if (!MachineReady.getReadyOffline())
            {
                OnEvent("Maszyna nie jest gotowa (PowerOff)", 3);
                return;
            }
            relock();
            if (Machine.State == MachineState.MachineState_PoweredOff || Machine.State == MachineState.MachineState_Aborted) return;
            IConsole con = Session.Console;
            if (kill)
            {
                OnEvent("Odłączanie maszyny", 1);
                con.PowerDown();
            }
            else
            {
                OnEvent("Wyłączanie maszyny", 1);
                con.PowerButton();
            }
        }
        internal void Restart()
        {
            if (!MachineReady.getReadyOffline())
            {
                OnEvent("Maszyna nie jest gotowa (Restart)", 3);
                return;
            }
            relock();
            IConsole con = Session.Console;
            OnEvent("Restartowanie", 1);
            con.Reset();
        }
        #endregion
        #region Events handling
        private void AssureEvents()
        {
            if (!MachineReady.getReadyOffline())
            {
                OnEvent("Maszyna nie jest gotowa (Events)", 3);
                return;
            }
            relock();
            if (EvListener != null) return;
            EvListener = new VBoXEventL1();
            Session.Console.EventSource.RegisterListener(EvListener, EvTypes, 1);
            
        }
        internal class VBoXEventL1 : IEventListener
        {
            public void HandleEvent(IEvent aEvent)
            {
                if (aEvent.Type == VBoxEventType.VBoxEventType_OnEventSourceChanged)
                {
                    IEventSourceChangedEvent ev = (IEventSourceChangedEvent)aEvent;

                    if (ev.Add == 0)
                    {
                        //Program.VM.OnEvent("Eventy off", "VBox Event1", 0);
                    }
                    else
                    {
                        //Program.VM.OnEvent("Eventy on", "VBox Event1", 0);
                    }
                }
                else if (aEvent.Type == VBoxEventType.VBoxEventType_OnStateChanged)
                {
                    IStateChangedEvent ev = (IStateChangedEvent)aEvent;
                    if (ev.State == MachineState.MachineState_PoweredOff)
                    {
                        Program.VM.OnEvent("Maszyna wyłączona", 1);
                    }
                    else
                    {
                        #if DEBUG
                        Program.VM.OnEvent(aEvent.Type.ToString(), 0);
                        #endif
                    }
                }
                else if (aEvent.Type == VBoxEventType.VBoxEventType_OnAdditionsStateChanged)
                {
                    if (Program.VM.Session.Console.Guest.AdditionsRunLevel == AdditionsRunLevelType.AdditionsRunLevelType_Userland)
                    {
                        Program.VM.Session.Console.Machine.SetGuestPropertyValue("VBOX_USER_NAME", Program.username);
                        Program.VM.Session.Console.Machine.SetGuestPropertyValue("VBOX_USER_IDENTITY", Program.identity);
                        Program.VM.OnEvent("Maszyna gotowa do pracy", 1);
                    }
                }
            }
        }
        #endregion
        public void Tick()
        {
            if (!MachineReady.getReadyOffline())
            {
                Status = State.Busy;
                return;
            }
            if (!MachineReady.getReadyOnline())
            {
                Status = State.Busy;
                return;
            }
            relock();
            #region Translate MachineState
            switch (Machine.State)
            {
                case MachineState.MachineState_Aborted:
                    Status = State.Off;
                    break;
                case MachineState.MachineState_DeletingSnapshot:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_DeletingSnapshotOnline:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_DeletingSnapshotPaused:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_FaultTolerantSyncing:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_FirstTransient:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_LastTransient:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_LiveSnapshotting:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_Null:
                    Status = State.Error;
                    break;
                case MachineState.MachineState_Paused:
                    Status = State.Off;
                    break;
                case MachineState.MachineState_PoweredOff:
                    Status = State.Off;
                    break;
                case MachineState.MachineState_Restoring:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_RestoringSnapshot:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_Saving:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_Starting:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_Stopping:
                    Status = State.Busy;
                    break;
                case MachineState.MachineState_Stuck:
                    Status = State.Error;
                    break;
                case MachineState.MachineState_Running:
                    Status = State.On;
                    break;
                default:
                    Status = State.Unknown;
                    break;
            }
            #endregion
            if (Status == State.On && Session.Console.Guest.AdditionsRunLevel==AdditionsRunLevelType.AdditionsRunLevelType_Userland)
            {
                Status = State.Operational;
            }
        }
        #region Remote process execution
        public void exec_api(String cmd, String[] args)
        {
            if (!MachineReady.getReadyOffline())
            {
                OnEvent("Maszyna nie jest gotowa (Start)", 3);
                return;
            }
            relock();
            if(Session.Console.Guest.AdditionsRunLevel!=AdditionsRunLevelType.AdditionsRunLevelType_Userland) return;
            uint pid = 0;
            String[] env = {"BETAMGR=1"};
            IProgress progress = Session.Console.Guest.ExecuteProcess(cmd, 0, args, env, "fotka", "@fotka", 0, out pid);

            IGuest gu = Session.Console.Guest;
            int[] asd = (int[])gu.GetProcessOutput(pid, 0, 5000, (long)100);
            
            progress.WaitForCompletion(-1);

            throw new Exception("VirtualBox COM API error?");
        }
        public string exec(String cmd, String args)
        {
            string filename = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";
            args = " guestcontrol \"" + MachineName + "\" execute --image \"" + cmd + "\" --username=\"fotka\" --password=\"@fotka\" --wait-exit --wait-stdout " + args;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            startInfo.FileName = filename;
            startInfo.Arguments = args;
            
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd();
        }
        #endregion
        #region 
        public void Install()
        {
            if (!MachineReady.API || !MachineReady.VersionRemote) return;
            IAppliance ia = vb.CreateAppliance();
            ia.Read(ImgPath).WaitForCompletion(-1);
            ia.Interpret();
            IVirtualSystemDescription[] descs = (IVirtualSystemDescription[])ia.VirtualSystemDescriptions;
            if (descs.Length != 1)
            {
                throw new Exception("Bledny obraz maszyny");
            }

            System.Array aRefs, aOvfValues, aVBoxValues, aExtraConfigValues, aTypes;
            VirtualSystemDescriptionType[] Types;
            String[] VBoxValues, ExtraConfigValues;
            IVirtualSystemDescription desc = descs[0];

            desc.GetDescription(out aTypes, out aRefs, out aOvfValues, out aVBoxValues, out aExtraConfigValues);

            Types = (VirtualSystemDescriptionType[])aTypes;
            VBoxValues = (String[])aVBoxValues;
            ExtraConfigValues = (String[])aExtraConfigValues;
            List<int> enabled = new List<int>();

            int imgversion = 0;

            for(int i=0; i<Types.Length;i++)
	        {
		        enabled.Add(1);
                if (Types[i] == VirtualSystemDescriptionType.VirtualSystemDescriptionType_Version)
                {
                    if (!int.TryParse(VBoxValues[i], out imgversion))
                    {
                        imgversion = 0;
                    }
                }
                if (Types[i]== VirtualSystemDescriptionType.VirtualSystemDescriptionType_Name)
                {
                    VBoxValues[i] += "_installing";
                }
                if (Types[i] == VirtualSystemDescriptionType.VirtualSystemDescriptionType_HardDiskImage)
                {
                    string[] deli = {@"\"};
                    string[] path = VBoxValues[i].Split(deli, StringSplitOptions.RemoveEmptyEntries);
                    path[(path.Length-2)] += "_installing";
                    VBoxValues[i] = String.Join(@"\", path);
                }
	        }

            aVBoxValues = VBoxValues;
            aExtraConfigValues = ExtraConfigValues;

            descs[0].SetFinalValues((Array)(enabled.ToArray()), aVBoxValues, aExtraConfigValues);
            ImportOptions[] opts = {ImportOptions.ImportOptions_KeepAllMACs};
            
            ia.ImportMachines(opts).WaitForCompletion(-1);

        }
        public bool Uninstall(string name)
        {
            if (Session != null)
                Session.UnlockMachine();
            IMachine mach;
            try
            {
                mach = vb.FindMachine(name);
            }
            catch (Exception)
            {
                return false;
            }
            String t = mach.SettingsFilePath;
            IMedium[] med = (IMedium[])mach.Unregister(CleanupMode.CleanupMode_Full);
            mach.Delete(med).WaitForCompletion(-1);
            /* Due to some strange behaviour with VB API App needs to separately delete medium and direcotry */
            foreach(IMedium m in med) {
                MediumState s = m.LockWrite();
                if (s == MediumState.MediumState_LockedWrite)
                {
                    m.Reset().WaitForCompletion(-1);
                }
                m.UnlockWrite();
                m.DeleteStorage().WaitForCompletion(-1);
            }
            DirectoryInfo di = Directory.GetParent(t);
            di.Delete(true);
            return true;
        }
        private void Rename(string _old, string _new)
        {
            if(Session != null)
                Session.UnlockMachine();
            Session tmps = new VirtualBox.Session();;
            try
            {
                IMachine mach = vb.FindMachine(_old);
                mach.LockMachine(tmps, LockType.LockType_Write);

                tmps.Machine.Name = _new;

                tmps.Machine.SaveSettings();
                tmps.UnlockMachine();
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                if(tmps.State==SessionState.SessionState_Locked)
                    tmps.UnlockMachine();
            }
        }
        #endregion
    }

}

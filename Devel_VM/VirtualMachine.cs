using System;
using System.Collections.Generic;
using System.Text;
using VirtualBox;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Devel_VM
{
    class VirtualMachine
    {
        private const String imgpath = @"\\alpha\instale\Devel.102.ova";

        private VirtualBox.VirtualBox vb;

        private IMachine Machine;
        public Session Session;

        public bool MachineReady = false;

        public enum State
        {
            Off,
            Busy,
            On,
            Operational,
            Error,
            Unknown
        }

        #region Events
        private VBoXEventL1 EvListener;
        private VBoxEventType[] EvTypes = { VBoxEventType.VBoxEventType_Any };

        public VmEvent OnVmEvent;
        public delegate void VmEvent(String msg, String title, int priority);
        public void OnEvent(String msg, String title, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, title, priority);
            }
        }
        #endregion

        public State Status;

        private string api_ver = "4_1";

        public VirtualMachine()
        {
            vb = new VirtualBox.VirtualBox();
            if (api_ver != vb.APIVersion)
            {
                throw new Exception("Program nie jest zgodny z zainstalowana wersja VirtualBoxa.");
            }
            while (!MachineReady)
            {
                Application.DoEvents();
                try
                {
                    Machine = vb.FindMachine("Devel");
                    MachineReady = true;
                }
                catch (Exception)
                {
                    Install();
                    //throw new Exception("Nie znaleziono maszyny Devel.");
                }
            }
            Session = new Session();
            lock_share();
        }
        public int getVersion()
        {
            string result = Program.VM.exec("/bin/cat", "/etc/devel_version").Trim();
            int v = -1;
            bool valid = int.TryParse(result, out v);
            if (!valid)
            {
                throw new Exception("Nie udało się ustalić wersji wirtualnej maszyny");
            }
            return v;
        }
        #region Machine locking
        private void lock_share()
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
                    //throw;
                }
            }
        }
        internal void unlock()
        {
            if (Session.State == SessionState.SessionState_Locked)
            {
                Session.UnlockMachine();
            }
        }
        #endregion
        void MachineConfig()
        {
            try
            {
                lock_share();
                Session.Machine.MemorySize = 1024;
                Session.Machine.VRAMSize = 24;
                Session.Machine.SetBootOrder(1, DeviceType.DeviceType_HardDisk);
                Session.Machine.SetBootOrder(2, DeviceType.DeviceType_Null);
                Session.Machine.SetBootOrder(3, DeviceType.DeviceType_Null);
                Session.Machine.SetBootOrder(4, DeviceType.DeviceType_Null);
                if (vb.Host.ProcessorOnlineCount > 1 && false)
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
                unlock();
            }
            catch (Exception)
            {
                OnEvent("Error configuring machine", "VirtualMachine Config", 3);
            }
        }
        #region Machine state control
        public void Start()
        {
            lock_share();
            if (Machine.State != MachineState.MachineState_Running)
            {
                if (Machine.State != MachineState.MachineState_PoweredOff && Machine.State != MachineState.MachineState_Aborted)
                {
                    Session.Console.PowerDown();
                }
                unlock();
                MachineConfig();
                OnEvent("Starting", "Beta state", 1);
                Machine.LaunchVMProcess(Session, "headless", "VBETAM=1").WaitForCompletion(-1);
            }
            else
            {
                OnEvent("Already running", "Beta state", 1);
            }
            AssureEvents();
        }
        public void PowerOff(bool kill)
        {
            lock_share();
            if (Machine.State == MachineState.MachineState_PoweredOff || Machine.State == MachineState.MachineState_Aborted) return;
            IConsole con = Session.Console;
            if (kill)
            {
                OnEvent("Power Down", "Beta state", 1);
                con.PowerDown();
            }
            else
            {
                OnEvent("Shutting down", "Beta state", 1);
                con.PowerButton();
            }
        }
        internal void Restart()
        {
            lock_share();
            IConsole con = Session.Console;
            OnEvent("Reset", "Beta state", 1);
            con.Reset();
        }
        #endregion
        #region Events handling
        private void AssureEvents()
        {
            lock_share();
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
                        Program.VM.OnEvent("Powered down", "Beta state", 1);
                    }
                    else
                    {
                        Program.VM.OnEvent(aEvent.Type.ToString(), "VBox Event", 0);
                    }
                }
                else if (aEvent.Type == VBoxEventType.VBoxEventType_OnAdditionsStateChanged)
                {
                    if (Program.VM.Session.Console.Guest.AdditionsRunLevel == AdditionsRunLevelType.AdditionsRunLevelType_Userland)
                    {
                        Program.VM.OnEvent("Operational", "Beta state", 0);
                    }
                }
            }
        }
        #endregion
        public void Tick()
        {
            if (!MachineReady)
            {
                Status = State.Busy;
                return;
            }
            lock_share();
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
            /*lock_share();
            if(Session.Console.Guest.AdditionsRunLevel!=AdditionsRunLevelType.AdditionsRunLevelType_Userland) return;
            uint pid = 0;
            String[] env = {"BETAMGR=1"};
            IProgress progress = Session.Console.Guest.ExecuteProcess(cmd, 0, args, env, "fotka", "@fotka", 0, out pid);

            IGuest gu = Session.Console.Guest;
            byte[] asd = (byte[])gu.GetProcessOutput(pid, 0, 5000, (long)100);
            
            progress.WaitForCompletion(-1);
             */
            throw new Exception("VirtualBox COM API error?");
        }
        public string exec(String cmd, String args)
        {
            string filename = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";
            args = " guestcontrol \"Devel\" execute --image \"" + cmd + "\" --username=\"fotka\" --password=\"@fotka\" --wait-exit --wait-stdout " + args;

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
            IAppliance ia = vb.CreateAppliance();
            ia.Read(imgpath).WaitForCompletion(-1);
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
            List<bool> enabled = new List<bool>();

            for(int i=0; i<Types.Length;i++)
	        {
		        enabled.Add(true);
                if (Types[i]== VirtualSystemDescriptionType.VirtualSystemDescriptionType_Name)
                {
                    VBoxValues[i] += "_installing";
                }
	        }

            aVBoxValues = VBoxValues;
            aExtraConfigValues = ExtraConfigValues;

            descs[0].SetFinalValues(enabled.ToArray(), aVBoxValues, aExtraConfigValues);


        }
        public void Uninstall()
        {
            if (Status != State.Off)
            {
                PowerOff(true);
            }
            unlock();
            IMedium[] med = (IMedium[]) Machine.Unregister(CleanupMode.CleanupMode_Full);
            Machine.Delete(med);
            Machine.SaveSettings();
        }
        #endregion
    }

}

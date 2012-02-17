using System;
using System.Collections.Generic;
using System.Text;
using VirtualBox;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace Devel_VM
{
    class VirtualMachine
    {
        private VirtualBoxClass vb;

        private IMachine Machine;
        private static Session Session;

        public VmEvent OnVmEvent;

        public delegate void VmEvent(String msg, String title, int priority);

        public enum State
        {
            Off,
            Busy,
            On,
            Operational,
            Error,
            Unknown
        }

        private VBoXEventL1 EvListener;
        VBoxEventType[] EvTypes = { VBoxEventType.VBoxEventType_Any };

        public State Status;

        private string api_ver = "4_1";

        public void OnEvent(String msg, String title, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, title, priority);
            }
        }

        public VirtualMachine()
        {
            vb = new VirtualBoxClass();
            if (api_ver != vb.APIVersion)
            {
                throw new Exception("Program nie jest zgodny z zainstalowana wersja VirtualBoxa.");
            }
            try
            {
                Machine = vb.FindMachine("Devel");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono maszyny Devel.");
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
            try
            {
                Machine.LockMachine(Session, LockType.LockType_Shared);
            }
            catch (Exception)
            {
                throw new Exception("Nie udało się dobrać do maszyny.");
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
                OnEvent("Starting", "Beta state", 1);
                Machine.LaunchVMProcess(Session, "headless", "VBETAM=1").WaitForCompletion(-1);
            }
            else
            {
                OnEvent("Already running", "Beta state", 1);
            }
        }
        public void PowerOff(bool kill)
        {
            lock_share();
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
        #endregion
        public void Tick()
        {
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
                AssureEvents();
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
        internal class VBoXEventL1 : IEventListener
        {
            public void HandleEvent(IEvent aEvent)
            {
                if (aEvent.Type == VBoxEventType.VBoxEventType_OnEventSourceChanged)
                {
                    IEventSourceChangedEvent ev = (IEventSourceChangedEvent)aEvent;

                    if (ev.Add == 0)
                    {
                        //Program.VM.OnEvent(ev.Type.ToString(), "VBox Event1", 0);
                    }
                }
                else if(aEvent.Type == VBoxEventType.VBoxEventType_OnStateChanged) 
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
                else if(aEvent.Type == VBoxEventType.VBoxEventType_OnAdditionsStateChanged)
                {
                    if(VirtualMachine.Session.Console.Guest.AdditionsRunLevel==AdditionsRunLevelType.AdditionsRunLevelType_Userland) {
                        Program.VM.OnEvent("Operational", "Beta state", 0);
                    }
                }
            }
        }
    }

    
}

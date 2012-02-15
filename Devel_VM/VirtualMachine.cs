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
        private Session Session;

        public VmEvent OnVmEvent;

        public enum State
        {
            Off,
            Busy,
            On,
            Operational,
            Error,
            Unknown
        }

        private IEventListener EvListener;
        VBoxEventType[] EvTypes = { VBoxEventType.VBoxEventType_OnStateChanged };

        public State Status;

        private string api_ver = "4_1";

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
            Sanitize();
        }
        public void Start()
        {
            gracefulShutdown();
            Machine.LaunchVMProcess(Session, "headless", "VBETAM=1").WaitForCompletion(-1);
            EvListener = Session.Console.EventSource.CreateListener();
            Session.Console.EventSource.RegisterListener(EvListener, EvTypes, 0);
        }
        private void gracefulShutdown()
        {
            lock_share();
            if (Machine.State != MachineState.MachineState_PoweredOff)
            {
                if (Machine.State == MachineState.MachineState_Running)
                {
                    IEventSource es = Session.Console.EventSource;
                    IEventListener listener = es.CreateListener();
                    VBoxEventType[] aTypes = { VBoxEventType.VBoxEventType_OnStateChanged };
                    es.RegisterListener(listener, aTypes, 0);
                    Session.Console.PowerButton();
                    do
                    {
                        IEvent ev = es.GetEvent(listener, 30000);
                        if (ev != null)
                        {
                            IStateChangedEvent me = (IStateChangedEvent)ev;
                            ev.SetProcessed();
                            if (me.State == MachineState.MachineState_PoweredOff)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Session.Console.PowerDown().WaitForCompletion(10000);
                        }
                    } while (Machine.State != MachineState.MachineState_PoweredOff);
                }
                else
                {
                    Sanitize();
                }
                for (int timeout = 1; timeout < 25; timeout++)
                {
                    Thread.Sleep(200);
                    if (Machine.State == MachineState.MachineState_PoweredOff && Session.State == SessionState.SessionState_Unlocked && Machine.SessionState == SessionState.SessionState_Unlocked)
                    {
                        break;
                    }
                }
            }
            unlock();
        }
        public void PowerOff(bool kill)
        {
            lock_share();
            IConsole con = Session.Console;
            if (kill)
            {
                con.PowerDown();
            }
            else
            {
                con.PowerButton();
            }
        }
        internal void Restart()
        {
            lock_share();
            IConsole con = Session.Console;
            con.Reset();
        }
        private void Sanitize()
        {
            lock_share();
            if (Machine.State != MachineState.MachineState_PoweredOff && Session.State == SessionState.SessionState_Locked)
            {
                //Session.Console.PowerDown().WaitForCompletion(-1);
            }
            unlock();
        }
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
        public void Tick()
        {
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
            if (Session.State == SessionState.SessionState_Locked)
            {
                if (Status == State.On && Session.Console.Guest.AdditionsRunLevel==AdditionsRunLevelType.AdditionsRunLevelType_Userland)
                {
                    Status = State.Operational;
                }
                IEvent ev = Session.Console.EventSource.GetEvent(EvListener, 0);
                if (ev != null)
                {
                    IStateChangedEvent me = (IStateChangedEvent)ev;
                    ev.SetProcessed();
                    OnEvent(me.State.ToString(), 1);
                }
            }
        }
        private void OnEvent(String msg, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, priority);
            }
        }

        public delegate void VmEvent(String msg, int priority);

        public void exec2(String cmd, String[] args)
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
            string retMessage = String.Empty;

            /*
             * C:\Program Files\Oracle\VirtualBox\VBoxManage.exe guestcontrol "Devel" execute --image "/bin/ls" --username="fotka" --password="@fotka" --wait-exit --wait-stdout /tmp
             */
            string vbm = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";
            string vbargs = " guestcontrol \"Devel\" execute --image \"" + cmd + "\" --username=\"fotka\" --password=\"@fotka\" --wait-exit --wait-stdout ";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            startInfo.UseShellExecute = false;
            startInfo.Arguments = vbargs;
            startInfo.FileName = vbm;

            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd();
        }
    }
}

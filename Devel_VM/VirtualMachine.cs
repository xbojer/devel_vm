using System;
using System.Collections.Generic;
using System.Text;
using VirtualBox;
using System.Threading;

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
            throw new NotImplementedException();
        }

        private void Sanitize()
        {
            lock_share();
            if (Machine.State != MachineState.MachineState_PoweredOff && Session.State == SessionState.SessionState_Locked)
            {
                Session.Console.PowerDown().WaitForCompletion(-1);
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
                    Status = State.Operational;
                    break;
                default:
                    Status = State.Unknown;
                    break;
            }
            #endregion

            if (Session.State == SessionState.SessionState_Locked)
            {
                IEvent ev = Session.Console.EventSource.GetEvent(EvListener, 0);
                if (ev != null)
                {
                    IStateChangedEvent me = (IStateChangedEvent)ev;
                    ev.SetProcessed();
                    OnEvent(me.State.ToString(), 1);
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

        private void OnEvent(String msg, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, priority);
            }
        }

        public delegate void VmEvent(String msg, int priority);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VirtualBox;

namespace Devel_VM
{
    class VirtualMachine
    {
        private VirtualBoxClass vb;

        private IMachine Machine;
        private Session Session;

        public enum State
        {
            Off,
            Busy,
            Operational,
            Error,
            Unknown
        }

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
            if (Session.State != SessionState.SessionState_Locked)
            {
                try
                {
                    Machine.LockMachine(Session, LockType.LockType_Shared);
                }
                catch (Exception)
                {
                    throw new Exception("Nie udało się dobrać do maszyny.");
                }
            }
            if (Machine.State != MachineState.MachineState_PoweredOff)
            {
                if (Machine.State == MachineState.MachineState_Running)
                {
                    IEventSource es = Session.Console.EventSource;
                    IEventListener listener = es.CreateListener();
                    VBoxEventType[] aTypes = { VBoxEventType.VBoxEventType_OnStateChanged};
                    es.RegisterListener(listener, aTypes, 0);
                    Session.Console.PowerButton();
                    do
                    {
                        IEvent ev = es.GetEvent(listener, 30000);
                        if (ev == null)
                        {
                            IStateChangedEvent me = (IStateChangedEvent)ev;
                            Session.Console.PowerDown().WaitForCompletion(10000);
                            //break;
                            //System.Windows.Forms.MessageBox.Show(me.State.ToString());
                            ev.SetProcessed();
                        }
                    } while (true);
                }
                else
                {
                    Sanitize();
                }
            }
            if (Machine.SessionState != SessionState.SessionState_Unlocked)
            {
                Session.UnlockMachine();
            }
            Machine.LaunchVMProcess(Session, "headless", "VBETAM=1").WaitForCompletion(-1);
        }
        public void PowerOff(bool kill)
        {
            if (Session.State != SessionState.SessionState_Locked) return;
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
            unlock();
            try
            {
                Machine.LockMachine(Session, LockType.LockType_Shared);
            }
            catch (Exception)
            {
                throw new Exception("Nie udało się dobrać do maszyny.");
            }
            if (Machine.State != MachineState.MachineState_PoweredOff && Session.State == SessionState.SessionState_Locked)
            {
                Session.Console.PowerDown().WaitForCompletion(-1);
            }
            if (Session.State == SessionState.SessionState_Locked)
            {
                Session.UnlockMachine();
            }
        }

        public void Tick()
        {
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
        }


        internal void unlock()
        {
            if (Session.State == SessionState.SessionState_Locked)
            {
                Session.UnlockMachine();
            }
        }
    }
}

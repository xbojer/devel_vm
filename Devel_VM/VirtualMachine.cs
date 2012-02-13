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
            Error
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
        }
        public void Start()
        {

            if (Machine.State == MachineState.MachineState_PoweredOff)
            {
                Machine.LaunchVMProcess(Session, "headless", "");
            }
            else
            {
                //Machine.ShowConsoleWindow();
                //
            }
        }
        public void PowerOff(bool kill)
        {

        }


        internal void Restart()
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            switch (Machine.State)
            {
                case MachineState.MachineState_Aborted:
                    break;
                case MachineState.MachineState_DeletingSnapshot:
                    break;
                case MachineState.MachineState_DeletingSnapshotOnline:
                    break;
                case MachineState.MachineState_DeletingSnapshotPaused:
                    break;
                case MachineState.MachineState_FaultTolerantSyncing:
                    break;
                case MachineState.MachineState_FirstOnline:
                    break;
                case MachineState.MachineState_FirstTransient:
                    break;
                case MachineState.MachineState_LastTransient:
                    break;
                case MachineState.MachineState_LiveSnapshotting:
                    break;
                case MachineState.MachineState_Null:
                    break;
                case MachineState.MachineState_Paused:
                    break;
                case MachineState.MachineState_PoweredOff:
                    break;
                case MachineState.MachineState_Restoring:
                    break;
                case MachineState.MachineState_RestoringSnapshot:
                    break;
                case MachineState.MachineState_Saved:
                    break;
                case MachineState.MachineState_Saving:
                    break;
                case MachineState.MachineState_Starting:
                    break;
                case MachineState.MachineState_Stopping:
                    break;
                case MachineState.MachineState_Stuck:
                    break;
                default:
                    break;
            }
        }
    }
}

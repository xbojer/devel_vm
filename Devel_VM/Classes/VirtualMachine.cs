using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VirtualBox;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Pipes;
using Devel_VM.Classes;

namespace Devel_VM
{
    class VirtualMachine
    {
        private String ImgPath;
        public String MachineName;
        private String VbApiVersion = "4_2";

        public bool UpdateNeeded = false;
        
        public class MachineReadiness
        {
            public bool VersionRemote = false; // remote version got
            public bool VersionLocal = false; // machine version ok
            public bool Installed = false; // machine found
            public bool API = false; // vb api ok

            public bool getReadyOnline()
            {
                return API && Installed && VersionLocal && VersionRemote;
            }
            public bool getReadyOffline()
            {
                return API && Installed;
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

        public int RemoteVersion = 0;

        public SerialPipe TTY;
        
        string serial_prompt1 = "Enter passphrase: ";
        string serial_response1 = "qweqweqwe\n";

        #region Events
        private VBoXEventL1 EvListener;
        private VBoxEventType[] EvTypes = { VBoxEventType.VBoxEventType_Any };

        public delegate void VmEvent(String msg, String title, int priority);
        public VmEvent OnVmEvent;
        public void OnEvent(String msg, int priority)
        {
            if (OnVmEvent != null)
            {
                OnVmEvent(msg, "Devel VM Manager: VM", priority);
            }
        }
        #endregion

        #region Init
        public VirtualMachine()
        {
            MachineName = Properties.Settings.Default.vm_name;
            ImgPath = Properties.Settings.Default.path_image.Replace("{vm_name}", MachineName);
            RemoteVersion = getRemoteVersion();
        }
        public void initMachine()
        {
            if (MachineReady.getReadyOffline()) return;
            vb = new VirtualBox.VirtualBox();
            if (VbApiVersion != vb.APIVersion)
            {
                OnEvent("Niezgodna wersja API VB", 3);
                return;
            }
            MachineReady.API = true;
            Session = new Session();

            reInit();

            Program.NL.OnReset += delegate(string auth)
            {
                Packet p = new Packet();
                p.dataIdentifier = Packet.DataIdentifier.Pong;
                p.message = "Resetting";
                Network_Broadcast.send(p);
                this.Restart();
            };
        }
        private void deInit()
        {
            if (Machine != null)
            {
                if (Machine.State == MachineState.MachineState_Running)
                {
                    relock();
                    Session.Console.PowerDown().WaitForCompletion(-1);
                }
            }
            MachineReady.Installed = false;
            MachineReady.VersionLocal = false;
            Machine = null;
            unlock();
        }
        public void reInit()
        {
            while (!MachineReady.Installed)
            {
                Application.DoEvents();
                try
                {
                    Machine = vb.FindMachine(MachineName);
                    MachineReady.Installed = true;
                }
                catch (Exception e)
                {
                        try
                        {
                            IMachine tmpmach = vb.FindMachine(MachineName + "_installing");
                            Rename(MachineName + "_installing", MachineName);
                        }
                        catch (Exception)
                        {
                            DialogResult odp = MessageBox.Show("Nie znaleziono maszyny "+MachineName+". Zaciągnąć na nowo? (Nie == spróbuj ponownie)\n("+e.Message+")", "Błąd wczytywania VM", MessageBoxButtons.YesNoCancel);
                            if (odp == DialogResult.No) continue;
                            if (odp == DialogResult.Yes)
                            {
                                Install();
                            }
                            if (odp == DialogResult.Cancel)
                            {
                                return;
                            }
                        }

                }
            }

            checkVersion(false);
        }
        public bool checkVersion(bool verbose = true)
        {
            int lv = getVersion(true);
            RemoteVersion = getRemoteVersion();

            UpdateNeeded = lv < RemoteVersion;
            MachineReady.VersionLocal = !UpdateNeeded;

            if (verbose && UpdateNeeded) OnEvent("Lokalny obraz jest nieaktualny, zaktualizuj!", 2);

            return MachineReady.VersionLocal;
        }
        #endregion
        #region Machine locking
        private void relock()
        {
            unlock();
            if (Session == null) return;
            if (Session.State == SessionState.SessionState_Unlocked)
            {
                try
                {
                    Machine.LockMachine(Session, LockType.LockType_Shared);
                }
                catch (Exception)
                {
                    OnEvent("Nie udało się uzyskać dostępu do maszyny", 0);
                }
            }
        }
        public void unlock()
        {
            if (Session == null) return;
            if (Session.State == SessionState.SessionState_Locked)
            {
                Session.UnlockMachine();
            }
        }
        #endregion
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
        public int getRemoteVersion()
        {
            int rver = 0;
            try
            {
                using (StreamReader sr = new StreamReader(Properties.Settings.Default.path_imgver))
                {
                    String line;
                    if((line = sr.ReadLine()) != null)
                    {
                        int.TryParse(line, out rver);
                        if (rver > 0) MachineReady.VersionRemote = true;
                    }
                }
            }
            catch (Exception)
            {
                OnEvent("Nie udało się odczytać wersji zewnętrznego obrazu", 1);
            }
            return rver;
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
        public void PowerOff(bool kill, bool noevents = false)
        {
            if (!MachineReady.getReadyOffline())
            {
                if(!noevents) OnEvent("Maszyna nie jest gotowa (PowerOff)", 3);
                return;
            }
            relock();
            if (Machine.State == MachineState.MachineState_PoweredOff || Machine.State == MachineState.MachineState_Aborted) return;
            IConsole con = Session.Console;
            if (kill)
            {
                if (!noevents) OnEvent("Odłączanie maszyny", 1);
                con.PowerDown();
            }
            else
            {
                if (!noevents) OnEvent("Wyłączanie maszyny", 1);
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
                        Program.VM.Session.Console.Machine.SetGuestPropertyValue("VBOX_USER_NAME", Program.username);
                        Program.VM.Session.Console.Machine.SetGuestPropertyValue("VBOX_USER_IDENTITY", Program.identity);
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
            }
        }
        public void Tick()
        {
            if (!MachineReady.getReadyOffline())
            {
                Status = State.Busy;
                return;
            }
            relock();
            State oldState = Status;
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

            if (Status == State.On && Session.Console.Guest.AdditionsRunLevel == AdditionsRunLevelType.AdditionsRunLevelType_Userland)
            {
                Status = State.Operational;
                if (oldState != State.Operational)
                {
                    Program.VM.OnEvent("Maszyna gotowa do pracy", 1);
                }
            }
        }
        #endregion
        #region Remote process execution
        public string exec(String cmd, String args, bool verbose = false)
        {
            string filename = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";
            args = " -- -c 'source /etc/profile;" + args + "'";
            if (verbose) args = " --verbose " + args;
            args = " guestcontrol \"" + MachineName + "\" execute --image \"" + cmd + "\" --username=\"fotka\" --password=\"@fotka\" --wait-exit --wait-stdout --wait-stderr " + args;

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
            string outputss = p.StandardOutput.ReadToEnd();
            return outputss;
        }
        #endregion
        #region VM Management
        public void Install()
        {
            if (!MachineReady.API || !MachineReady.VersionRemote) return;
            OnEvent("Pobieranie obrazu", 1);
            IAppliance ia = vb.CreateAppliance();
            ia.Read(ImgPath).WaitForCompletion(-1);
            OnEvent("Odczytywanie obrazu", 1);
            ia.Interpret();
            IVirtualSystemDescription[] descs = (IVirtualSystemDescription[])ia.VirtualSystemDescriptions;
            if (descs.Length != 1)
            {
                throw new Exception("Bledny obraz maszyny");
            }

            //System.Array aRefs, aOvfValues, aVBoxValues, aExtraConfigValues, aTypes;
            VirtualSystemDescriptionType[] Types;
            String[] VBoxValues, ExtraConfigValues, Refs, OvfValues;
            IVirtualSystemDescription desc = descs[0];

            desc.GetDescription(out Types, out Refs, out OvfValues, out VBoxValues, out ExtraConfigValues);

            //Types = (VirtualSystemDescriptionType[])aTypes;
            //VBoxValues = (String[])aVBoxValues;
            //ExtraConfigValues = (String[])aExtraConfigValues;
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

            //aVBoxValues = VBoxValues;
            //aExtraConfigValues = ExtraConfigValues;

            descs[0].SetFinalValues(enabled.ToArray(), VBoxValues, ExtraConfigValues);
            ImportOptions[] opts = {ImportOptions.ImportOptions_KeepAllMACs};
            OnEvent("Instalacja obrazu", 1);
            IProgress proprc = ia.ImportMachines(opts);
            uint olprc = proprc.Percent;

            while (proprc.Percent<100)
            {
                if (olprc != proprc.Percent)
                {
                    OnEvent("Instalacja obrazu (" + proprc.Percent + "%)", 1);
                    olprc = proprc.Percent;
                }
                Application.DoEvents();
            }
            proprc.WaitForCompletion(-1);
            vb.FindMachine(((string[])ia.Machines)[0]).SetExtraData("BM/Version", imgversion.ToString());
        }
        public bool Uninstall(string name)
        {
            unlock();
            OnEvent("Usuwanie obrazu", 1);
            IMachine mach;
            try
            {
                mach = vb.FindMachine(name);
            }
            catch (Exception)
            {
                OnEvent("Obraz nie jest zainstalowany", 1);
                return false;
            }
            String t = mach.SettingsFilePath;
            if (MachineName == name) deInit();
            bool unregistered = false;
            IMedium[] med;
            for (int tmp = 0; tmp <= 16; tmp++)
            {
                try
                {
                    med = (IMedium[])mach.Unregister(CleanupMode.CleanupMode_Full);
                    mach.Delete(med).WaitForCompletion(-1);
                    foreach (IMedium m in med)
                    {
                        MediumState s = m.LockWrite();
                        if (s == MediumState.MediumState_LockedWrite)
                        {
                            m.Reset().WaitForCompletion(-1);
                        }
                        m.UnlockWrite();
                        m.DeleteStorage().WaitForCompletion(-1);
                    }
                    unregistered = true;
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }
            if (!unregistered)
            {
                OnEvent("Nie udało się usunąć obrazu :(", 3);
                return false;
            }
            OnEvent("Usuwanie plików", 1);
            DirectoryInfo di = Directory.GetParent(t);
            di.Delete(true);
            OnEvent("Obraz usunięty", 1);
            return true;
        }
        public void Rename(string _old, string _new)
        {
            OnEvent("Finalizowanie instalacji", 1);
            if(Session != null && Session.State == SessionState.SessionState_Locked)
                Session.UnlockMachine();
            Session tmps = new VirtualBox.Session();
            try
            {
                IMachine mach = vb.FindMachine(_old);
                Thread.Sleep(1000);
                mach.LockMachine(tmps, LockType.LockType_Write);
                tmps.Machine.SaveSettings();
                tmps.UnlockMachine();

                mach.LockMachine(tmps, LockType.LockType_Write);
                tmps.Machine.Name = _new;

                tmps.Machine.SaveSettings();
                tmps.UnlockMachine();
            }
            catch (Exception)
            {
                //TODO: Remove Devel directory
                return;
            }
            finally
            {
                if(tmps.State==SessionState.SessionState_Locked)
                    tmps.UnlockMachine();
            }
            OnEvent("Zakończono instalację", 1);
        }
        public void MachineConfig()
        {
            if (Properties.Settings.Default.vm_settings_bypass) return;
            if (!MachineReady.getReadyOffline() || Status != State.Off) return;
            relock();
            try
            {
                if (Properties.Settings.Default.vm_settings_mem > 0) Session.Machine.MemorySize = Properties.Settings.Default.vm_settings_mem;
                if (Properties.Settings.Default.vm_settings_vram > 0) Session.Machine.VRAMSize = Properties.Settings.Default.vm_settings_vram;

                if (Properties.Settings.Default.vm_settings_setbootorder)
                {
                    Session.Machine.SetBootOrder(1, DeviceType.DeviceType_HardDisk);
                    Session.Machine.SetBootOrder(2, DeviceType.DeviceType_Null);
                    Session.Machine.SetBootOrder(3, DeviceType.DeviceType_Null);
                    Session.Machine.SetBootOrder(4, DeviceType.DeviceType_Null);
                }
                if (Properties.Settings.Default.vm_settings_setcpu)
                {
                    if (vb.Host.ProcessorOnlineCount > 1 && Properties.Settings.Default.vm_multicore) //performance degradation with more than one core
                    {
                        Session.Machine.CPUCount = 2;
                        if (Properties.Settings.Default.vm_settings_cpuautocap)
                        {
                            if (vb.Host.ProcessorOnlineCount > 2)
                            {
                                Session.Machine.CPUExecutionCap = 100;
                            }
                            else
                            {
                                Session.Machine.CPUExecutionCap = Properties.Settings.Default.vm_settings_cpucap;
                            }
                        }
                    }
                    else
                    {
                        Session.Machine.CPUCount = 1;
                        if (Properties.Settings.Default.vm_settings_cpuautocap)
                        {
                            Session.Machine.CPUExecutionCap = 100;
                        }
                    }
                    if (!Properties.Settings.Default.vm_settings_cpuautocap && Properties.Settings.Default.vm_settings_cpucap > 0)
                    {
                        Session.Machine.CPUExecutionCap = Properties.Settings.Default.vm_settings_cpucap;
                    }
                }
                ISerialPort serialp = Session.Machine.GetSerialPort(0);
                if (Properties.Settings.Default.vm_settings_serialport)
                {
                    TTY = new SerialPipe();
                    TTY.addChallange(serial_prompt1, serial_response1);
                    serialp.Path = @"\\.\pipe\" + TTY.Start();
                    serialp.HostMode = PortMode.PortMode_HostPipe;
                    serialp.Enabled = 1;
                }
                else
                {
                    DialogResult odp = DialogResult.No;
                    try
                    {
                        if (serialp.Enabled == 1 && serialp.HostMode == PortMode.PortMode_HostPipe)
                        {
                            odp = MessageBox.Show("Maszyna " + MachineName + " ma skonfigurowany port szeregowy do wspolpracy z BM, ale jego obsługa w BM jest wyłączona.\nCzy chcesz aby BM wyłączył port szeregowy?", "Konfiguracja VM", MessageBoxButtons.YesNo);
                            if (odp == DialogResult.Yes)
                            {
                                serialp.Enabled = 0;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Session.Machine.DiscardSettings();
                        OnEvent("Wystąpił problem podczas sprawdzania ustawień maszyny", 1);
                    }
                    if (odp == DialogResult.Yes)
                    {
                        serialp.Enabled = 0;
                    }
                }
                if (Properties.Settings.Default.vm_settings_setnetmac)
                {
                    INetworkAdapter n0 = Session.Machine.GetNetworkAdapter(0);
                    if (n0.AttachmentType == NetworkAttachmentType.NetworkAttachmentType_Bridged)
                    { // change settings only when bridged interface
                        if (n0.MACAddress == Properties.Settings.Default.vm_settings_defaultmac)
                        { // if mac has been set before, do nothing, else:
                            bool foundInterface = false;
                            foreach (IHostNetworkInterface hni in vb.Host.NetworkInterfaces) // iterate host interfaces to find and set bridged interface
                            {
                                if (hni.IPAddress.StartsWith(Properties.Settings.Default.vm_settings_networkprefix))
                                {
                                    n0.BridgedInterface = hni.Name;
                                    foundInterface = true;
                                    break;
                                }
                            }
                            if (!foundInterface) OnEvent("Wystąpił problem podczas ustawiania sieci (nie znaleziono karty lokalnej)", 2);

                            string usermac = Program.getMACAddress(); // get mac from remote text file
                            if (usermac.Length == 12)
                            { // if mac found for current user
                                Program.NetworkLog(String.Format("Assigning network MAC '{1}' for {0}", Program.username, usermac), "", 0);
                                n0.MACAddress = usermac;
                            }
                            else
                            {
                                Program.NetworkLog(String.Format("Generate random network MAC for {0}", Program.username, usermac), "", 1);
                                n0.MACAddress = ""; // VB will generate random mac
                            }
                        }
                    }
                    else
                    {
                        OnEvent("Wystąpił problem podczas ustawiania mac (zły typ karty)", 2);
                    }
                }
                Session.Machine.SaveSettings();
#if DEBUG
                OnEvent("Config ok", 0);
#endif
            }
            catch (Exception)
            {
                Session.Machine.DiscardSettings();
                OnEvent("Wystąpił problem podczas ustawiania maszyny", 2);
            }
            unlock();
        }
        #endregion
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Devel_VM.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string User {
            get {
                return ((string)(this["User"]));
            }
            set {
                this["User"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.11.11.200\\instale\\Devel_beta\\current_version.txt")]
        public string path_imgver {
            get {
                return ((string)(this["path_imgver"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.11.11.200\\instale\\Devel\\{vm_name}_test.ova")]
        public string path_image {
            get {
                return ((string)(this["path_image"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://00.info.pl/devel/ov.xml")]
        public string update_uri {
            get {
                return ((string)(this["update_uri"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Devel")]
        public string vm_name {
            get {
                return ((string)(this["vm_name"]));
            }
            set {
                this["vm_name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool vm_multicore {
            get {
                return ((bool)(this["vm_multicore"]));
            }
            set {
                this["vm_multicore"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.11.11.200\\instale\\Installer_Beta_Manager.exe")]
        public string path_updater {
            get {
                return ((string)(this["path_updater"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/S")]
        public string path_updater_args {
            get {
                return ((string)(this["path_updater_args"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BMTTY{0}")]
        public string serial_pipe {
            get {
                return ((string)(this["serial_pipe"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\DEVEL")]
        public string web_dir {
            get {
                return ((string)(this["web_dir"]));
            }
            set {
                this["web_dir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\DEVEL\\node\\apps\\")]
        public string node_dir {
            get {
                return ((string)(this["node_dir"]));
            }
            set {
                this["node_dir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/export/www/DEVEL/node/apps/")]
        public string node_devel_dir {
            get {
                return ((string)(this["node_devel_dir"]));
            }
            set {
                this["node_devel_dir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\DEVEL\\modules\\daemons")]
        public string daemons_path_absolute {
            get {
                return ((string)(this["daemons_path_absolute"]));
            }
            set {
                this["daemons_path_absolute"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("application\\core\\cron")]
        public string daemons_path_relative {
            get {
                return ((string)(this["daemons_path_relative"]));
            }
            set {
                this["daemons_path_relative"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".conf")]
        public string daemons_file_ext {
            get {
                return ((string)(this["daemons_file_ext"]));
            }
            set {
                this["daemons_file_ext"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/export/www/DEVEL/")]
        public string daemons_devel_dir {
            get {
                return ((string)(this["daemons_devel_dir"]));
            }
            set {
                this["daemons_devel_dir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool vm_settings_bypass {
            get {
                return ((bool)(this["vm_settings_bypass"]));
            }
            set {
                this["vm_settings_bypass"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("768")]
        public uint vm_settings_mem {
            get {
                return ((uint)(this["vm_settings_mem"]));
            }
            set {
                this["vm_settings_mem"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("24")]
        public uint vm_settings_vram {
            get {
                return ((uint)(this["vm_settings_vram"]));
            }
            set {
                this["vm_settings_vram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool vm_settings_setbootorder {
            get {
                return ((bool)(this["vm_settings_setbootorder"]));
            }
            set {
                this["vm_settings_setbootorder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool vm_settings_setcpu {
            get {
                return ((bool)(this["vm_settings_setcpu"]));
            }
            set {
                this["vm_settings_setcpu"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool vm_settings_cpuautocap {
            get {
                return ((bool)(this["vm_settings_cpuautocap"]));
            }
            set {
                this["vm_settings_cpuautocap"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("80")]
        public uint vm_settings_cpucap {
            get {
                return ((uint)(this["vm_settings_cpucap"]));
            }
            set {
                this["vm_settings_cpucap"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool vm_settings_serialport {
            get {
                return ((bool)(this["vm_settings_serialport"]));
            }
            set {
                this["vm_settings_serialport"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool vm_settings_setnetmac {
            get {
                return ((bool)(this["vm_settings_setnetmac"]));
            }
            set {
                this["vm_settings_setnetmac"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.11.11.200\\instale\\Devel\\name2mac.txt")]
        public string path_macaddr {
            get {
                return ((string)(this["path_macaddr"]));
            }
            set {
                this["path_macaddr"] = value;
            }
        }
    }
}

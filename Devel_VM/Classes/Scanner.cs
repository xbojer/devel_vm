using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Devel_VM.Classes
{
    class Scanner
    {
        public static Dictionary<string, Dictionary<string, string>> getSiteUrls()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string,Dictionary<string,string>>();

            string rootDir = Properties.Settings.Default.web_dir;
            if (!Directory.Exists(rootDir))
            {
                return result;
            }
            string[] domainLevel = Directory.GetDirectories(rootDir);
            foreach (string domainEntry in domainLevel)
            {
                string domainName = Path.GetFileName(domainEntry);
                if(!domainName.Contains('.')) continue;

                result[domainName] = new Dictionary<string, string>();
                result[domainName]["@"] = "http://"+Program.username.Replace('.','-')+"."+domainName+"/";
                result[domainName]["X"] = "http://" + domainName.Replace("devel","x")+"/";
                result[domainName]["WWW"] = "http://" + domainName.Replace("devel", "x") + "/";
            }

            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> getNodeApps()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

            string rootDir = Properties.Settings.Default.node_dir;
            string develDir = Properties.Settings.Default.node_devel_dir;

            if (!Directory.Exists(rootDir))
            {
                return result;
            }

            string[] domainLevel = Directory.GetDirectories(rootDir);
            foreach (string domainEntry in domainLevel)
            {
                string appdir = Path.GetFileName(domainEntry);

                result[appdir] = new Dictionary<string, string>();
                result[appdir]["Start"] = "export NODE_ENV=beta; /usr/bin/screen -dmS nodeBM_" + appdir + " /usr/bin/node " + develDir + appdir + "/index.js";
                result[appdir]["Stop"] = "export NODE_ENV=beta; /usr/bin/screen -S nodeBM_" + appdir + " -X quit";
            }

            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> getDaemonsInstances()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

            string cmd_begin = "export BETA=1; /usr/bin/screen ";

            string develDir = Properties.Settings.Default.daemons_devel_dir;
            string rootDir = Properties.Settings.Default.web_dir;
            
            string absolutePath = Properties.Settings.Default.daemons_path_absolute;
            string relativePath = Properties.Settings.Default.daemons_path_relative;
            
            if (Directory.Exists(absolutePath))
            {
                string[] files = Directory.GetFiles(absolutePath, "*" + Properties.Settings.Default.daemons_file_ext);

                foreach (string configPath in files)
                {
                    string relativeToFile = configPath.Replace(rootDir, "").Replace(Path.GetFileName(configPath), "").Replace(@"\", "/").Trim("/".ToCharArray());
                    string[] parts = Path.GetFileNameWithoutExtension(configPath).Split("-".ToCharArray());
                    if(parts.Length != 3) continue;
                    
                    string develPath = develDir + relativeToFile + "/";
                    string service = parts[0];
                    string option = parts[2];
                    string service_name = "daemonBM_" + service + "_" + option;

                    if (!result.ContainsKey(service)) result[service] = new Dictionary<string, string>();
                    result[service][option + " - Start"] = cmd_begin + "-dmS " + service_name + " /usr/bin/python " + develPath + "daemon.py " + develPath + Path.GetFileName(configPath);
                    result[service][option + " - Stop"] = cmd_begin + "-S " + service_name + " -X quit";
                }
            }

            string[] domainLevel = Directory.GetDirectories(rootDir);
            foreach (string domainPath in domainLevel)
            {
                string domainName = Path.GetFileName(domainPath);
                if (!domainName.Contains('.')) continue;

                if (Directory.Exists(domainPath+@"\"+relativePath))
                {
                    string[] files = Directory.GetFiles(domainPath + @"\" + relativePath, "*" + Properties.Settings.Default.daemons_file_ext);

                    foreach (string configPath in files)
                    {
                        string relativeToFile = configPath.Replace(rootDir, "").Replace(Path.GetFileName(configPath), "").Replace(@"\", "/").Trim("/".ToCharArray());
                        string[] parts = Path.GetFileNameWithoutExtension(configPath).Split("-".ToCharArray());
                        if (parts.Length != 2) continue;

                        string develPath = develDir + relativeToFile + "/";
                        string option = parts[1];
                        string service_name = "daemonBM_" + domainName + "_" + option;

                        if (!result.ContainsKey(domainName)) result[domainName] = new Dictionary<string, string>();
                        result[domainName][option + " - Start"] = cmd_begin + "-dmS " + service_name + " /usr/bin/python " + develPath + "daemon.py " + develPath + Path.GetFileName(configPath);
                        result[domainName][option + " - Stop"] = cmd_begin + "-S " + service_name + " -X quit";
                    }
                }
                
            }

            return result;
        }
    }
}

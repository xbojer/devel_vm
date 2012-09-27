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
                //result[domainName]["@"] = "http://" + Program.username.Replace('.', '-') + "." + domainName + "/";
                result[appdir]["Start"] = "export NODE_ENV=beta; /usr/bin/screen -dmS nodeBM_" + appdir + " /usr/bin/node " + develDir + appdir + "/index.js";
                result[appdir]["Stop"] = "export NODE_ENV=beta; /usr/bin/screen -S nodeBM_" + appdir + " -X quit";
            }

            return result;
        }
    }
}

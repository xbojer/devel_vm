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
    }
}

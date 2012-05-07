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
            Dictionary<string, Dictionary<string, string>> branches = new Dictionary<string, Dictionary<string, string>>();

            string rootDir = Properties.Settings.Default.web_dir;
            string trunkDir = Properties.Settings.Default.web_dir_trunk;
            string branchesDir = Properties.Settings.Default.web_dir_branches;

            string[] domainLevel = Directory.GetDirectories(rootDir);
            foreach (string domainEntry in domainLevel)
            {
                string domainName = Path.GetFileName(domainEntry);
                if (domainName == "modules") continue;
                string[] subLevel = Directory.GetDirectories(domainEntry);
                foreach (string subEntry in subLevel)
                {
                    string subDomainName = Path.GetFileName(subEntry) + "." + domainName;
                    string[] itemLevel = Directory.GetDirectories(subEntry);
                    foreach (string itemEntry in itemLevel)
                    {
                        string name = Path.GetFileName(itemEntry);
                        if (name == trunkDir)
                        {
                            string uri = "http://" + subDomainName;
                            if (!branches.ContainsKey(subDomainName))
                            {
                                branches[subDomainName] = new Dictionary<string, string>();
                            }
                            branches[subDomainName].Add(name, uri);
                        }
                        else if (name == branchesDir)
                        {
                            string[] bLevel = Directory.GetDirectories(itemEntry);
                            foreach (string bEntry in bLevel)
                            {
                                string bName = Path.GetFileName(bEntry);
                                if (bName != ".svn")
                                {
                                    string uri = "http://" + bName + "." + subDomainName;
                                    if (!branches.ContainsKey(subDomainName))
                                    {
                                        branches[subDomainName] = new Dictionary<string, string>();
                                    }
                                    branches[subDomainName].Add(bName, uri);
                                }
                            }
                        }
                    }
                }
            }

            return branches;
        }
    }
}

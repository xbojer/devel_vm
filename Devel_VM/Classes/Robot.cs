using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Devel_VM
{
    class Robot
    {
        public const string user_unknown = "unknown";
        const string uname_reg = "(<span class=\"gbgt gbts gbtsa\">)([A-z.]+)@spolecznosci.pl";
        public static string getUsernameByLink(string url)
        {
            HTTPGet http = new HTTPGet();
            try
            {
                http.Request(url);
            }
            catch (Exception)
            {
                return user_unknown;
            }

            if (http.StatusCode != 200) return user_unknown;
            Match m = new Regex(uname_reg).Match(http.ResponseBody);
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else return user_unknown;
        }
    }
}

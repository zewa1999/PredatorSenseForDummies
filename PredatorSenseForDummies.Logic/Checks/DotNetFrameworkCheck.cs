using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    public static class DotNetFrameworkCheck
    {
        private static string name = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";

        [SupportedOSPlatform("windows")]
        public static bool IsDotNetRequireMatch(string strDotNetVer)
        {
            bool flag = false;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
            if (registryKey != null)
            {
                string[] subKeyNames = registryKey.GetSubKeyNames();
                string lower = ("v" + strDotNetVer).ToLower();
                foreach (string str in subKeyNames)
                {
                    if (str.ToLower().Contains(lower))
                    {
                        flag = true;
                        break;
                    }
                }
                registryKey.Close();
            }
            return flag;
        }
    }
}

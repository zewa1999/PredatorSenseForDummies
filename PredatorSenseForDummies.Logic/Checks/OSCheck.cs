using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    public static class OSCheck
    {
        public static void GetCurrentOSVersion(ref int versionMajor, ref int versionMinor)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe");
            versionMajor = versionInfo.ProductMajorPart;
            versionMinor = versionInfo.ProductMinorPart;
        }

        public static bool OSMinimumversionCheck(string RequiredOSversion)
        {
            char[] chArray = new char[1] { '.' };
            string[] strArray = RequiredOSversion.Split(chArray);
            int int32_1 = Convert.ToInt32(strArray[0]);
            int int32_2 = Convert.ToInt32(strArray[1]);
            bool flag = false;
            int versionMajor = 0;
            int versionMinor = 0;
            GetCurrentOSVersion(ref versionMajor, ref versionMinor);
            if (int32_1 == versionMajor && int32_2 <= versionMinor || int32_1 < versionMajor)
                flag = true;
            return flag;
        }
    }
}

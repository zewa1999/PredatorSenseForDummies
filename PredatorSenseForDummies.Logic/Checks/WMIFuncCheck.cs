using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    // this class needs to be changed according to all the models that are working with predator sense from the Helios 300 series
    public static class WMIFuncCheck
    {
        public static string DoSupportRGBKeyboard()
        {
            bool flag = false;
            try
            {
                ManagementScope scope = new ManagementScope("\\root\\wmi", new ConnectionOptions()
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                });
                scope.Connect();
                WqlObjectQuery wqlObjectQuery = new WqlObjectQuery("select * from MSSMBios_RawSMBiosTables");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, (ObjectQuery)wqlObjectQuery);
                int length = 0;
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    if (managementObject != null)
                    {
                        length = int.Parse(managementObject.Properties["Size"].Value.ToString());
                        break;
                    }
                }
                if (length > 0)
                {
                    byte[] numArray1 = new byte[length];
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        if (managementObject != null)
                        {
                            numArray1 = managementObject.Properties["SMBiosData"].Value as byte[];
                            break;
                        }
                    }
                    int num1 = 0;
                    while (num1 < length)
                    {
                        int num2 = (int)numArray1[0];
                        byte num3 = numArray1[1];
                        byte[] numArray2 = new byte[length + 1];
                        Array.Copy((Array)numArray1, (int)num3, (Array)numArray2, 0, length - (int)num3);
                        int index1 = 0;
                        while (numArray2[index1] != (byte)0 || numArray2[index1 + 1] != (byte)0)
                            ++index1;
                        num1 = num1 + (int)num3 + index1 + 2;
                        Array.Copy((Array)numArray1, (int)num3 + index1 + 2, (Array)numArray1, 0, length - ((int)num3 + index1 + 2));
                        if (numArray1[0] == (byte)171)
                        {
                            for (int index2 = 4; index2 < (int)numArray1[1]; index2 += 5)
                            {
                                int num4 = (int)numArray1[index2];
                                int num5 = (int)numArray1[index2 + 1];
                                int num6 = (int)numArray1[index2 + 2];
                                int num7 = (int)numArray1[index2 + 3];
                                int num8 = (int)numArray1[index2 + 4];
                                if (num4 == 19)
                                    flag = true;
                            }
                        }
                    }
                }
                managementObjectSearcher.Dispose();
                return "" + (flag ? " RGBKB=1" : " RGBKB=0");
            }
            catch
            {
                return "";
            }
        }

        public static string GetModelName()
        {
            string str = "";
            try
            {
                ManagementScope scope = new ManagementScope("\\root\\cimv2", new ConnectionOptions()
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                });
                scope.Connect();
                WqlObjectQuery wqlObjectQuery = new WqlObjectQuery("select * from Win32_ComputerSystem");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, (ObjectQuery)wqlObjectQuery);
                if (managementObjectSearcher != null)
                {
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        if (managementObject["Model"] != null)
                            str = managementObject["Model"].ToString().ToLower(CultureInfo.InvariantCulture);
                    }
                }
                managementObjectSearcher.Dispose();
                if (str == "predator ph317-53")
                    str = "predator ph315-53";
                return str;
            }
            catch
            {
                return "";
            }
        }

        public static string GetCPUName()
        {
            string str = "";
            try
            {
                ManagementScope scope = new ManagementScope("\\root\\cimv2", new ConnectionOptions()
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                });
                scope.Connect();
                WqlObjectQuery wqlObjectQuery = new WqlObjectQuery("select * from Win32_Processor");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, (ObjectQuery)wqlObjectQuery);
                if (managementObjectSearcher != null)
                {
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        if (managementObject["Name"] != null)
                            str = managementObject["Name"].ToString().ToLower(CultureInfo.InvariantCulture);
                    }
                }
                managementObjectSearcher.Dispose();
                return str;
            }
            catch
            {
                return "";
            }
        }
    }
}

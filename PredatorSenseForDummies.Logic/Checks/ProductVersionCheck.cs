using Microsoft.Win32;
using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    public static class ProductVersionCheck
    {
        public static ProductExistFlag CheckProductExist(string softwareid, string currentVersion)
        {
            ProductExistFlag productExistFlag = ProductExistFlag.Enum_NotExist;
            string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            char[] chArray = new char[1] { '.' };
            string[] strArray = currentVersion.Split(chArray);
            int int32_1 = Convert.ToInt32(strArray[0]);
            int int32_2 = Convert.ToInt32(strArray[1]);
            int int32_3 = Convert.ToInt32(strArray[2]);
            RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(name);
            if (registryKey1 != null)
            {
                bool flag = false;
                softwareid = softwareid.ToLower();
                foreach (string subKeyName in registryKey1.GetSubKeyNames())
                {
                    if (subKeyName.ToLower().Contains(softwareid))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    productExistFlag = ProductExistFlag.Enum_NotExist;
                }
                else
                {
                    string str = softwareid;
                    if (!softwareid.Contains("{"))
                        str = "{" + softwareid + "}";
                    RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name + "\\" + str);
                    if (registryKey2 != null)
                    {
                        int int32_4 = Convert.ToInt32(registryKey2.GetValue("Version", (object)0));
                        if (int32_4 == 0)
                        {
                            productExistFlag = ProductExistFlag.Enum_NotExist;
                        }
                        else
                        {
                            int num = int32_1 * Convert.ToInt32(Math.Pow(16.0, 6.0)) + int32_2 * Convert.ToInt32(Math.Pow(16.0, 4.0)) + int32_3;
                            productExistFlag = int32_4 != num ? (int32_4 >= num ? ProductExistFlag.Enum_NewerVersionExist : ProductExistFlag.Enum_OlderVersionExist) : ProductExistFlag.Enum_SameVersionExist;
                        }
                        registryKey2.Close();
                    }
                }
                registryKey1.Close();
            }
            return productExistFlag;
        }
        public static ProductExistFlag CheckProductExist(
        string softwareid,
        string currentVersion,
        out string installedVersion)
        {
            ProductExistFlag productExistFlag = ProductExistFlag.Enum_NotExist;
            string str1 = string.Empty;
            string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            char[] chArray = new char[1] { '.' };
            string[] strArray = currentVersion.Split(chArray);
            int int32_1 = Convert.ToInt32(strArray[0]);
            int int32_2 = Convert.ToInt32(strArray[1]);
            int int32_3 = Convert.ToInt32(strArray[2]);
            RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(name);
            if (registryKey1 != null)
            {
                bool flag = false;
                softwareid = softwareid.ToLower();
                foreach (string subKeyName in registryKey1.GetSubKeyNames())
                {
                    if (subKeyName.ToLower().Contains(softwareid))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    productExistFlag = ProductExistFlag.Enum_NotExist;
                }
                else
                {
                    string str2 = softwareid;
                    if (!softwareid.Contains("{"))
                        str2 = "{" + softwareid + "}";
                    RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name + "\\" + str2);
                    if (registryKey2 != null)
                    {
                        int int32_4 = Convert.ToInt32(registryKey2.GetValue("Version", (object)0));
                        if (int32_4 == 0)
                        {
                            productExistFlag = ProductExistFlag.Enum_NotExist;
                        }
                        else
                        {
                            int num = int32_1 * Convert.ToInt32(Math.Pow(16.0, 6.0)) + int32_2 * Convert.ToInt32(Math.Pow(16.0, 4.0)) + int32_3;
                            productExistFlag = int32_4 != num ? (int32_4 >= num ? ProductExistFlag.Enum_NewerVersionExist : ProductExistFlag.Enum_OlderVersionExist) : ProductExistFlag.Enum_SameVersionExist;
                            str1 = (string)registryKey2.GetValue("DisplayVersion") ?? string.Empty;
                        }
                        registryKey2.Close();
                    }
                }
                registryKey1.Close();
            }
            installedVersion = str1;
            return productExistFlag;
        }
        public static bool HotkeyUpdateNoticeCheck(string softwareid, string currentVersion)
        {
            bool flag1 = false;
            string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            char[] chArray = new char[1] { '.' };
            string[] strArray = currentVersion.Split(chArray);
            int int32_1 = Convert.ToInt32(strArray[0]);
            int int32_2 = Convert.ToInt32(strArray[1]);
            int int32_3 = Convert.ToInt32(strArray[2]);
            RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(name);
            if (registryKey1 != null)
            {
                bool flag2 = false;
                softwareid = softwareid.ToLower();
                foreach (string subKeyName in registryKey1.GetSubKeyNames())
                {
                    if (subKeyName.ToLower().Contains(softwareid))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    flag1 = false;
                }
                else
                {
                    string str = softwareid;
                    if (!softwareid.Contains("{"))
                        str = "{" + softwareid + "}";
                    RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name + "\\" + str);
                    if (registryKey2 != null)
                    {
                        int int32_4 = Convert.ToInt32(registryKey2.GetValue("Version", (object)0));
                        if (int32_4 == 0)
                        {
                            flag1 = false;
                        }
                        else
                        {
                            int num = int32_1 * Convert.ToInt32(Math.Pow(16.0, 6.0)) + int32_2 * Convert.ToInt32(Math.Pow(16.0, 4.0)) + int32_3;
                            if (int32_4 < num)
                                flag1 = true;
                        }
                        registryKey2.Close();
                    }
                }
                registryKey1.Close();
            }
            return flag1;
        }
    }
}

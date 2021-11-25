using Microsoft.Win32;
using PredatorSenseForDummies.Logic;
using PredatorSenseForDummies.Logic.Checks;
using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PredatorSenseForDummies.UI
{
    public class MainWindowViewModel
    {
        public const int WS_VISIBLE = 268435456;
        private static readonly string gName = "PredatorSense";
        private static readonly string gStrPackageX64_intel = "PredatorSense_I.msi";
        private static readonly string gStrPackageX64_amd = "PredatorSense_A.msi";
        private static string gProductID = "8D399C7A-8693-4BDE-9D22-D43CBB8BBF62";
        private static readonly string gMutexName = "PredatorSense-Installer-8D399C7A-8693-4BDE-9D22-D43CBB8BBF62";
        private static Brand gBrand = Brand.Enum_Acer;
        private static int gMachineType = 0;
        private static string gProductName = "PredatorSense Service";
        private static string gCurrentVersion = "3.00.3152";
        private static string gOSversion = "6.3";
        private static string gDOTNETversion = "4.0";
        private static int Machine_PTB = 0;
        private static bool gIsRunSilent = false;
        private static bool gIsBgOnly = false;
        private static bool gIsLiveUpdate = false;
        private static readonly string gAppxFolderName = "PredatorSenseUWP";
        private static readonly string gAppxFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + gAppxFolderName;
        private static readonly Regex rxVersionDateTag = new Regex("^" + gName + "_v[0-9.]+_[0-9]+\\.tag$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
          string section,
          string key,
          string def,
          StringBuilder retVal,
          int size,
          string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpDefault,
          StringBuilder lpReturnedString,
          uint nSize,
          string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
          string section,
          string key,
          string val,
          string filePath);

        public MainWindowViewModel()
        {

        }

        public void StartProcess()
        {
            BasicCommandData cmdData = new BasicCommandData();
            gBrand = BrandCheck.CheckBrand();
            if (gBrand == Brand.Enum_NonAcer || gBrand == Brand.Enum_eMachine || gBrand == Brand.Enum_Founder)
            {
                ErrorMessageHandler.GetErrorMessage(Messages.NotSupportedDevice);
                return;
            }
            Mutex mutex = (Mutex)null;
            try
            {
                bool createdNew;
                mutex = new Mutex(true, gMutexName, out createdNew);
                if (!createdNew)
                {
                    ErrorMessageHandler.GetErrorMessage(Messages.AnotherInstanceRun, gProductName);
                    return;
                }
            }
            catch
            {
            }
            if (MachineCheck.CheckMachine() != Machine_PTB)
            {
                ErrorMessageHandler.GetErrorMessage(Messages.NotSupportedOS);
                return;
            }

            ProductExistFlag productExistFlag = ProductAndItsVersionCheck();
            if (productExistFlag == ProductExistFlag.Enum_NewerVersionExist || !SystemPrerequirementCheck() || ProductsCheck() || !ModelNameCheck())
                return;
            if (productExistFlag == ProductExistFlag.Enum_OlderVersionExist && gIsRunSilent && gIsLiveUpdate)
            {
                IntPtr zero = IntPtr.Zero;
                string str = "";
                for (int index = 0; index < 10; ++index)
                {
                    str += " ";
                    if (!FindWindow((string)null, gName + str).Equals((object)IntPtr.Zero))
                        return;
                }
            }
            //problema tatii
            if (ProductVersionCheck.HotkeyUpdateNoticeCheck("{" + gProductID + "}", "3.00.3006"))
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\OEM\\PredatorSense\\KeyAssignment", true);
                if (registryKey != null)
                {
                    registryKey.SetValue("Hotkey_Update_Notice", (object)0, RegistryValueKind.DWord);
                    registryKey.Close();
                }
            }
            string str1 = "";
            if (productExistFlag == ProductExistFlag.Enum_OlderVersionExist)
            {
                try
                {
                    str1 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{" + gProductID + "}").GetValue("DisplayVersion").ToString();
                }
                catch
                {
                }
            }
            try
            {
                string str2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\oem\\" + gName + "\\Feature.ini";
                if (File.Exists(str2))
                {
                    if (IniReadValue("Planet9Support", "Planet9", str2) == "0")
                    {
                        string str3 = directoryName + "\\Plugs\\" + WMIFuncCheck.GetModelName() + "\\Feature.ini";
                        if (File.Exists(str3))
                            WritePrivateProfileString("Planet9Support", "Planet9", "0", str3);
                    }
                }
            }
            catch
            {
            }
            try
            {
                string str4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugs\\" + WMIFuncCheck.GetModelName();
                if (Directory.Exists(str4))
                {
                    string target_file_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\oem\\" + gName;
                    DirectoryCopy(str4, target_file_path, true);
                }
            }
            catch (Exception ex)
            {
            }
            MsiCommandMaker msiCommandMaker = new MsiCommandMaker();
            cmdData.ProductPath = !Intel_CPU_support() ? directoryName + "\\" + gStrPackageX64_amd : directoryName + "\\" + gStrPackageX64_intel;
            cmdData.ProductFlag = productExistFlag;
            cmdData.MachineBrand = gBrand;
            cmdData.ProductOldVersion = "v" + str1;
            cmdData.ProductNewVersion = "v" + gCurrentVersion;
            cmdData.MachineType = gMachineType;
            cmdData.ProductName = gProductName;
            string commandParameters = msiCommandMaker.GetMsiCommandParameters(cmdData);
            try
            {
                string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\oem\\" + gName;
                string appSetting1 = ConfigurationManager.AppSettings["appName"];
                string appSetting2 = ConfigurationManager.AppSettings["appVersion"];
                string appSetting3 = ConfigurationManager.AppSettings["appDate"];
                string path = dirPath + "\\" + appSetting1 + "_" + appSetting2 + "_" + appSetting3 + ".tag";
                RemoveOldVersionTag(dirPath);
                File.Create(path).Dispose();
            }
            catch (Exception ex)
            {
            }
            try
            {
                string str5 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\oem\\" + gName + "\\Feature.ini";
                int num = 0;
                if (File.Exists(str5))
                    num = Convert.ToInt32(IniReadValue("MobileSupport", "Mobile", str5));
                if (num == 1)
                    CommandOutput("netsh advfirewall firewall add rule name=\"PSMobile\" dir=in program=\"C:\\Program Files\\Acer\\PredatorSense Service\\PSMobile.exe\" action=allow ");
            }
            catch
            {
            }
            int num1 = ExecuteMsiInstall(commandParameters);
            try
            {
                mutex.ReleaseMutex();
            }
            catch
            {
            }
            if ((num1 == 0 || num1 == 3010) && productExistFlag != ProductExistFlag.Enum_SameVersionExist)
            {
                if (gIsRunSilent)
                {
                    if (gIsBgOnly)
                        if (num1 == 3010 & gIsRunSilent & gIsLiveUpdate)
                        {
                            num1 = 0;
                            return;
                        }
                   
                }
                try
                {
                    if (Directory.Exists(gAppxFolderPath))
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            string appSetting4 = ConfigurationManager.AppSettings["packageName"];
                            string appSetting5 = ConfigurationManager.AppSettings["packageDirectory"];
                            string appSetting6 = ConfigurationManager.AppSettings["packagePath"];
                            string appSetting7 = ConfigurationManager.AppSettings["licensePath"];
                            int int32 = Convert.ToInt32(ConfigurationManager.AppSettings["dependencyX64Number"]);
                            List<string> stringList = new List<string>();
                            string PSInstallCMD = "Add-AppxPackage -Path " + appSetting6;
                            string DismInstallCMD = "Dism /Online /Add-Provisionedappxpackage /packagepath:" + appSetting6;
                            CombineInstallCMD(ListDependency(int32), appSetting7, ref PSInstallCMD, ref DismInstallCMD);
                            InstallbyPS(PSInstallCMD, appSetting5);
                            CommandOutputUWP(DismInstallCMD, appSetting5);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            
        }
        private static ProductExistFlag ProductAndItsVersionCheck()
        {
            string softwareid = "{" + gProductID + "}";
            ProductExistFlag productExistFlag = ProductVersionCheck.CheckProductExist(softwareid, gCurrentVersion);
            if (productExistFlag == ProductExistFlag.Enum_NewerVersionExist)
                ErrorMessageHandler.GetErrorMessage(Messages.NewVersionExists, gProductName);
            return productExistFlag;
        }

        private static bool ProductsCheck()
        {
            bool flag = false;
            string installedVersion = "";
            string softwareid1 = "{FEA5F263-29F7-4C53-B6EB-69F7B4D61C76}";
            if (ProductVersionCheck.CheckProductExist(softwareid1, gCurrentVersion, out installedVersion) != ProductExistFlag.Enum_NotExist)
            {
                ErrorMessageHandler.GetErrorMessage(Messages.OldPSExist, installedVersion);
                flag = true;
            }
            string softwareid2 = "{CDEDBAFC-AC75-4AB0-9A83-A6CD59BA56D3}";
            if (ProductVersionCheck.CheckProductExist(softwareid2, gCurrentVersion, out installedVersion) != ProductExistFlag.Enum_NotExist)
            {
                ErrorMessageHandler.GetErrorMessage(Messages.OldPSExist,installedVersion);
                flag = true;
            }
            return flag;
        }

        private static bool ModelNameCheck()
        {
            var modelDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "\\Plugs\\" + WMIFuncCheck.GetModelName().ToUpper());
            if (Directory.Exists(modelDirectory))
                return true;
            ErrorMessageHandler.GetErrorMessage(Messages.NotSupportedDevice);
            return false;
        }

        private static bool SystemPrerequirementCheck()
        {
            bool flag = true;
            if (!OSCheck.OSMinimumversionCheck(gOSversion))
            {
                ErrorMessageHandler.GetErrorMessage(Messages.NotSupportedOS);
                flag = false;
            }
            else if (!DotNetFrameworkCheck.IsDotNetRequireMatch(gDOTNETversion))
            {
                ErrorMessageHandler.GetErrorMessage(Messages.DotNetFrameworkRequired);
                flag = false;
            }
            return flag;
        }

        private static int ExecuteMsiInstall(string strArgument)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("msiexec.exe");
            processStartInfo.Arguments = strArgument;
            processStartInfo.Verb = "runas";
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

        private static void DirectoryCopy(
          string source_file_path,
          string target_file_path,
          bool copy_sub_dirs)
        {
            try
            {
                DirectoryInfo directoryInfo1 = new DirectoryInfo(source_file_path);
                if (!directoryInfo1.Exists)
                    return;
                DirectoryInfo[] directories = directoryInfo1.GetDirectories();
                if (!Directory.Exists(target_file_path))
                    Directory.CreateDirectory(target_file_path);
                foreach (FileInfo file in directoryInfo1.GetFiles())
                {
                    string str = Path.Combine(target_file_path, file.Name);
                    FileInfo fileInfo = new FileInfo(str);
                    if (fileInfo.Exists)
                        fileInfo.IsReadOnly = false;
                    file.CopyTo(str, true);
                }
                if (!copy_sub_dirs)
                    return;
                foreach (DirectoryInfo directoryInfo2 in directories)
                {
                    string target_file_path1 = Path.Combine(target_file_path, directoryInfo2.Name);
                    DirectoryCopy(directoryInfo2.FullName, target_file_path1, copy_sub_dirs);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static bool Intel_CPU_support() => WMIFuncCheck.GetCPUName().IndexOf("Intel".ToLower(CultureInfo.InvariantCulture)) >= 0;

        private static string CommandOutput(params string[] list)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            string str;
            try
            {
                process.Start();
                for (int index = 0; index < list.Length; ++index)
                    process.StandardInput.WriteLine(list[index]);
                process.StandardInput.WriteLine("exit");
                str = process.StandardOutput.ReadToEnd();
                Console.WriteLine(str);
                process.WaitForExit();
                process.Close();
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }

        public string IniReadValue(string Section, string Key, string inipath)
        {
            StringBuilder retVal = new StringBuilder((int)byte.MaxValue);
            GetPrivateProfileString(Section, Key, "", retVal, (int)byte.MaxValue, inipath);
            return retVal.ToString();
        }

        private List<string> ListDependency(int depLengthX64)
        {
            int num = depLengthX64;
            string str = "dependencyX64Packagepath";
            List<string> stringList = new List<string>();
            for (int index = 1; index <= num; ++index)
            {
                bool flag = false;
                string appSetting = ConfigurationManager.AppSettings[str + index.ToString()];
                Console.WriteLine("Checking: " + appSetting);
                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.AddScript("Get-AppxPackage -name '*" + appSetting.Split('_')[0] + "*'");
                    foreach (PSObject psObject in powerShell.Invoke())
                    {
                        if (Path.GetFileNameWithoutExtension(appSetting).Equals(psObject.BaseObject.ToString()))
                        {
                            Console.WriteLine("Found: " + psObject.BaseObject.ToString());
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                    stringList.Add(appSetting);
            }
            return stringList;
        }

        private void CombineInstallCMD(
          List<string> dependencypackagepath,
          string licensepath,
          ref string PSInstallCMD,
          ref string DismInstallCMD)
        {
            for (int index = 0; index < dependencypackagepath.Count<string>(); ++index)
            {
                if (index == 0)
                {
                    ref string local = ref PSInstallCMD;
                    local = local + " -DependencyPath " + dependencypackagepath.ElementAt<string>(index);
                }
                else
                {
                    ref string local = ref PSInstallCMD;
                    local = local + ", " + dependencypackagepath.ElementAt<string>(index);
                }
                ref string local1 = ref DismInstallCMD;
                local1 = local1 + " /dependencypackagepath:" + dependencypackagepath.ElementAt<string>(index);
            }
            ref string local2 = ref DismInstallCMD;
            local2 = local2 + " /licensepath:" + licensepath;
            Console.WriteLine(PSInstallCMD);
            Console.WriteLine(DismInstallCMD);
        }

        private void InstallbyPS(string PSInstallCMD, string packagedirectory)
        {
            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript("cd  \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\"");
                powerShell.AddScript("cd  \"" + packagedirectory + "\"");
                powerShell.AddScript("(Get-Item -Path \".\\\" -Verbose).FullName");
                powerShell.AddScript(PSInstallCMD);
                foreach (PSObject psObject in powerShell.Invoke())
                    Console.WriteLine(psObject.BaseObject.ToString() + "\n");
                PSDataCollection<ErrorRecord> error = powerShell.Streams.Error;
                if (error == null || error.Count <= 0)
                    return;
                foreach (object obj in error)
                    Console.WriteLine("error: {0}", (object)obj.ToString());
            }
        }

        private string CommandOutputUWP(string DismInstallCMD, string packagedirectory)
        {
            string[] strArray = new string[3]
            {
        "cd %~dp0",
        "cd " + packagedirectory,
        DismInstallCMD
            };
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            string str;
            try
            {
                process.Start();
                for (int index = 0; index < strArray.Length; ++index)
                    process.StandardInput.WriteLine(strArray[index]);
                process.StandardInput.WriteLine("exit");
                str = process.StandardOutput.ReadToEnd();
                Console.WriteLine(str);
                process.WaitForExit();
                process.Close();
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }

        public void RemoveOldVersionTag(string dirPath)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dirPath, "*.tag"))
                {
                    string fileName = Path.GetFileName(file);
                    if (rxVersionDateTag.IsMatch(fileName))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            // needs logging
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // needs logging
            }
        }
    }
}

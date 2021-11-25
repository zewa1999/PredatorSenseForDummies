using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic
{
    public class MsiCommandMaker
    {
        private bool bSilentArg;
        private bool bUninstallArg;
        private bool bLogParaArg;
        private string LogFileName = "";

        private void parseInputInConstructor(string[] Installargs)
        {
            string str1 = "-s";
            string str2 = "autoun";
            string str3 = "-log";
            string str4 = ".txt";
            foreach (string installarg in Installargs)
            {
                if (installarg.Equals(str1))
                    this.bSilentArg = true;
                if (installarg.ToLower().Equals(str2))
                    this.bUninstallArg = true;
                if (installarg.ToLower().Equals(str3))
                    this.bLogParaArg = true;
                if (installarg.ToLower().Contains(str4))
                    this.LogFileName = installarg;
            }
        }

        public string GetMsiCommandParameters(BasicCommandData cmdData)
        {
            string str1 = "";
            this.parseInputInConstructor(cmdData.InstallArgs);

            string str2 = (!this.bSilentArg || !this.bUninstallArg ? str1 + "/i" : str1 + "/x") + " \"" + cmdData.ProductPath + "\"";
            if (!this.bSilentArg || !this.bUninstallArg)
            {
                if (cmdData.ProductFlag == ProductExistFlag.Enum_OlderVersionExist)
                    str2 += " REINSTALL=ALL REINSTALLMODE=ves";
                else if (cmdData.ProductFlag == ProductExistFlag.Enum_SameVersionExist && this.bSilentArg)
                    str2 += " REINSTALL=ALL REINSTALLMODE=vemus REPAIR=1";
            }
            string str3 = str2 + " BOOTSTRATOR=1 ISDT=" + (object)cmdData.MachineType + " GPRODUCTNAME=\"" + cmdData.ProductName + "\"";
            string str4;
            switch (cmdData.MachineBrand)
            {
                case Brand.Enum_Gateway:
                    str4 = str3 + " GATEWAY=1 BRANDNAME=\"Gateway\"";
                    break;

                case Brand.Enum_Packard:
                    str4 = str3 + " PACKARD=1 BRANDNAME=\"Packard Bell\"";
                    break;

                default:
                    str4 = str3 + " ACER=1 BRANDNAME=\"Acer\"";
                    break;
            }
            if (cmdData.ProductFlag == ProductExistFlag.Enum_OlderVersionExist && !this.bSilentArg)
                str4 += string.Format(" OLDVERSION={0} NEWVERSION={1}", (object)cmdData.ProductOldVersion, (object)cmdData.ProductNewVersion);
            string str5 = str4 + " REBOOT=ReallySuppress" + new Commands().GetAllExtendCommand();
            if (this.bSilentArg)
                str5 += " /qn";
            if (this.bLogParaArg)
                str5 = str5 + " /l*v " + this.LogFileName;
            return str5;
        }
    }
}

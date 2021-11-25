using PredatorSenseForDummies.Logic.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic
{
    public class Commands
    {
        public string GetAllExtendCommand()
        {
            string empty = string.Empty;
            return this.GetWMISupportCommand() + this.GetIntelCPUSupportCommand();
        }

        public string GetWMISupportCommand() => WMIFuncCheck.DoSupportRGBKeyboard();

        public string GetWMIModelName() => WMIFuncCheck.GetModelName();

        public string GetIntelCPUSupportCommand()
        {
            bool flag = false;
            if (WMIFuncCheck.GetCPUName().IndexOf("Intel".ToLower(CultureInfo.InvariantCulture)) >= 0)
                flag = true;
            return "" + (flag ? " ICPU=1" : " ICPU=0");
        }
    }
}

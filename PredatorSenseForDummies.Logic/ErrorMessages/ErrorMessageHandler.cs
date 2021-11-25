using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic
{
    public class ErrorMessageHandler
    {
        public static string GetErrorMessage(Messages msg, string parameter = "")
        {
            switch (msg)
            {
                case Messages.NotSupportedDevice:
                    return "Sorry! This computer is not supported.";

                case Messages.NotSupportedOS:
                    return "Sorry! This operating system is not supported.";

                case Messages.DotNetFrameworkRequired:
                    return "Please install Microsoft .NET framework 4.0 first.";

                case Messages.NewVersionExists:
                    return "You already have a newer version of " + parameter + " installed. If you want to install this version, please uninstall the newer version first.";

                case Messages.OldLMExist:
                    return "Launch Manager " + parameter + " has been installed on this computer. Please remove it first.";

                case Messages.AnotherInstanceRun:
                    return "Another installation of " + parameter + " is already processing.";

                case Messages.RebootRequired:
                    return "You must restart your system for the configuration changes made to " + parameter + " to take effect. Click Yes to restart now or No if you plan to manually restart later.";

                case Messages.SwitchLockIsSupport:
                    return "Please turn off Acer SwitchLock™ before uninstalling Quick Access. If you do not turn it off, you will not be able to access the dock’s hard drive in the future.";

                case Messages.OldPSExist:
                    return "PredatorSense " + parameter + " has been installed on this computer. Please remove it first.";

                default:
                    return "Not Defined";
            }
        }
    }
}

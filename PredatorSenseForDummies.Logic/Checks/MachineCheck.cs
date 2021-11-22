using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    public static class MachineCheck
    {
        private static bool _isDT;

        private static bool CheckMachineFromWMI()
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\root\\cimv2", new ConnectionOptions()
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                });
                scope.Connect();
                WqlObjectQuery wqlObjectQuery = new WqlObjectQuery("select * from Win32_SystemEnclosure");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, (ObjectQuery)wqlObjectQuery);
                ManagementObject managementObject1 = new ManagementObject();
                short num = 0;
                foreach (ManagementObject managementObject2 in managementObjectSearcher.Get())
                {
                    if (managementObject2 != null)
                    {
                        num = (managementObject2.Properties["ChassisTypes"].Value as short[])[0];
                        break;
                    }
                }
                managementObjectSearcher.Dispose();
                _isDT = (short)8 != num && (short)9 != num && (short)10 != num && (short)14 != num && (short)31 != num;
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static int CheckMachine()
        {
            CheckMachineFromWMI();
            return _isDT ? 1 : 0;
        }

    }
}

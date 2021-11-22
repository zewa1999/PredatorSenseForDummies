using Microsoft.Win32;
using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic.Checks
{
    public static class BrandCheck
{
        private static string strRegPath = @"Hardware\Description\System\BIOS";

        [SupportedOSPlatform("windows")]
        private static Brand CheckBrandFromReg()
        {
            var registryKey = Registry.LocalMachine.OpenSubKey(strRegPath); 
            Brand eBrand = Brand.Enum_NonAcer;

            if (registryKey != null)
            {
                var predatorModelType = registryKey.GetValue("SystemProductName").ToString();
                var manufacturerName = registryKey.GetValue("SystemManufacturer").ToString();

                bool flag = false;
                if (manufacturerName != null)
                {
                    string lower = manufacturerName.ToString().ToLower(CultureInfo.InvariantCulture);
                    if (lower.IndexOf("acer") >= 0)
                        flag = true;
                }

                if (predatorModelType != null && flag == true)
                {
                    string lower = predatorModelType.ToString().ToLower(CultureInfo.InvariantCulture);
                    if (lower.IndexOf("predator") >= 0)
                        eBrand = Brand.Enum_Acer;
                }
                registryKey.Close();
            }
            return eBrand;
        }

        /// <summary>
        /// Checks the brand in the registry
        /// The path for the keys that are checked is HKEY_LOCAL_MACHINE\Hardware\Description\System\BIOS
        /// The keys are SystemProductName and SystemManufacturer
        /// The SystemProductName needs to have predator in it's value
        /// The SystemManufacturer needs to have acer in it's value
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static Brand CheckBrand()
        {
            Brand eBrand;
            eBrand = CheckBrandFromReg();
            return eBrand;
        }
    }
}

using PredatorSenseForDummies.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic
{
    public class BasicCommandData
    {
        public int MachineType { get; set; }

        public ProductExistFlag ProductFlag { get; set; }

        public Brand MachineBrand { get; set; }

        public string? ProductOldVersion { get; set; }

        public string? ProductNewVersion { get; set; }

        public string? ProductName { get; set; }

        public string? ProductPath { get; set; }

        public string[]? InstallArgs { get; set; }
    }
}

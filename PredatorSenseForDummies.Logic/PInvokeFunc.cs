using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PredatorSenseForDummies.Logic
{
    internal class PInvokeFunc
    {
        internal const int TOKEN_QUERY = 8;
        internal const int TOKEN_ADJUST_PRIVILEGES = 32;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int SE_PRIVILEGE_ENABLED = 2;
        internal const int EWX_LOGOFF = 0;
        internal const int EWX_SHUTDOWN = 1;
        internal const int EWX_REBOOT = 2;
        internal const int EWX_FORCE = 4;
        internal const int EWX_POWEROFF = 8;
        internal const int EWX_RESTARTAPPS = 64;
        internal const int EWX_HYBRID_SHUTDOWN = 4194304;

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetupDiGetClassDevs(
          ref Guid ClassGuid,
          [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
          IntPtr hwndParent,
          uint Flags);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid hidGuid);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
          IntPtr hDevInfo,
          IntPtr devInfo,
          ref Guid interfaceClassGuid,
          uint memberIndex,
          ref PInvokeFunc.SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
          IntPtr DeviceInfoSet,
          uint MemberIndex,
          ref PInvokeFunc.SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiRemoveDevice(
          IntPtr DeviceInfoSet,
          ref PInvokeFunc.SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        public static extern int CM_Get_Device_ID(
          uint dnDevInst,
          IntPtr Buffer,
          int BufferLen,
          int ulFlags);

        [DllImport("setupapi.dll")]
        public static extern int CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, int ulFlags);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        public static extern uint CM_Get_DevNode_Registry_Property_Ex(
          uint dnDevInst,
          uint ulProperty,
          IntPtr pulRegDataType,
          byte[] Buffer,
          ref uint pulLength,
          uint ulFlags,
          IntPtr hMachine);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
          IntPtr hDevInfo,
          ref PInvokeFunc.SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
          ref PInvokeFunc.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
          uint deviceInterfaceDetailDataSize,
          out uint requiredSize,
          ref PInvokeFunc.SP_DEVINFO_DATA deviceInfoData);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(
          IntPtr htok,
          bool disall,
          ref PInvokeFunc.TokPriv1Luid newst,
          int len,
          IntPtr prev,
          IntPtr relen);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ExitWindowsEx(int flg, int rea);

        public static void DoExitWin(int flg)
        {
            IntPtr currentProcess = PInvokeFunc.GetCurrentProcess();
            IntPtr zero = IntPtr.Zero;
            PInvokeFunc.OpenProcessToken(currentProcess, 40, ref zero);
            PInvokeFunc.TokPriv1Luid newst;
            newst.Count = 1;
            newst.Luid = 0L;
            newst.Attr = 2;
            PInvokeFunc.LookupPrivilegeValue((string)null, "SeShutdownPrivilege", ref newst.Luid);
            PInvokeFunc.AdjustTokenPrivileges(zero, false, ref newst, 0, IntPtr.Zero, IntPtr.Zero);
            PInvokeFunc.ExitWindowsEx(flg, 0);
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(
          string lpDevice,
          uint iDevNum,
          ref PInvokeFunc.DISPLAY_DEVICE lpDisplayDevice,
          uint dwFlags);

        [DllImport("shell32.dll")]
        public static extern int ShellExecute(
          IntPtr hwnd,
          string lpszOp,
          string lpszFile,
          string lpszParams,
          string lpszDir,
          int FsShowCmd);

        public enum DIGCF_CONFIG
        {
            DEFAULT = 1,
            PRESENT = 2,
            ALLCLASSES = 4,
            PROFILE = 8,
            DEVICEINTERFACE = 16, // 0x00000010
        }

        public enum CM_CONFIG
        {
            DRP_DEVICEDESC = 1,
            DRP_HARDWAREID = 2,
            DRP_SPDRP_SERVICE = 5,
            DRP_CLASSGUID = 9,
            CM_DRP_MFG = 12, // 0x0000000C
            DRP_FRIENDLYNAME = 13, // 0x0000000D
            DRP_LOCATION_INFORMATION = 14, // 0x0000000E
            DRP_BUSTYPEGUID = 20, // 0x00000014
            DRP_DEVTYPE = 26, // 0x0000001A
        }

        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            private UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [Flags]
        public enum DisplayDeviceStateFlags
        {
            None = 0,
            AttachedToDesktop = 1,
            MultiDriver = 2,
            PrimaryDevice = 4,
            MirroringDriver = 8,
            VGACompatible = 22, // 0x00000016
            Removable = 32, // 0x00000020
            ModesPruned = 134217728, // 0x08000000
            Remote = 67108864, // 0x04000000
            Disconnect = 33554432, // 0x02000000
        }

        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;

            [MarshalAs(UnmanagedType.U4)]
            public PInvokeFunc.DisplayDeviceStateFlags StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
    }
}

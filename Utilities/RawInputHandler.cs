using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RawMat.Utilities
{

    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class RawInputHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            public uint dwType;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [DllImport("user32.dll")]
        private static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        private static extern void HidD_GetHidGuid(out Guid HidGuid);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, StringBuilder DeviceInstanceId, uint DeviceInstanceIdSize, out uint RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, out uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

        [DllImport("setupapi.dll")]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        private const uint RIM_TYPEKEYBOARD = 1;
        private const uint RIDI_DEVICENAME = 0x20000007;
        private const uint DIGCF_PRESENT = 0x00000002;
        private const uint DIGCF_ALLCLASSES = 0x00000004;
        private const uint SPDRP_DEVICEDESC = 0x00000000;

        public static (string DeviceName, string DeviceDesc)[] GetConnectedDevices()
        {
            uint deviceCount = 0;
            uint deviceListSize = (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));

            if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, deviceListSize) != 0)
            {
                Console.WriteLine($"Initial GetRawInputDeviceList failed, error: {Marshal.GetLastWin32Error()}");
                return new (string, string)[0];
            }

            var deviceList = new RAWINPUTDEVICELIST[deviceCount];
            IntPtr pDeviceList = Marshal.AllocHGlobal((int)(deviceCount * deviceListSize));

            try
            {
                uint result = GetRawInputDeviceList(pDeviceList, ref deviceCount, deviceListSize);
                if (result != 0)
                {
                    Console.WriteLine($"GetRawInputDeviceList failed, error: {Marshal.GetLastWin32Error()}");
                    return new (string, string)[0];
                }

                var devices = new (string DeviceName, string DeviceDesc)[deviceCount];
                int index = 0;

                for (uint i = 0; i < deviceCount; i++)
                {
                    RAWINPUTDEVICELIST device = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                        new IntPtr(pDeviceList.ToInt64() + i * deviceListSize), typeof(RAWINPUTDEVICELIST));

                    if (device.dwType == RIM_TYPEKEYBOARD)
                    {
                        uint nameSize = 0;
                        if (GetRawInputDeviceInfo(device.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref nameSize) == 0)
                        {
                            Console.WriteLine($"GetRawInputDeviceInfo size failed for device {i}, error: {Marshal.GetLastWin32Error()}");
                            continue;
                        }

                        IntPtr namePtr = Marshal.AllocHGlobal((int)nameSize);
                        try
                        {
                            if (GetRawInputDeviceInfo(device.hDevice, RIDI_DEVICENAME, namePtr, ref nameSize) > 0)
                            {
                                string deviceName = Marshal.PtrToStringAnsi(namePtr);
                                string deviceDesc = GetDeviceDescription(deviceName);
                                devices[index++] = (deviceName, deviceDesc);
                            }
                            else
                            {
                                Console.WriteLine($"GetRawInputDeviceInfo failed for device {i}, error: {Marshal.GetLastWin32Error()}");
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(namePtr);
                        }
                    }
                }

                Array.Resize(ref devices, index);
                return devices;
            }
            finally
            {
                Marshal.FreeHGlobal(pDeviceList);
            }
        }

        public static string GetDeviceDescription(string deviceName)
        {
            Guid hidGuid;
            HidD_GetHidGuid(out hidGuid);

            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);
            if (hDevInfo == IntPtr.Zero)
            {
                Console.WriteLine($"SetupDiGetClassDevs failed, error: {Marshal.GetLastWin32Error()}");
                return "Unknown";
            }

            SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA))
            };
            uint devIndex = 0;

            try
            {
                while (SetupDiEnumDeviceInfo(hDevInfo, devIndex++, ref devInfoData))
                {
                    StringBuilder deviceInstanceId = new StringBuilder(256);
                    uint requiredSize = 0;

                    if (!SetupDiGetDeviceInstanceId(hDevInfo, ref devInfoData, deviceInstanceId, 256, out requiredSize))
                    {
                        Console.WriteLine($"SetupDiGetDeviceInstanceId failed, error: {Marshal.GetLastWin32Error()}");
                        continue;
                    }

                    string instanceId = deviceInstanceId.ToString().ToLower();
                    string normalizedDeviceName = deviceName.ToLower().Replace(@"\\?\", "").Replace("#", @"\");

                    Console.WriteLine($"Comparing: DeviceName={normalizedDeviceName}, InstanceId={instanceId}");

                    if (instanceId.Contains(normalizedDeviceName.Split('\\')[1]))
                    {
                        uint propertyRegDataType;
                        if (SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfoData, SPDRP_DEVICEDESC, out propertyRegDataType, IntPtr.Zero, 0, out requiredSize))
                        {
                            IntPtr buffer = Marshal.AllocHGlobal((int)requiredSize);
                            try
                            {
                                if (SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfoData, SPDRP_DEVICEDESC, out propertyRegDataType, buffer, requiredSize, out requiredSize))
                                {
                                    string desc = Marshal.PtrToStringAuto(buffer);
                                    Console.WriteLine($"Found DeviceDesc: {desc} for DeviceName: {deviceName}");
                                    return desc ?? "Unknown";
                                }
                                else
                                {
                                    Console.WriteLine($"SetupDiGetDeviceRegistryProperty failed, error: {Marshal.GetLastWin32Error()}");
                                }
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(buffer);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"SetupDiGetDeviceRegistryProperty (size) failed, error: {Marshal.GetLastWin32Error()}");
                        }
                    }
                }
                Console.WriteLine($"No matching device found for DeviceName: {deviceName}");
                return "Unknown";
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }
    }

}

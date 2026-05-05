using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RawMat.Utilities
{

    public class LowLevelKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int RIM_TYPEKEYBOARD = 1;
        private const uint RIDI_DEVICEINFO = 0x2000000b;
        private const uint RIDI_DEVICENAME = 0x20000007;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static DateTime _lastKeyPressTime = DateTime.MinValue;
        private static bool _isFromDevice = false;
        private static bool _isEditingValue = false;
        public static string _currentDeviceDesc = "Unknown";
        private static IntPtr _currentDeviceHandle = IntPtr.Zero;

        public static bool IsFromDevice => _isFromDevice;
        public static bool IsEditingValue
        {
            get => _isEditingValue;
            set
            {
                _isEditingValue = value;
                if (_isEditingValue)
                {
                    _lastKeyPressTime = DateTime.MinValue;
                    _isFromDevice = true;
                    UpdateDeviceList();
                    if (string.IsNullOrEmpty(_currentDeviceDesc))
                        _currentDeviceDesc = "Unknown";
                    Console.WriteLine($"Reset: IsEditingValue = true, IsFromDevice = true, Device: {_currentDeviceDesc}");
                }
            }
        }

        public static event EventHandler<KeyPressedEventArgs> KeyPressed;

        public static void SetHook()
        {
            _hookID = SetHook(_proc);
        }

        public static void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // ดึงข้อมูล RawInput
                uint rawInputSize = 0;
                if (GetRawInputData(lParam, 0x10000003 /* RID_INPUT */, IntPtr.Zero, ref rawInputSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == 0)
                {
                    Console.WriteLine($"GetRawInputData size failed, error: {Marshal.GetLastWin32Error()}");
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }

                IntPtr rawInputBuffer = Marshal.AllocHGlobal((int)rawInputSize);
                try
                {
                    if (GetRawInputData(lParam, 0x10000003 /* RID_INPUT */, rawInputBuffer, ref rawInputSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == rawInputSize)
                    {
                        RAWINPUT rawInput = Marshal.PtrToStructure<RAWINPUT>(rawInputBuffer);
                        if (rawInput.header.dwType == RIM_TYPEKEYBOARD)
                        {
                            _currentDeviceHandle = rawInput.header.hDevice;
                            string deviceDesc = GetDeviceDescFromHandle(_currentDeviceHandle);
                            _currentDeviceDesc = string.IsNullOrEmpty(deviceDesc) ? "Unknown" : deviceDesc;
                            Console.WriteLine($"Device identified: {_currentDeviceDesc} from handle {rawInput.header.hDevice}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"GetRawInputData failed, error: {Marshal.GetLastWin32Error()}");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(rawInputBuffer);
                }

                Console.WriteLine($"KeyDown detected, vkCode: {vkCode}, Device: {_currentDeviceDesc}");

                if (_isEditingValue && IsRelevantKey(vkCode))
                {
                    _isFromDevice = true;
                    Console.WriteLine($"Key processed, vkCode: {vkCode}, IsFromDevice: {_isFromDevice}, Device: {_currentDeviceDesc}");
                    KeyPressed?.Invoke(null, new KeyPressedEventArgs(vkCode));
                }
                else if (_isEditingValue)
                {
                    Console.WriteLine($"Key ignored: vkCode {vkCode}, IsRelevantKey: {IsRelevantKey(vkCode)}, Device: {_currentDeviceDesc}");
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static string GetDeviceDescFromHandle(IntPtr deviceHandle)
        {
            uint nameSize = 0;
            if (GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, IntPtr.Zero, ref nameSize) == 0)
            {
                Console.WriteLine($"GetRawInputDeviceInfo size failed for handle, error: {Marshal.GetLastWin32Error()}");
                return "Unknown";
            }

            IntPtr namePtr = Marshal.AllocHGlobal((int)nameSize);
            try
            {
                if (GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, namePtr, ref nameSize) > 0)
                {
                    string deviceName = Marshal.PtrToStringAnsi(namePtr);
                    return RawInputHandler.GetDeviceDescription(deviceName);
                }
                else
                {
                    Console.WriteLine($"GetRawInputDeviceInfo failed for device handle, error: {Marshal.GetLastWin32Error()}");
                    return "Unknown";
                }
            }
            finally
            {
                Marshal.FreeHGlobal(namePtr);
            }
        }

        private static bool IsRelevantKey(int vkCode)
        {
            bool isRelevant = (vkCode >= 48 && vkCode <= 57) || // 0-9
                              vkCode == 190 || // จุดทศนิยม (.)
                              vkCode == 8 ||   // Backspace
                              vkCode == 16 ||  // Shift
                              vkCode == 13;  // Enter

            Console.WriteLine($"IsRelevantKey: vkCode {vkCode}, Result: {isRelevant}");
            return isRelevant;
        }

        private static void UpdateDeviceList()
        {
            var devices = RawInputHandler.GetConnectedDevices();
            if (devices.Length == 0)
            {
                Console.WriteLine("No devices found in UpdateDeviceList");
                _currentDeviceDesc = "Unknown";
                return;
            }

            foreach (var device in devices)
            {
                Console.WriteLine($"Connected device - Name: {device.Item1}, Desc: {device.Item2}");
                _currentDeviceDesc = device.Item2;
                break; // ใช้อุปกรณ์แรกที่พบชั่วคราว
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RAWINPUT
        {
            [FieldOffset(0)] public RAWINPUTHEADER header;
            [FieldOffset(16)] public RAWKEYBOARD keyboard;
        }

        [DllImport("user32.dll")]
        private static extern uint GetRawInputData(IntPtr lParam, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class KeyPressedEventArgs : EventArgs
    {
        public int VkCode { get; private set; }
        public KeyPressedEventArgs(int vkCode)
        {
            VkCode = vkCode;
        }
    }
}

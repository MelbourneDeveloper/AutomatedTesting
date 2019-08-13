using System;
using System.Runtime.InteropServices;

namespace AutomatedTestingFramework
{
    public static class WindowsAPI
    {
        /// <summary>
        /// http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/2abc6be8-c593-4686-93d2-89785232dacd
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646270(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/en/csharplanguage/thread/f0e82d6e-4999-4d22-b3d3-32b25f61fb2a
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, Int32 cbSize);


        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;

        //This simulates a left mouse click
        public static void MouseClick(int xpos, int ypos, bool isRightClick)
        {
            SetCursorPos(xpos, ypos);

            if (!isRightClick)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
            }
            else
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
            }
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(KeyCode keyCode)
        {
            var input = new INPUT
            {
                Type = 1,
                Data =
                {
                    Keyboard = new KEYBDINPUT
                    {
                        Vk = (ushort)keyCode,
                        Scan = 0,
                        Flags = 0,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };
            INPUT[] inputs = { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Release a key that is being hold down
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(KeyCode keyCode)
        {
            var input = new INPUT
            {
                Type = 1,
                Data =
                {
                    Keyboard = new KEYBDINPUT
                    {
                        Vk = (ushort)keyCode,
                        Scan = 0,
                        Flags = 2,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };
            INPUT[] inputs = { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();

        }

        public static void SendKeyPress(KeyCode keyCode)
        {
            SendKeyPress(keyCode, false);
        }

        public static void SendKeyPress(KeyCode keyCode, bool shift)
        {
            if (shift)
            {
                SendKeyDown(KeyCode.SHIFT);
            }

            var input = new INPUT
            {
                Type = 1,
                Data =
                {
                    Keyboard = new KEYBDINPUT
                    {
                        Vk = (ushort)keyCode,
                        Scan = (ushort)keyCode,
                        Flags = 0,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            var input2 = new INPUT
            {
                Type = 1,
                Data =
                {
                    Keyboard = new KEYBDINPUT
                    {
                        Vk = (ushort)keyCode,
                        Scan = (ushort)keyCode,
                        Flags = 2,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            INPUT[] inputs = { input, input2 };

            var retVal = SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
            if (retVal == 0)
            {
                throw new Exception();
            }

            if (shift)
            {
                SendKeyUp(KeyCode.SHIFT);
            }
        }

    }
}

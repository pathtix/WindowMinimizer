using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowMinimizator
{
    internal class WindowMinimizator
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.x, point.y);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern uint RealGetWindowClass(IntPtr hwnd, StringBuilder pszType, uint cchType);

        private delegate bool EnumWindowsDelegate(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MINIMIZE = 6;

        public static Point GetCursorPosition()
        {
            GetCursorPos(out POINT lpPoint);
            return lpPoint;
        }

        //public static int CheckReachBottomLeft(Point lpPoint)s
        public static void CheckReachBottomCorners(Point lpPoint, int screenHeight)
        {
            if (lpPoint.X == 0 && lpPoint.Y == screenHeight) { MinizmizeFocusedApplication(); }
        }
        public static void MinizmizeFocusedApplication()
        {
            Process fgProc = getForegroundProcess();
            var processArray = Process.GetProcesses();
            var process = processArray.FirstOrDefault(p => p.ProcessName == fgProc.ProcessName);
            DateTime now = DateTime.Now;
            switch (process.ProcessName)
            {
                case "explorer":
                    {
                        EnumWindowsDelegate childProc = new EnumWindowsDelegate(EnumWindowsCallback);
                        EnumWindows(childProc, nint.Zero);
                        break;
                    }

                default:
                    ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
                    Console.WriteLine($"{now} | {fgProc.ProcessName} minimized!");
                    break;
            }
            Thread.Sleep(250);
        }


        private static bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam)
        {
            IntPtr pid = new IntPtr();
            GetWindowThreadProcessId(hwnd, out pid);
            var wndProcess = System.Diagnostics.Process.GetProcessById(pid.ToInt32());
            var wndClass = new StringBuilder(255);
            DateTime now = DateTime.Now;
            RealGetWindowClass(hwnd, wndClass, 255);
            if (wndProcess.ProcessName == "explorer" && wndClass.ToString() == "CabinetWClass")
            {
                if (ShowWindow(hwnd, SW_MINIMIZE))
                    Console.WriteLine($"{now} | {wndProcess.ProcessName} minimized!");
            }
            return (true);
        }

        public static Process getForegroundProcess()
        {
            IntPtr hWnd = GetForegroundWindow(); // Get foreground window handle
            uint threadID = GetWindowThreadProcessId(hWnd, out uint processID);
            Process fgProc = Process.GetProcessById(Convert.ToInt32(processID));
            return fgProc;
        }

    }
}



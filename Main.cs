using System.Drawing;
using System.Runtime.InteropServices;
using static WindowMinimizator.WindowMinimizator;

class MainWindow
{
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_CYSCREEN = 1;

    [STAThread]
    static public void Main(String[] args)
    {
        var screenHeight = GetSystemMetrics(SM_CYSCREEN);
        Console.WriteLine("Window Minimizator is currently working, hover bottom left to minimize focused window.");
        
        while (true)
        {
            Point lpPoint = GetCursorPosition();
            CheckReachBottomCorners(lpPoint, screenHeight);
            Thread.Sleep(100);
        }
    }
}
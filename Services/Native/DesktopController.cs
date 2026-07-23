using System.Runtime.InteropServices;

namespace AnkiFocusEnforcer.Services.Native;

public static class DesktopController
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll")]
    private static extern int ShowWindow(IntPtr hwnd, int command);

    public static void HideDesktopIcons() { ToggleDesktopIcons(0); } // 0 = Ẩn
    public static void ShowDesktopIcons() { ToggleDesktopIcons(5); } // 5 = Hiện

    private static void ToggleDesktopIcons(int command)
    {
        IntPtr hwndWalk = IntPtr.Zero;
        IntPtr workerW = IntPtr.Zero;
        
        do
        {
            workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
            if (workerW != IntPtr.Zero)
            {
                IntPtr tempShellDefView = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (tempShellDefView != IntPtr.Zero)
                {
                    hwndWalk = tempShellDefView;
                    break;
                }
            }
        } while (workerW != IntPtr.Zero);

        // Nếu không tìm thấy bằng WorkerW, tìm bằng Progman
        if (hwndWalk == IntPtr.Zero)
        {
            IntPtr hwndProgman = FindWindow("Progman", "Program Manager");
            hwndWalk = FindWindowEx(hwndProgman, IntPtr.Zero, "SHELLDLL_DefView", null);
        }
        
        if (hwndWalk != IntPtr.Zero)
        {
            IntPtr iconWindow = FindWindowEx(hwndWalk, IntPtr.Zero, "SysListView32", "FolderView");
            if (iconWindow != IntPtr.Zero) ShowWindow(iconWindow, command);
        }
    }
}
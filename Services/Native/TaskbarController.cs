using System.Runtime.InteropServices;

namespace AnkiFocusEnforcer.Services.Native;

public static class TaskbarController
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

    public static void DisableTaskbarInteraction()
    {
        IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
        if (taskbarHandle != IntPtr.Zero)
        {
            EnableWindow(taskbarHandle, false); 
        }
        IntPtr startButtonHandle = FindWindow("Button", "Start");
        if (startButtonHandle != IntPtr.Zero)
        {
            EnableWindow(startButtonHandle, false);
        }
    }

    public static void EnableTaskbarInteraction()
    {
        IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
        if (taskbarHandle != IntPtr.Zero)
        {
            EnableWindow(taskbarHandle, true); 
        }

        IntPtr startButtonHandle = FindWindow("Button", "Start");
        if (startButtonHandle != IntPtr.Zero)
        {
            EnableWindow(startButtonHandle, true);
        }
    }
}
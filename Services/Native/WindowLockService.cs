namespace AnkiFocusEnforcer.Services.Native;

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Interfaces;

public class WindowLockService : IWindowService
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);
    
    private IntPtr GetRealMainWindowHandle(string processName)
    {
        IntPtr foundHandle = IntPtr.Zero;
        var pids = Process.GetProcessesByName(processName).Select(p => p.Id).ToList();

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true;

            GetWindowThreadProcessId(hWnd, out uint processId);

            // Bỏ kiểm tra Title, chỉ bắt chính xác theo Process ID
            if (pids.Contains((int)processId))
            {
                foundHandle = hWnd;
                return false; 
            }
            return true;
        }, IntPtr.Zero);

        return foundHandle;
    }

    public bool IsAppInForeground(string processName)
    {
        IntPtr activeWindow = Win32Native.GetForegroundWindow();
        if (activeWindow == IntPtr.Zero) return false;

        GetWindowThreadProcessId(activeWindow, out uint processId);
        
        try
        {
            using (var process = Process.GetProcessById((int)processId))
            {
                return process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch
        {
            return false;
        }
    }

    public bool LockFocusToApp(string processName)
    {
        IntPtr hWnd = GetRealMainWindowHandle(processName);
        
        if (hWnd != IntPtr.Zero)
        {
            if (Win32Native.IsIconic(hWnd))
            {
                Win32Native.ShowWindow(hWnd, Win32Native.SW_RESTORE);
            }

            // Ghim lên trên cùng
            Win32Native.SetWindowPos(hWnd, Win32Native.HWND_TOPMOST, 0, 0, 0, 0,
                Win32Native.SWP_NOMOVE | Win32Native.SWP_NOSIZE);
            Win32Native.keybd_event(0x12, 0, 0, UIntPtr.Zero);
            Win32Native.keybd_event(0x12, 0, Win32Native.KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Ép đưa cửa sổ lên foreground
            Win32Native.SwitchToThisWindow(hWnd, true);
            return Win32Native.SetForegroundWindow(hWnd);
        }
        
        return false;
    }

    public void ReleaseFocus()
    {
        IntPtr hWnd = GetRealMainWindowHandle("anki");
        if (hWnd != IntPtr.Zero)
        {
            Win32Native.SetWindowPos(hWnd, Win32Native.HWND_NOTOPMOST, 0, 0, 0, 0, 
                Win32Native.SWP_NOMOVE | Win32Native.SWP_NOSIZE);
        }
    }

    public bool IsAppRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any() || GetRealMainWindowHandle(processName) != IntPtr.Zero;
    }
}
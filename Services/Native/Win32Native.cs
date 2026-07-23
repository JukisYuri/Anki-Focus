using System.Runtime.InteropServices;
namespace AnkiFocusEnforcer.Services.Native;

internal static class Win32Native
{
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
    
    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    // Hằng số cho SetWindowPos và ShowWindow
    public static readonly IntPtr HWND_TOPMOST = -1;               // Ghim lên trên mọi cửa sổ
    public static readonly IntPtr HWND_NOTOPMOST = -2;             // Gỡ ghim
    public const uint SWP_NOMOVE = 0x0002;                         // Không đổi vị trí
    public const uint SWP_NOSIZE = 0x0001;                         // Không đổi kích thước
    public const int SW_RESTORE = 9;
    public const int KEYEVENTF_KEYUP = 0x0002;
}
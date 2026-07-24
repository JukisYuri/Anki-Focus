namespace AnkiFocusEnforcer.Core.Model;

public class AppSettings
{
    public int DefaultFocusDuration { get; set; } = 15;
    public bool AutoPauseMedia { get; set; } = true;   // Tự động dừng Media
    public bool LockShortcuts { get; set; } = true;    // Khóa phím tắt (Alt+Tab, Win, Alt+F4...)
    public bool LockTaskbar { get; set; } = true;      // Khóa Taskbar & Nút Start
    public bool HideDesktopIcons { get; set; } = true; // Ẩn Icon ngoài Desktop
    public bool PreventSleep { get; set; } = true;     // Chống tắt màn hình / Sleep
}
namespace AnkiFocusEnforcer.Core.Interfaces;

public interface IWindowService
{
    bool LockFocusToApp(string processName);
    void ReleaseFocus();
    bool IsAppRunning(string processName);
    bool IsAppInForeground(string processName);
}
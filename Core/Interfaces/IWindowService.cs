namespace AnkiFocusEnforcer.Core.Interfaces;

public interface IWindowService
{
    bool IsAppRunning(string processName);
}
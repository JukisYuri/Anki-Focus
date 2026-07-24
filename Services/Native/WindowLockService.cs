namespace AnkiFocusEnforcer.Services.Native;

using System.Diagnostics;
using System.Linq;
using Core.Interfaces;

public class WindowLockService : IWindowService
{
    public bool IsAppRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any();
    }
}
using AnkiFocusEnforcer.Core.Interfaces;
using AnkiFocusEnforcer.Services;
using AnkiFocusEnforcer.Services.Native;

namespace AnkiFocusEnforcer;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Các cấu hình mặc định của WinForms
        ApplicationConfiguration.Initialize();
        IWindowService windowService = new WindowLockService();
        IMediaService mediaService = new MediaControlService();
        IFocusService focusService = new FocusService(windowService, mediaService);
        
        MainForm mainForm = new MainForm(focusService);
        Application.Run(mainForm);
    }
}
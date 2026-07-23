namespace AnkiFocusEnforcer.Core.Model;

public class AppSettings
{
    public int DefaultFocusDuration { get; set; } = 15;
    public bool AutoPauseMedia { get; set; } = true;
    public bool LockAltTab { get; set; } = true;
}
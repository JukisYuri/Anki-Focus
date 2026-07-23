namespace AnkiFocusEnforcer.Core.Model;

public enum SessionState
{
    Stopped,
    Running,
    Completed
}

public class FocusSession 
{
    public int DurationInMinutes { get; set; }
    public int RemainingSeconds { get; set; }
    public SessionState State { get; set; }
    
    public FocusSession(int durationInMinutes)
    {
        DurationInMinutes = durationInMinutes;
        RemainingSeconds = durationInMinutes * 60;
        State = SessionState.Stopped;
    }
}
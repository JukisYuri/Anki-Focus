using System.Timers;
using AnkiFocusEnforcer.Core.Interfaces;
using AnkiFocusEnforcer.Core.Model;

namespace AnkiFocusEnforcer.Services;

public class FocusService : IFocusService
{
    private readonly IMediaService _mediaService;
    private readonly IWindowService _windowService;
    private readonly System.Timers.Timer _timer;
    private FocusSession _currentSession;

    public event Action<int> OnTick;
    public event Action OnSessionCompleted;
    public event Action OnLockdownActivated; 
    
    public FocusService(IWindowService windowService, IMediaService mediaService)
    {
        _windowService = windowService;
        _mediaService = mediaService;
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed; 
    }

    public void StartSession(int minutes)
    {
        if (!_windowService.IsAppRunning("anki"))
            throw new Exception("Không tìm thấy Anki! Hãy mở Anki lên trước rồi mới bấm Bắt đầu nhé.");

        _currentSession = new FocusSession(minutes);
        _currentSession.State = SessionState.Running;
        
        _mediaService.PauseAllMedia();
        OnLockdownActivated?.Invoke(); 
        
        _timer.Start();
    }

    public void StopSession()
    {
        _timer.Stop();
        _currentSession.State = SessionState.Completed;
        OnSessionCompleted?.Invoke();
    }
    
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_currentSession.State != SessionState.Running) return;
        _currentSession.RemainingSeconds--;
        OnTick.Invoke(_currentSession.RemainingSeconds);
        if (_currentSession.RemainingSeconds <= 0)
        {
            StopSession(); 
        }
    }
}
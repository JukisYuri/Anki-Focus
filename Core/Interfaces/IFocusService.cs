using AnkiFocusEnforcer.Core.Model;

namespace AnkiFocusEnforcer.Core.Interfaces;

public interface IFocusService
{
    void StartSession(int minutes);
    event Action<int> OnTick;          
    event Action OnSessionCompleted;   
    event Action OnLockdownActivated;
}
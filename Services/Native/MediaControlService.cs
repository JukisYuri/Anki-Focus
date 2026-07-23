using Windows.Media.Control;
using AnkiFocusEnforcer.Core.Interfaces;

namespace AnkiFocusEnforcer.Services.Native
{
    public class MediaControlService : IMediaService
    {
        private bool _hasPaused;
        public void PauseAllMedia()
        {
            if (!_hasPaused)
            {
                Task.Run(async () => await PauseIfPlayingAsync()).GetAwaiter().GetResult();
                _hasPaused = true;
            }
        }

        private async Task PauseIfPlayingAsync()
        {
            try
            {
                var sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var currentSession = sessionManager.GetCurrentSession();
                if (currentSession != null)
                {
                    var playbackInfo = currentSession.GetPlaybackInfo();
                    if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        await currentSession.TryPauseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Media API: {ex.Message}");
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.ViewControllers;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.UI
{
    public abstract class AbstractView : BSMLAutomaticViewController
    {
        [Inject] protected readonly SiraLog _log = null!;
        
        protected async void SafeAwait<T>(Task<T> task, Action<T> onSuccess, Action? onError = null)
        {
            try
            {
                onSuccess(await task);
            }
            catch (Exception e)
            {
                HandleException(e);
                onError?.Invoke();
            }
        }
        
        private void HandleException(Exception e)
        {
            _log.Error(e);
        }
    }
}
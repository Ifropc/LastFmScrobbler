using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using LastFmScrobbler.Utils;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.UI
{
    public abstract class AbstractView : BSMLAutomaticViewController
    {
        [Inject] protected readonly SiraLog _log = null!;

        [UIParams] protected BSMLParserParams _parserParams = null!;

        private string _errorModalText;
        [UIValue("error-modal-text")]
        protected string ErrorModalText
        {
            get => _errorModalText;
            set
            {
                _errorModalText = value;
                NotifyPropertyChanged();
            }
        }

        private Action? _modalConfirm;
        [UIAction("info-modal-confirm")]
        protected void ModalConfirm()
        {
            _modalConfirm?.Invoke();
            _parserParams.EmitEvent("hide-info-modal");
        }

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
            
            var message = $"Error: {e.Message}. ";
            
            if (e is LastFmException lastException)
            {
                if (lastException.ShouldBeReported())
                {
                    message += "Please report this error on Github with logs attached.";
                }
                if (lastException.TokenNotAuthorized())
                {
                    message = "Please allow access on Last.Fm before clicking Confirm button.";
                }
            }

            ShowErrorModal(message);
        }

        protected void ShowInfoModal(Action onConfirm)
        {
            _modalConfirm = onConfirm;
            _parserParams.EmitEvent("show-info-modal");
        }

        protected void ShowErrorModal(string text)
        {
            ErrorModalText = text;
            _parserParams.EmitEvent("show-error-modal");
        }
    }
}
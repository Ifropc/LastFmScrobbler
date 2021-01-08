using System;
using BS_LastFm_Scrobbler.Patches;
using SiraUtil.Tools;
using Zenject;

namespace BS_LastFm_Scrobbler.Managers
{
    public class SongManager : IInitializable, IDisposable
    {
        [Inject] private LevelCollectionViewController _levelCollectionViewController;
        [Inject] private SiraLog _log;

        private IPreviewBeatmapLevel _selected;

        public void Initialize()
        {
            _levelCollectionViewController.didSelectLevelEvent += OnEventSelected;
            TransitionHelperPatch.LevelStarted += onLevelStarted;
         
        }

        public void Dispose()
        {
            _levelCollectionViewController.didSelectLevelEvent -= OnEventSelected;
        }

        private void OnEventSelected(LevelCollectionViewController _, IPreviewBeatmapLevel beatmapPreview)
        {
            _selected = beatmapPreview;
            _log.Debug($"Selected {beatmapPreview.songAuthorName} : {beatmapPreview.songName}");
        }

        private void onLevelStarted()
        {
            _log.Debug("Level started");
        }

        private void onLevelFinished(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupDataSo, LevelCompletionResults levelCompletionResults)
        {
            _log.Debug("Level finished");
        }
    }
}
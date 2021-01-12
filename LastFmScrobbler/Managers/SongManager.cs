using System;
using LastFmScrobbler.Components;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class SongManager : IInitializable, IDisposable
    {
        private Action<LevelCollectionViewController, IPreviewBeatmapLevel> _eventSelectedAction;
        [Inject] private LevelCollectionViewController _levelCollectionViewController;
        [Inject] private SiraLog _log;
        [Inject] private MissionSelectionMapViewController _missionSelection;


        private IPreviewBeatmapLevel _selected;
        [Inject] private LastFmMenuTransitionHelper _transitionHelper;

        public void Dispose()
        {
            _levelCollectionViewController.didSelectLevelEvent -= _eventSelectedAction;
        }

        public void Initialize()
        {
            _eventSelectedAction = (_, x) => OnEventSelected(x);

            _levelCollectionViewController.didSelectLevelEvent += _eventSelectedAction;
            _missionSelection.didSelectMissionLevelEvent += OnMissionEventSelected;
            _transitionHelper.SongSelectedEvent += OnEventSelected;
            _transitionHelper.SongDidStartEvent += OnLevelStarted;
            _transitionHelper.SongDidFinishEvent += OnLevelFinished;
        }

        private void OnEventSelected(IPreviewBeatmapLevel beatmapPreview)
        {
            _selected = beatmapPreview;
            _log.Debug($"Selected {beatmapPreview.songAuthorName}:{beatmapPreview.songName}");
        }

        private void OnMissionEventSelected(MissionSelectionMapViewController c, MissionNode n)
        {
            OnEventSelected(n.missionData.level);
        }

        private void OnLevelStarted(float offset)
        {
            _log.Debug($"Level started: {_selected.songName}:{_selected.songDuration}:{offset}");
        }

        private void OnLevelFinished(LevelCompletionResults results)
        {
            _log.Debug($"Level finished : {results.songDuration} : {results.endSongTime}");
        }
    }
}
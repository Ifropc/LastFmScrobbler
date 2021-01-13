using System;
using System.Collections.Generic;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Components
{
    public class LastFmMenuTransitionHelper : MenuTransitionsHelper
    {
        [Inject] private readonly SiraLog _log= null!;

        public event Action<IPreviewBeatmapLevel>? SongSelectedEvent;
        public event Action<float>? SongStartedEvent;
        public event Action<LevelCompletionResults>? SongFinishedEvent;

        public override void StartStandardLevel(
            string gameMode,
            IDifficultyBeatmap difficultyBeatmap,
            OverrideEnvironmentSettings overrideEnvironmentSettings,
            ColorScheme overrideColorScheme,
            GameplayModifiers gameplayModifiers,
            PlayerSpecificSettings playerSpecificSettings,
            PracticeSettings practiceSettings,
            string backButtonText,
            bool useTestNoteCutSoundEffects,
            Action beforeSceneSwitchCallback,
            Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> levelFinishedCallback)
        {
            SendSongDidStartEvent(practiceSettings);
            levelFinishedCallback += (_, r) => SongFinishedEvent?.Invoke(r);

            base.StartStandardLevel(
                gameMode,
                difficultyBeatmap,
                overrideEnvironmentSettings,
                overrideColorScheme,
                gameplayModifiers,
                playerSpecificSettings,
                practiceSettings,
                backButtonText,
                useTestNoteCutSoundEffects,
                beforeSceneSwitchCallback,
                levelFinishedCallback
            );
        }

        public override void StartMissionLevel(
            string missionId,
            IDifficultyBeatmap difficultyBeatmap,
            ColorScheme overrideColorScheme,
            GameplayModifiers gameplayModifiers,
            MissionObjective[] missionObjectives,
            PlayerSpecificSettings playerSpecificSettings,
            Action beforeSceneSwitchCallback,
            Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults> levelFinishedCallback
        )
        {
            SendSongDidStartEvent(null);
            levelFinishedCallback += (_, r) => SongFinishedEvent?.Invoke(r.levelCompletionResults);

            base.StartMissionLevel(
                missionId,
                difficultyBeatmap,
                overrideColorScheme,
                gameplayModifiers,
                missionObjectives,
                playerSpecificSettings,
                beforeSceneSwitchCallback,
                levelFinishedCallback
            );
        }

        public override void StartMultiplayerLevel(
            string gameMode,
            IPreviewBeatmapLevel previewBeatmapLevel,
            BeatmapDifficulty beatmapDifficulty,
            BeatmapCharacteristicSO beatmapCharacteristic,
            IDifficultyBeatmap difficultyBeatmap,
            ColorScheme overrideColorScheme,
            GameplayModifiers gameplayModifiers,
            PlayerSpecificSettings playerSpecificSettings,
            PracticeSettings practiceSettings,
            string backButtonText,
            bool useTestNoteCutSoundEffects,
            Action beforeSceneSwitchCallback,
            Action<MultiplayerLevelScenesTransitionSetupDataSO, LevelCompletionResults,
                Dictionary<string, LevelCompletionResults>> levelFinishedCallback,
            Action<DisconnectedReason> didDisconnectCallback)
        {
            _log.Debug("Started multiplayer level");

            SongSelectedEvent?.Invoke(previewBeatmapLevel);
            SendSongDidStartEvent(null);
            levelFinishedCallback += (_, r, _) => SongFinishedEvent?.Invoke(r);

            base.StartMultiplayerLevel(
                gameMode,
                previewBeatmapLevel,
                beatmapDifficulty,
                beatmapCharacteristic,
                difficultyBeatmap,
                overrideColorScheme,
                gameplayModifiers,
                playerSpecificSettings,
                practiceSettings,
                backButtonText,
                useTestNoteCutSoundEffects,
                beforeSceneSwitchCallback,
                levelFinishedCallback,
                didDisconnectCallback
            );
        }

        public void SendSongDidStartEvent(PracticeSettings? practiceSettings)
        {
            SongStartedEvent?.Invoke(practiceSettings?.startSongTime ?? 0);
            _log.Debug("Sent song start event");
        }
    }
}
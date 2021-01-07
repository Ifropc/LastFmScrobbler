using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;


namespace BS_LastFm_Scrobbler.Components
{
    public class MyMenuTransitionHelper : MenuTransitionsHelper
    {
        [Inject] private readonly SiraLog _log = null;
        
        public event Action<IDifficultyBeatmap> SongDidStartEvent;

        public override void StartStandardLevel(string gameMode, IDifficultyBeatmap difficultyBeatmap,
            OverrideEnvironmentSettings overrideEnvironmentSettings, ColorScheme overrideColorScheme,
            GameplayModifiers gameplayModifiers, PlayerSpecificSettings playerSpecificSettings,
            PracticeSettings practiceSettings, string backButtonText, bool useTestNoteCutSoundEffects,
            Action beforeSceneSwitchCallback, Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> levelFinishedCallback)
        {
            _log.Debug("Start standard level called");
            
            SendSongDidStartEvent(difficultyBeatmap);
            
            base.StartStandardLevel(gameMode, difficultyBeatmap, overrideEnvironmentSettings, overrideColorScheme, gameplayModifiers, playerSpecificSettings, practiceSettings, backButtonText, useTestNoteCutSoundEffects, beforeSceneSwitchCallback, levelFinishedCallback);
        }
        
        public void SendSongDidStartEvent(IDifficultyBeatmap difficultyBeatmap)
        {
            var songDidStartEvent = SongDidStartEvent;
            songDidStartEvent?.Invoke(difficultyBeatmap);
        }
        
      
    }
}
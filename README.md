# LastFm scrobbler for Beat Saber
This plugin allows you to submit tracks you play in game to [last.fm](https://www.last.fm)

## Dependencies
* BSIPA 4.1.3+
* SiraUtil 2.4.0+
* BeatSaberMarkupLanguage 1.4.5+

You can install all dependencies via ModAssistant.

## Installation
Go to Releases tab on the right, download zip and extract it into your game folder 
(`Steam/steamapps/common/Beat Saber`), this will put plugin dll into `Plugins` folder.

## Configuration
#### Account
First of all, you need to authorize the plugin to scrobble your plays. To do that, open plugin 
configuration in game, and click `Settings` button. On the side panel click `Auth`, it will open 
new tab in your browser. Go to the browser (do not quit the game!) and allow access to the 
plugin. Finally, return to the game and click `Confirm` button. 
#### Other settings
* **Enable**: enable/disable scrobbling
* **Enable Now Playing**: enable/disable updating "Now playing" in your profile.
* **Scrobble percentage**: change when track data should be submitted. With default 
  value of 50%, when you play more than half of the beatmap, new scrobble will be sent to last.fm.
  
## Limitations
All beatmaps with duration less than 30 seconds won't be scrobbled. Also, songs without author
won't be submitted either.  
Please note that all information about track is obtained from map info file (`info.dat`)

## Acknowledgements
UI code was largely inspired by [Auros' DiSounds plugin](https://github.com/Auros/DiSounds) licensed under [GPL-3.0](https://github.com/Auros/DiSounds/blob/main/LICENSE)
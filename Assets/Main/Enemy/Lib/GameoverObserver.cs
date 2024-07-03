using System;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverObserver : MonoBehaviour {
  public string GameoverSceneName;
  public bool IsGameover { get; private set; } = false;
  public PauseMenu PauseMenu;

  public void GameOver(bool victory){
    if (IsGameover) return;
    IsGameover = true;

    if (GameoverSceneName is not null && GameoverSceneName.Length > 0){
      SceneManager.LoadScene(GameoverSceneName);
    } else if (PauseMenu != null) {
      PauseMenu.Pause();
      PauseMenu.Unallow(PauseMenuOption.Resume);
    }
    var bgmManager = GameObject.Find(GameObjectNames.BackgroundMusicManager)
      .GetComponent<AudioClipManager>();
    bgmManager.PlayAudio(MusicAssetNames.GameOver, false);
    if (victory){
      bgmManager.AudioSource.time = 3;
      PauseMenu.SetText("VICTORY");
    } else {
      PauseMenu.SetText("DEFEAT");
      bgmManager.AudioSource.time = 0;
      bgmManager.AudioSource.SetScheduledEndTime(AudioSettings.dspTime + 3);
    }
    bgmManager.AudioSource.Play();
  }
}
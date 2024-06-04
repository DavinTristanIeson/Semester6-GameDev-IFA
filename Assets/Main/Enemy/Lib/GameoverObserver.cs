using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverObserver : MonoBehaviour {
  public string GameoverSceneName;
  bool gameoverFlag = false;
  public PauseMenu PauseMenu;

  public void GameOver(){
    if (gameoverFlag) return;
    gameoverFlag = true;
    if (GameoverSceneName is not null && GameoverSceneName.Length > 0){
      SceneManager.LoadScene(GameoverSceneName);
    } else if (PauseMenu != null) {
      PauseMenu.Pause();
      PauseMenu.Unallow(PauseMenuOption.Resume);
    }
  }
}
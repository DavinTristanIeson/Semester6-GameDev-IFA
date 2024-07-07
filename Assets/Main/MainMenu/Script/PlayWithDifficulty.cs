using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayWithDifficulty : MonoBehaviour {
  public DifficultyMode difficulty;
  public void SetDifficulty(){
    SessionStorage.GetInstance().Set(SessionStorage.Keys.GameDifficulty, difficulty);
    SceneManager.LoadScene("CutsceneBefore");
  }
}

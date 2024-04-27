using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayWithDifficulty : MonoBehaviour {
  public DifficultyMode difficulty;
  public void SetDifficulty(){
    SceneManager.LoadScene("Playground");
    SessionStorage.GetInstance().Set(SessionStorage.Keys.GameDifficulty, difficulty);
  }
}

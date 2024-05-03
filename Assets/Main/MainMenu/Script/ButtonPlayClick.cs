using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPlay_Click : MonoBehaviour {

  public void OpenScene(string sceneName){
    SceneManager.LoadSceneAsync (sceneName);
  }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PauseMenuOption {
  Resume,
  Home,
  Restart
}

public class PauseMenu : MonoBehaviour {
  [SerializeField] GameObject pauseMenu;
  Button restartButton;
  Button homeButton;
  Button resumeButton;

  public void OnEnable(){
    var layout = transform.Find("Layout");
    restartButton = layout.transform.Find("Restart").GetComponent<Button>();
    homeButton = layout.transform.Find("Home").GetComponent<Button>();
    resumeButton = layout.transform.Find("Resume").GetComponent<Button>();
    restartButton.interactable = true;
    homeButton.interactable = true;
    resumeButton.interactable = true;
  }
  public void Pause(){
    pauseMenu.SetActive(true);
    Time.timeScale=0;
  }

  public void Home(){
    SceneManager.LoadScene("MenuScreen");
    Time.timeScale=1;
  }

  public void Resume(){
    pauseMenu.SetActive(false);
    Time.timeScale=1;
  }

  public void Restart(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    Time.timeScale=1;
  }

  public void Unallow(PauseMenuOption option){
    switch (option){
      case PauseMenuOption.Resume:
        resumeButton.interactable = false;
        break;
      case PauseMenuOption.Home:
        homeButton.interactable = false;
        break;
      case PauseMenuOption.Restart:
        restartButton.interactable = false;
        break;
      default:
        break;
    }
  }
}

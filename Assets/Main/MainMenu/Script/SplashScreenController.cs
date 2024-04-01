using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public string mainMenuSceneName; 
    public float splashDuration = 3f; 

    void Start()
    {
        Invoke("LoadMainMenu", splashDuration); 
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPlay_Click : MonoBehaviour
{

    public void OpenScene(string sceneName){
        SceneManager.LoadSceneAsync (sceneName);
    }
}
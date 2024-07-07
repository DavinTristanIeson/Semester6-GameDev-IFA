using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
class CutsceneToGame : MonoBehaviour {
  VideoPlayer videoPlayer;

  void OnEnable(){
    videoPlayer = GetComponent<VideoPlayer>();
  }

  public void EndCutscene(){
    videoPlayer.Stop();
    SceneManager.LoadScene("Playground");
  }

  void Update(){
    if (videoPlayer.frame > 0 && (videoPlayer.isPlaying == false)){
      EndCutscene();
    }
  }
}
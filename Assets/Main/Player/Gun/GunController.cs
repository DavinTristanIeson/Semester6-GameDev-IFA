using UnityEngine;

[RequireComponent(typeof(GameObjectPoolManager))]
public class GunController : MonoBehaviour {
  private GameObjectPoolManager projectileManager;
  private AudioClipManager sfxManager;

  void OnEnable(){
    projectileManager = GetComponent<GameObjectPoolManager>();
    sfxManager = GetComponentInChildren<AudioClipManager>();
  }

  void TrackCursor(){
    // https://stackoverflow.com/questions/46998241/getting-mouse-position-in-unity
    Vector3 mousePosition = Input.mousePosition;
    // https://stackoverflow.com/questions/45896852/unity-look-at-cursor-in-2d
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
      new Vector3 (mousePosition.x, mousePosition.y, transform.position.z)
    );
    Vector3 direction = worldPosition - transform.position;
    // https://gamedevbeginner.com/make-an-object-follow-the-mouse-in-unity-in-2d/
    float angle = Vector2.SignedAngle(Vector2.right, direction);
    transform.eulerAngles = new Vector3(0, 0, angle);

    Vector3 parentPos = transform.parent.position;
    float xOffset = angle / 180f * -0.2f;
    float zAxis = Constants.ZAxis.Gun * (angle > 30 ? -1 : 1);
    transform.position = new Vector3(parentPos.x + xOffset, parentPos.y, zAxis);
  }

  void Update(){
    if (projectileManager.Try()){
      var projectile = projectileManager.Get();
      var rb = projectile.GetComponent<Rigidbody2D>();
      rb.position = new Vector3(transform.position.x, transform.position.y, Constants.ZAxis.Bullet);
      rb.rotation = transform.rotation.eulerAngles.z;
    }
    if (Time.timeScale == 0 && sfxManager.AudioSource.volume != 0.0f){
      sfxManager.AudioSource.Pause();
    } else if (Time.timeScale != 0 && sfxManager.AudioSource.volume == 0.0f) {
      sfxManager.AudioSource.UnPause();
    }
  }
}

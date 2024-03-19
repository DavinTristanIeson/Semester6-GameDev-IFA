using System;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (HealthManager))]
public class PlayerController : MonoBehaviour {
  // Serialized
  public float defaultSpeed = 6f;

  // Focus
  private GameObject hitboxSprite;
  private bool isFocus = false;
  [NonSerialized]
  public bool NeedsReset = false;

  // Movement
  private Vector2 velocity;
  private Vector2 originalPosition;
  private Rigidbody2D rigidBody;

  // Health
  private HealthManager health;

  void OnEnable(){
    rigidBody = GetComponent<Rigidbody2D>();
    health = GetComponent<HealthManager>();
    health.Reset();
    rigidBody.freezeRotation = true;
    originalPosition = new Vector2(rigidBody.position.x, rigidBody.position.y);
    hitboxSprite = transform.Find("Render")?.Find("Hurtbox")?.gameObject;
    SetHurtboxVisibility(false);
  }

  void SetHurtboxVisibility(bool visible){
    if (hitboxSprite is null) return;
    Renderer[] renderers = hitboxSprite.GetComponentsInChildren<Renderer>();
    foreach (Renderer r in renderers){
      r.enabled = visible;
    }
  }

  void Update(){
    // Use GetAxisRaw rather than GetAxis so that there's no smoothing; we want our movement to be snappy.
    float xOffset = Input.GetAxisRaw("Horizontal");
    float yOffset = Input.GetAxisRaw("Vertical");
    float speed = defaultSpeed;
    // https://stackoverflow.com/questions/67579229/why-is-unity-not-recognising-my-left-shift-key
    // Use GetKey rather than GetKeyDown since that runs on every frame
    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
      speed /= 3;
      isFocus = true;
      SetHurtboxVisibility(true);
    } else if (isFocus){
      isFocus = false;
      SetHurtboxVisibility(false);
    }
    if (
      Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
      Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)
    ){
      speed *= 2;
    }

    velocity = new Vector2(xOffset, yOffset).normalized * speed;
    // Debug.Log($"{velocity.x} {velocity.y} {rigidBody.position.x} {rigidBody.position.y} {speed} {DefaultSpeed}");
  }
  void FixedUpdate(){
    if (NeedsReset){
      rigidBody.MovePosition(originalPosition);
      NeedsReset = false;
      return;
    }

    // https://forum.unity.com/threads/keeping-the-player-within-the-boundaries-of-the-camera.245693/
    var dist = (transform.position - Camera.main.transform.position).z;
    var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).x;
    var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1,0,dist)).x;
    var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).y;
    var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,1,dist)).y;
    Vector3 newPos = rigidBody.position + velocity * Time.fixedDeltaTime;
    rigidBody.MovePosition(new Vector2(
      Mathf.Clamp(newPos.x, leftBorder, rightBorder),
      Mathf.Clamp(newPos.y, topBorder, bottomBorder)
    ));
  }
}

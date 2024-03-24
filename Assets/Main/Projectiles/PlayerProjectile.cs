using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerProjectile : MonoBehaviour {
  public float Speed = 20f;
  Rigidbody2D rb;
  void Start(){
    rb = GetComponent<Rigidbody2D>();
  }

  void FixedUpdate() {
    Vector2 direction = Quaternion.Euler(0, 0, rb.rotation) * Vector2.right;
    Vector2 velocity = direction * Speed;
    rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
  }

  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.Enemy){
      gameObject.SetActive(false);
    }
  }
}
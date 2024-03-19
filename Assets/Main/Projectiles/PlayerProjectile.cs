using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerProjectile : BaseProjectile {
  protected override void Initialize(){
    rigidBody = GetComponent<Rigidbody2D>();
    Speed = 20f;
  }

  void FixedUpdate() {
    Vector2 direction = Quaternion.Euler(0, 0, rigidBody.rotation) * Vector2.right;
    Vector2 velocity = direction * Speed;
    rigidBody.MovePosition(rigidBody.position + (velocity * Time.fixedDeltaTime));
  }

  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.Enemy){
      Deactivate();
    }
  }
}
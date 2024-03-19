using UnityEngine;

public class EnemyProjectile : BaseProjectile {
  protected override void Initialize(){
    Speed = 5f;
  }

  void FixedUpdate() {
    if (Behavior is ScriptableBehavior<BaseProjectile> behavior){
      behavior.Execute(this);
    } else if (Action is ScriptableAction<BaseProjectile> action) {
      action(this);
    } else {
      Vector2 direction = Quaternion.Euler(0, 0, rigidBody.rotation) * Vector2.right;
      Vector2 velocity = direction * Speed;
      rigidBody.MovePosition(rigidBody.position + (velocity * Time.fixedDeltaTime));
    }
  }

  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.Player && collider.gameObject.tag == Constants.Tags.Hurtbox){
      Deactivate();
    }
  }
}
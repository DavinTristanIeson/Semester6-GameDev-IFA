using UnityEngine;

[RequireComponent(typeof(BehaviorManager))]
public class EnemyProjectile : BaseProjectile {
  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.Player && collider.gameObject.tag == Constants.Tags.Hurtbox){
      Deactivate();
    }
  }
}
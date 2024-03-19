using UnityEngine;

public class PlayerHurtbox : MonoBehaviour {
  HealthManager health;
  PlayerController controller;

  void OnEnable(){
    health = GetComponentInParent<HealthManager>();
    controller = GetComponentInParent<PlayerController>();
  }
  
  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.EnemyProjectiles){
      if (health.Damage(1)){
        Debug.Log($"Player HP: {health.Health}");
        controller.NeedsReset = true;
      }
    }
  }
}
using System;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour {
  HealthManager health;
  PlayerController controller;
  public HealthBar healthBar;
  public HealthBar RecoveryBar;
  GameObject boss;

  private float gauge = 0.0f;
  void OnEnable(){
    boss = GameObject.FindWithTag(Constants.Tags.Boss);
    health = GetComponentInParent<HealthManager>();
    controller = GetComponentInParent<PlayerController>();
    healthBar.SetMaxHealth(health.OriginalHealth);
    RecoveryBar.SetMaxHealth(1000f);
    RecoveryBar.SetHealth(0f);
  }

  void Update(){
    Vector2 position = transform.position;
    var distance = (position - boss.GetComponent<Rigidbody2D>().position).magnitude;

    gauge += 2f / distance;
    if (gauge >= RecoveryBar.slider.maxValue){
      health.Health = Math.Clamp(health.Health + 1, 0, health.OriginalHealth);
      healthBar.SetHealth(health.Health);
      gauge = 0f;
    }
    RecoveryBar.SetHealth(gauge);
  }
  
  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.EnemyProjectiles){
      if (health.Damage(1)){
        Debug.Log($"Player HP: {health.Health}");
        controller.NeedsReset = true;
        healthBar.SetHealth(health.Health);
      }
    }
  }
}
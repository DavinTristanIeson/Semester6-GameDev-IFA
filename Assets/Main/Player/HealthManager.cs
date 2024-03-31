using System;
using UnityEngine;

/// <summary>
/// Reusable script for managing health
/// </summary>
class HealthManager : MonoBehaviour {
  protected int health;

  [SerializeField]
  protected int originalHealth;

  [SerializeField]
  protected float invincibilitySeconds = 1f;
  protected CooldownTimer cooldown;

  public int Health {
    get => health;
    set {
      health = Math.Max(0, value);
    }
  }

  public int OriginalHealth {
    get => originalHealth;
  }

  public int HealthPercentage {
    get => health / originalHealth;
  }

  public bool IsDead {
    get => health <= 0;
  }

  void Initialize(){
    if (cooldown == null){
      cooldown = new CooldownTimer(invincibilitySeconds);
    }
  }

  void Start(){
    Initialize();
  }

  void OnEnable(){
    Initialize();
  }

  public void Reset(){
    health = originalHealth;
  }

  public bool Damage(int damage){
    if (cooldown.Try() && health != 0){
      health = Math.Max(0, health - damage);
      return true;
    }
    return false;
  }
}
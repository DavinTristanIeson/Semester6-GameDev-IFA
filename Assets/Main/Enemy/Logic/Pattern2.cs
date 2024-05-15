using System;
using UnityEditor;
using UnityEngine;
public class Boss1Pattern2_SineWave : BossPattern<Boss1> {
  Boss1 boss;
  private GameObjectPool pool;
  private GameObject blueprint;
  private CooldownTimer cooldown;
  private int rotation = 0;

  void Behavior(GameObject caller, float direction){
    var projectile = caller.GetComponent<BaseProjectile>();
    Vector2 nowPos = projectile.rb.position;
    Vector2 velocity = projectile.Direction * (5f * direction);
    velocity.y *= Mathf.Sin(nowPos.x) * (Difficulty == DifficultyMode.Challenge ? 2f : 1f);
    projectile.rb.MovePosition(nowPos + (velocity * Time.fixedDeltaTime));
  }
  void BehaviorLeft(GameObject caller){
    Behavior(caller, -1f);
  }
  void BehaviorRight(GameObject caller){
    Behavior(caller, 1f);
  }
  public Boss1Pattern2_SineWave(Boss1 boss){
    this.boss = boss;
    blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern2 SineWave")
    };
    cooldown = new CooldownTimer(0.05f);
  }

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = pool.Get();
      go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);

      var rb = go.GetComponent<Rigidbody2D>();
      var playerX = caller.Player.GetComponent<Rigidbody2D>().position.x;
      var bossX = caller.GetComponent<Rigidbody2D>().position.x;
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Custom(playerX < bossX ? BehaviorLeft : BehaviorRight);
      Vector2 yOffset = Vector2.up * ((float) Math.Sin(Time.time) * 3f);
      rb.position = caller.rb.position + yOffset;
      float rotationFactor = Difficulty switch {
        DifficultyMode.Casual => 3,
        DifficultyMode.Normal => 2,
        DifficultyMode.Challenge => 1,
        _ => 2,
      };
      rb.rotation = Math.Abs(rotation - 180) - 90;
    }
    rotation = (rotation + 1) % 360;
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 0.4f,
      DifficultyMode.Normal => 0.25f,
      DifficultyMode.Challenge => 0.15f,
      _ => cooldown.WaitTime
    };
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}
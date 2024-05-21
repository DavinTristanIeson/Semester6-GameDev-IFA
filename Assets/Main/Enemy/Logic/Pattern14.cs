using Unity.VisualScripting;
using UnityEngine;
#nullable enable

class Boss1Pattern14_Rain : BossPattern<Boss1>{
  GameObjectPool rainPool;
  GameObjectPool showerPool;
  GameObjectPool fountainPool;
  Boss1 boss;
  CooldownTimer rainCooldown;
  CooldownTimer showerCooldown;
  CooldownTimer fountainCooldown;
  Vector2 top;
  Vector2 bottom;

  public override void Execute(Boss1 caller){
    if (showerCooldown.Try()){
      var projectiles = rainPool.GetMany((int) Difficulty + 1);
      foreach (var projectile in projectiles){
        projectile.GetComponent<BaseProjectile>().InstantlyDespawnWhenInvisible = true;
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.position = top + new Vector2(Random.Range(-1f, 1f), 0);
        rb.rotation = Calculate.Vector.AngleTowards(rb.position, bottom + new Vector2(Random.Range(-1f, 1f), 0));
        projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(7f);
      }
    }
    if (rainCooldown.Try()){
      var projectiles = rainPool.GetMany((int) Difficulty + 1);
      var boundary = BoundaryInformation.GetInstance();
      foreach (var projectile in projectiles){
        projectile.GetComponent<BaseProjectile>().InstantlyDespawnWhenInvisible = true;
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.position = new Vector2(boundary.GetRandomX(), boundary.y1);
        rb.rotation = -90f;
        projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(4f);
      }
    }
    if (fountainCooldown.Try()){
      var projectiles = rainPool.GetMany(3);
      foreach (var projectile in projectiles){
        projectile.GetComponent<BaseProjectile>().InstantlyDespawnWhenInvisible = true;
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.position = boss.eyesPosition;
        rb.rotation = Random.Range(90f, 110f);
        projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(7f);
      }
    }
    var playerRb = boss.Player.GetComponent<Rigidbody2D>();
    var topShift = new Vector2(playerRb.position.x, top.y) - top;
    top = top + Vector2.ClampMagnitude(topShift, 0.05f);
    var bottomShift = new Vector2(playerRb.position.x, bottom.y) - bottom;
    bottom = bottom + Vector2.ClampMagnitude(bottomShift, 0.2f);
  }

  public override void Destroy(Boss1 caller){
    rainPool.Destroy();
    fountainPool.Destroy();
    showerPool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    rainPool.Revoke();
    fountainPool.Revoke();
    showerPool.Revoke();
  }

  public Boss1Pattern14_Rain(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Tear);
    showerPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern14 Shower"),
    };
    rainPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern14 Rain"),
    };
    fountainPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern15 Fountain"),
    };
    showerCooldown = new CooldownTimer(0.25f);
    rainCooldown = new CooldownTimer(0.1f);
    fountainCooldown = new CooldownTimer(0.1f);
    var boundary = BoundaryInformation.GetInstance();
    top = new Vector2(boss.eyesPosition.x, boundary.y1);
    bottom = new Vector2(boss.eyesPosition.x, boundary.y0);
  }
}
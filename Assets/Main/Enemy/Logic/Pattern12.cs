using UnityEngine;
#nullable enable

class Boss1Pattern12_Meteors : BossPattern<Boss1>{
  GameObjectPool debrisPool;
  GameObjectPool meteorPool;
  Boss1 boss;
  CooldownTimer cooldown;

  void SpawnRing(GameObject go){
    int projectileCount = 10 * ((int) Difficulty + 1);
    var startAngle = Random.Range(0f, 360f);
    var projectiles = debrisPool.GetMany(projectileCount);
    var parentRb = go.GetComponent<Rigidbody2D>();
    for (int i = 0; i < projectileCount; i++) {
      GameObject projectile = projectiles[i];
      projectile.transform.localScale = Vector3.one;
      projectile.transform.localScale = new Vector3(1f, 1f, 1f);
      
      var rb = projectile.GetComponent<Rigidbody2D>();
      rb.position = parentRb.position;
      rb.rotation = startAngle + i * (360f / projectileCount);

      projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Acceleration(0f, 0.01f, 0f, 2f);
      go.transform.localScale = Vector3.one;
    }
  }

  public override void Execute(Boss1 caller){
    var boundary = BoundaryInformation.GetInstance();
    if (cooldown.Try()){
      var go = meteorPool.Get();
      go.transform.localScale = Vector3.one * 10f;
      var x = boundary.GetRandomX();
      var targetY = boundary.GetRandomY(0, 0.7f);
      var destination = new Vector2(x, targetY);
      go.GetComponent<Rigidbody2D>().position = new Vector2(x, boundary.y1);
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
        .Then(new ProjectileBehavior.Merge(3)
          .With(new ProjectileBehavior.Propulsion(5f))
          .With(new ProjectileBehavior.Homing(destination))
          .With(new ProjectileBehavior.Custom((go) => {
            var distance = go.GetComponent<Rigidbody2D>().position - destination;
            if (distance.magnitude < 5f * Time.fixedDeltaTime){
              go.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Timing>()?.Next(go);
            }
          })
        ))
        .Then(new ProjectileBehavior.Spawner(SpawnRing) { Once = true })
        .Then(new ProjectileBehavior.Despawn());
    }
  }

  public override void Destroy(Boss1 caller){
    debrisPool.Destroy();
    meteorPool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    debrisPool.Revoke();
    meteorPool.Revoke();
  }

  public Boss1Pattern12_Meteors(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    debrisPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern12 Debris"),
    };
    meteorPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern12 Meteors"),
    };
    cooldown = new CooldownTimer(1f);
  }
}
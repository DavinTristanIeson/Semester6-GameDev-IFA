using Unity.VisualScripting;
using UnityEngine;
#nullable enable

class Boss1Pattern13_AirPunch : BossPattern<Boss1>{
  GameObjectPool blastPool;
  GameObjectPool pool;
  Boss1 boss;
  CooldownTimer cooldown;
  
  void CurvedMovement(GameObject go){
    if (go.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Acceleration>()!.Speed < 0f){
      go.GetComponent<Rigidbody2D>().rotation += 0.1f;
    }
  }

  void SpawnRing(GameObject go){
    int projectileCount = 6 + 4 * (int) Difficulty;
    var startAngle = Random.Range(0f, 360f);
    var projectiles = blastPool.GetMany(projectileCount);
    var parentRb = go.GetComponent<Rigidbody2D>();
    for (int i = 0; i < projectileCount; i++) {
      GameObject projectile = projectiles[i];
      projectile.transform.localScale = Vector3.one;
      
      var rb = projectile.GetComponent<Rigidbody2D>();
      rb.position = parentRb.position;
      rb.rotation = startAngle + i * (360f / projectileCount);

      projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Merge(2)
        .With(new ProjectileBehavior.Acceleration(6f, -0.05f, -4f))
        .With(new ProjectileBehavior.Custom(CurvedMovement));
    }
  }

  void ControlRotation(GameObject go, float value){
    var bm = go.GetComponent<BehaviorManager>();
    var homing = bm.GetBehaviorOfType<ProjectileBehavior.Homing>();
    if (homing is not null){
      homing.MaxRotation = value;
      if (homing.MaxRotation <= 0){
        bm.GetBehaviorOfType<ProjectileBehavior.Timing>()?.Next(go);
      }
    }
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 4f,
      DifficultyMode.Normal => 3f,
      DifficultyMode.Challenge => 2f,
      _ => 1.5f,
    };
  }

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var playerPos = boss.Player.GetComponent<Rigidbody2D>().position;
      var go = pool.Get();
      go.transform.localScale = Vector3.one * 5f;
      float xOffset = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(2f, 3f);
      float yOffset = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(2f, 3f);
      go.GetComponent<Rigidbody2D>().position = playerPos + new Vector2(xOffset, yOffset);
      go.GetComponent<BaseProjectile>().DeactivationDelay = 4f;
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
        .Then(new ProjectileBehavior.Merge(3)
          .With(new ProjectileBehavior.Acceleration(360f, -2f, 0f) {
            Accelerate = ControlRotation
          })
          .With(new ProjectileBehavior.Homing(boss.Player))
          .With(new ProjectileBehavior.Acceleration(0f, -0.01f, int.MinValue))
        )
        .Then(new ProjectileBehavior.Custom(), 0.5f)
        .Then(new ProjectileBehavior.Merge(2)
          .With(new ProjectileBehavior.Spawner(SpawnRing, 0.2f) { MaxSpawnCount = (int) Difficulty + 2 })
          .With(new ProjectileBehavior.Acceleration(5f, 0.2f))
        );
    }
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
    blastPool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
    blastPool.Revoke();
  }

  public Boss1Pattern13_AirPunch(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern13 Punch"),
    };
    blastPool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern13 Blast"),
    };
    cooldown = new CooldownTimer(1f);
  }
}
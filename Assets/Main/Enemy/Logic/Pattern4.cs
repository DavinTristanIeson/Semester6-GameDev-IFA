using UnityEngine;

class Boss1Pattern4_SpawnRing : BossPattern<Boss1> {
  GameObjectPool pool;
  CooldownTimer cooldown;
  private Boss1 boss;

  void SpawnChildProjectile(GameObject go){
    int projectileCount = 4 + (int) Difficulty * 2;
    var parentRb = go.GetComponent<Rigidbody2D>();
    var propulsion = go.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Propulsion>();
    GameObject[] projectiles = pool.GetMany(projectileCount);
    go.transform.localScale -= new Vector3(1f, 1f, 1f);
    propulsion.Speed += 0.5f;
    var startAngle = Random.Range(0f, 360f);
    for (int i = 0; i < projectileCount; i++) {
      GameObject projectile = projectiles[i];
      projectile.transform.localScale = new Vector3(1f, 1f, 1f);
      
      var rb = projectile.GetComponent<Rigidbody2D>();
      rb.position = parentRb.position;
      rb.rotation = startAngle + i * (360f / projectileCount);

      projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
        .Then(new ProjectileBehavior.Propulsion(5f), go.transform.localScale.x / 10f)
        .Then(new ProjectileBehavior.Custom(), 2.5f)
        .Then(new ProjectileBehavior.Propulsion(Difficulty == DifficultyMode.Challenge ? 7f : 5f)
        );
    }

    if (go.transform.localScale.x <= 1f){
      go.SetActive(false);
    }
  }
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = pool.Get();
      go.GetComponent<Rigidbody2D>().position = boss.mouthPosition;
      float size = 8f;
      go.transform.localScale = new Vector3(size, size, size);
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Merge(
        new ScriptableBehavior<GameObject>[] {
          new ProjectileBehavior.Homing(boss.Player) {
            MaxRotation = 0.5f,
          },
          new ProjectileBehavior.Propulsion(1f),
          new ProjectileBehavior.Spawner(SpawnChildProjectile, 1f),
        }
      );
    }    
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 2f,
      DifficultyMode.Normal => 2.5f,
      DifficultyMode.Challenge => 3f,
      _ => 3f,
    };
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern4_SpawnRing(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern4 SpawnRing")
    };
    cooldown = new CooldownTimer(3f);
  }
}
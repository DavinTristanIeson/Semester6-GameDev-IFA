using UnityEngine;
#nullable enable

class Boss1Pattern15_Orbit : BossPattern<Boss1>{
  GameObjectPool pool;
  Boss1 boss;
  CooldownTimer cooldown;
  float direction = 1f;
  Vector2 pivot = Vector2.zero;

  Vector2 GetPivot(){
    float sumX = 0;
    float sumY = 0;
    int count = 0;
    foreach (var go in pool.GetActiveObjects()){
      var rb = go.GetComponent<Rigidbody2D>();
      sumX += rb.position.x;
      sumY += rb.position.y;
      count++;
    }
    Vector2 mean = new Vector2(sumX / count, sumY / count);
    var position = boss.Player.GetComponent<Rigidbody2D>().position;
    return (mean + position) / 2;
  }

  void RotateAroundPivot(GameObject go){
    var rb = go.GetComponent<Rigidbody2D>();
    rb.position = Calculate.Vector.RotateAround(rb.position, pivot, direction);
    rb.rotation += Mathf.Clamp(rb.rotation - Calculate.Vector.AngleTowards(rb.position, pivot), -0.1f, 0.1f);
  }

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      direction *= -1;
      var projectiles = pool.GetMany(36 + 12 * (int) Difficulty);
      foreach (var projectile in projectiles){
        var rb = projectile.GetComponent<Rigidbody2D>();
        projectile.transform.localScale = Vector3.one * 2f;
        rb.position = boss.eyesPosition;
        rb.rotation = Random.Range(75f, 255f);
        projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
        .Then(new ProjectileBehavior.Merge(2)
          .With(new ProjectileBehavior.Custom(RotateAroundPivot))
          .With(new ProjectileBehavior.Acceleration(Random.Range(3f, 8f), Random.Range(-0.01f, -0.1f), Random.Range(2f, 4f))),
          3f
        )
          .Then(new ProjectileBehavior.Homing(boss.Player) { Once = true })
          .Then(new ProjectileBehavior.Propulsion(3f));
      }
    }
    pivot = GetPivot();
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern15_Orbit(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern15 Spiral"),
    };
    cooldown = new CooldownTimer(4f);
  }
}
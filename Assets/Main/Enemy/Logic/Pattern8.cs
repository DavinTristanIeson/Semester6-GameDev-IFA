using UnityEngine;

class Boss1Pattern8_BounceCrush : BossPattern<Boss1>{
  GameObjectPool pool;
  GameObjectPool quakePool;
  Boss1 boss;

  public Vector2 GetBoundaryPosition(GameObject go){
    var rb = go.GetComponent<Rigidbody2D>();
    var boundary = BoundaryInformation.GetInstance();
    float targetY = boundary.GetRandomY(0.15f, 0.85f);
    // At left boundary
    if (rb.position.x <= 0.0){
      return new Vector2(boundary.x1, targetY);
    } else {
      return new Vector2(boundary.x0, targetY);
    }
  }

  public bool IsAtBoundary(GameObject go){
    float x = go.GetComponent<Rigidbody2D>().position.x;
    var boundary = BoundaryInformation.GetInstance();
    return boundary.x0 == x || boundary.x1 == x;
  }

  public void SpawnQuake(GameObject go){
    var center = go.GetComponent<Rigidbody2D>().position;
    int count = 8 * ((int) Difficulty + 1);
    var projectiles = quakePool.GetMany(count);
    var startAngle = Random.Range(0f, 360f);
    for (int i = 0; i < count; i++){
      float angle = startAngle + i * (360f / count);
      var rb = projectiles[i].GetComponent<Rigidbody2D>();
      rb.rotation = angle;
      rb.position = center;
      projectiles[i].GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(4f);
    }
    go.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Timing>()?.Next(go);
  }

  public override void Start(Boss1 caller){
    int count = 4 + 2 * (int) Difficulty;
    var projectiles = pool.GetMany(count);
    var boundary = BoundaryInformation.GetInstance();
    for (int i = 0; i < count; i++){
      projectiles[i].transform.localScale = Vector3.one * 10;
      var rb = projectiles[i].GetComponent<Rigidbody2D>();
      Vector2 position = new Vector2(i % 2 == 0 ? boundary.x0 : boundary.x1, boundary.GetRandomY(0.15f, 0.85f));
      rb.position = position;

      var bm = projectiles[i].GetComponent<BehaviorManager>();
      var timing = new ProjectileBehavior.Timing(4) {
        IsRepeating = true
      }.Then(new ProjectileBehavior.Homing(GetBoundaryPosition) {
          Once = true,
        })
        .Then(new ProjectileBehavior.Propulsion(4f + 0.5f * i) {
          Cordoned = true
        }, 0.5f)
        .Then(new ProjectileBehavior.Propulsion(4f + 0.5f * i) {
          Cordoned = true
        }, condition: IsAtBoundary)
        .Then(new ProjectileBehavior.Custom(SpawnQuake));
      bm.Behavior = timing;
    }
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
    quakePool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
    quakePool.Revoke();
  }

  public Boss1Pattern8_BounceCrush(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern8 BounceCrush"),
    };
    quakePool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern8 Quake"),
    };
  }
}
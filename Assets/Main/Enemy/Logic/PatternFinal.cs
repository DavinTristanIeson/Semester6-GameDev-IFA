using UnityEngine;
#nullable enable

class Boss1PatternFinal_Spiral : BossPattern<Boss1> {
  float rotation = 0f;
  CooldownTimer cooldown;
  GameObjectPool pool;
  public Boss1PatternFinal_Spiral(Boss1 boss){
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 PatternFinal Spiral"),
    };
    cooldown = new CooldownTimer(0.1f);
  }
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var projectiles = pool.GetMany((int) Difficulty * 4);
      for (int i = 0; i < 4; i++){
        var rb = projectiles[i].GetComponent<Rigidbody2D>();
        rb.position = caller.eyesPosition;
        rb.rotation = rotation + 90f * i;
        var behavior = new ProjectileBehavior.Propulsion(5f);
        projectiles[i].GetComponent<BehaviorManager>().Behavior = behavior;
        if (Difficulty == DifficultyMode.Challenge){
          var otherRb = projectiles[4 + i].GetComponent<Rigidbody2D>();
          otherRb.position = caller.eyesPosition;
          otherRb.rotation = rb.rotation * -1;
          projectiles[4 + i].GetComponent<BehaviorManager>().Behavior = behavior;
        }
      }
    }
    rotation += 1f;
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}

class Boss1PatternFinal_Fists : BossPattern<Boss1> {
  CooldownTimer cooldown;
  GameObjectPool pool;
  bool aimed = false;
  public Boss1PatternFinal_Fists(Boss1 boss){
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 PatternFinal Fists"),
    };
    cooldown = new CooldownTimer(4f);
  }
  public override void Start(Boss1 caller){
    if (Difficulty == DifficultyMode.Challenge){
      cooldown.WaitTime = 2.5f;
    }
  }
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      aimed = !aimed;
      var projectiles = pool.GetMany(6);
      var playerPosition = caller.Player.GetComponent<Rigidbody2D>().position;
      var bossPosition = caller.GetComponent<Rigidbody2D>().position;
      var playerAngle = Calculate.Vector.AngleTowards(bossPosition, playerPosition) + (aimed ? 360f/12f : 0f);
      for (int i = 0; i < 6; i++){
        projectiles[i].transform.localScale = Vector3.one * (((int) Difficulty + 1) * 5f);
        var angleOffset = i * (360f / 6f);
        var rb = projectiles[i].GetComponent<Rigidbody2D>();
        rb.position = bossPosition;
        rb.rotation = playerAngle + angleOffset;
        projectiles[i].GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Acceleration(0f, 0.01f, 0f, 10f);
      }
    }
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}

class Boss1PatternFinal_CurvedYawns : BossPattern<Boss1> {
  CooldownTimer cooldown;
  GameObjectPool pool;
  int direction = -1;
  public Boss1PatternFinal_CurvedYawns(Boss1 boss){
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 PatternFinal CurvedYawns"),
    };
    cooldown = new CooldownTimer(3f);
  }
  void CurvedMovement(GameObject go){
    go.GetComponent<Rigidbody2D>().rotation += 0.2f * direction * (int) Difficulty;
  }
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      direction *= -1;
      var projectiles = pool.GetMany(8 + (4 * (int) Difficulty));
      var bossPosition = caller.GetComponent<Rigidbody2D>().position;
      var startAngle = Random.Range(0f, 360f);
      for (int i = 0; i < projectiles.Length; i++){
        projectiles[i].transform.localScale = Vector3.one * 8f;
        var angleOffset = i * (360f / projectiles.Length);
        var rb = projectiles[i].GetComponent<Rigidbody2D>();
        rb.position = bossPosition;
        rb.rotation = startAngle + angleOffset;
        projectiles[i].GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Merge(2)
          .With(new ProjectileBehavior.Acceleration(0f, 0.02f, 0f, 7f))
          .With(new ProjectileBehavior.Custom(CurvedMovement));
      }
    }
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}

class Boss1PatternFinal_Galaxy : BossPattern<Boss1> {
  Boss1 boss;
  CooldownTimer cooldown;
  GameObjectPool pool;
  int direction = -1;
  public Boss1PatternFinal_Galaxy(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 PatternFinal Galaxy"),
    };
    cooldown = new CooldownTimer(5f);
  }
  void RotateAroundPivot(GameObject go){
    var rb = go.GetComponent<Rigidbody2D>();
    var pivot = boss.GetComponent<Rigidbody2D>();
    rb.position = Calculate.Vector.RotateAround(rb.position, pivot.position, direction * (3f / (rb.position - pivot.position).magnitude));
    rb.rotation = 180f + Calculate.Vector.AngleTowards(rb.position, pivot.position);
  }
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      if (Difficulty == DifficultyMode.Challenge){
        direction *= -1;
      }
      var projectiles = pool.GetMany(30 + 40 * (int) Difficulty);
      foreach (var projectile in projectiles){
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.position = boss.GetComponent<Rigidbody2D>().position;
        rb.rotation = Random.Range(0f, 360f);
        projectile.transform.localScale = Vector3.one * 2.0f;
        projectile.GetComponent<BaseProjectile>().DeactivationDelay = 4f;
        projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
          .Then(new ProjectileBehavior.Propulsion(Random.Range(0.1f, 10f)), 1f)
          .Then(new ProjectileBehavior.Custom(RotateAroundPivot), 3f)
          .Then(new ProjectileBehavior.Merge(2)
            .With(new ProjectileBehavior.Custom(RotateAroundPivot))
            .With(new ProjectileBehavior.Propulsion(2f)));
      }
    }
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}

class Boss1PatternFinal_Paranoia : BossPattern<Boss1> {
  Boss1 boss;
  CooldownTimer cooldown;
  GameObjectPool pool;
  public Boss1PatternFinal_Paranoia(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 PatternFinal Paranoia"),
    };
    cooldown = new CooldownTimer(0.02f);
  }
  int rotation = 0;
  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = pool.Get();
      var rb = go.GetComponent<Rigidbody2D>();
      var playerRb = boss.Player.GetComponent<Rigidbody2D>();
      rb.position = playerRb.position + (Calculate.Vector.WithAngle(rotation) * 1f);
      rb.rotation = 180 + Calculate.Vector.AngleTowards(rb.position, playerRb.position);
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(2 - (int) Difficulty);
      rotation++;
    }
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }
}

class Boss1PatternFinal : BossPattern<Boss1> {
  Boss1 boss;
  BossPattern<Boss1>[] patterns;
  float[] timing;
  float startTime = 0.0f;

  public override void Start(Boss1 caller){
    foreach (var pattern in patterns){
      pattern.Difficulty = Difficulty;
      pattern.Start(caller);
    }
    startTime = Time.time;
  }

  public override void Execute(Boss1 caller){
    for (int i = 0; i < patterns.Length; i++){
      if (Time.time >= startTime + timing[i]){
        patterns[i].Execute(caller);
      }
    }
  }

  public override void Destroy(Boss1 caller){
    foreach (var pattern in patterns){
      pattern.Destroy(caller);
    }
  }

  public override void Deactivate(Boss1 caller){
    foreach (var pattern in patterns){
      pattern.Deactivate(caller);
    }
  }

  public Boss1PatternFinal(Boss1 boss){
    this.boss = boss;
    patterns = new BossPattern<Boss1>[] {
      new Boss1PatternFinal_Spiral(boss),
      new Boss1PatternFinal_Fists(boss),
      new Boss1PatternFinal_CurvedYawns(boss),
      new Boss1PatternFinal_Galaxy(boss),
      new Boss1PatternFinal_Paranoia(boss)
    };
    timing = new [] {
      0f,
      15f,
      30f,
      40f,
      50f
    };
  }
}
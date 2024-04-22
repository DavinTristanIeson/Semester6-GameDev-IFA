using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

class Boss1Pattern6_Geyser : BossPattern<Boss1>{
  GameObjectPool tearsPool;
  GameObjectPool geyserPool;
  CooldownTimer tearsCooldown;
  CooldownTimer geyserCooldown;
  Boss1 boss;
  float rotation = 0;

  void BounceTears(GameObject self, Collider2D collider){
    if (!Trigger2DListener.IsEnemyProjectileCollider(collider)) return;
    var bounceBack = collider.Distance(self.GetComponent<Collider2D>()).normal;
    var angle = Calculate.Vector.AngleTowards(Vector2.right, bounceBack);
    collider.gameObject.GetComponent<Rigidbody2D>().rotation = angle;
  }
  void AccelerateTears(GameObject self, Collider2D collider){
    if (!Trigger2DListener.IsEnemyProjectileCollider(collider)) return;
    var accel = collider.gameObject
      .GetComponent<BehaviorManager>()
      .GetBehaviorOfType<ProjectileBehavior.Acceleration>();
    accel.Speed = accel.Max;
  }

  void Enlarge(GameObject go, float value){
    go.transform.localScale = new Vector3(value, value, 0.0f);
  }

  void ResetGeyserProjectile(GameObject go){
    var target = boss.Player.GetComponent<Rigidbody2D>().position;
    var angle = Calculate.Vector.AngleTowards(boss.rb.position, target);
    go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Merge(2)
      .Set(0, new ProjectileBehavior.Propulsion(4f, angle))
      .Set(1, new ProjectileBehavior.Acceleration(1f, 0.1f, 1f, Difficulty switch {
        DifficultyMode.Casual => 10f,
        DifficultyMode.Normal => 20f,
        DifficultyMode.Challenge => 15f,
        _ => 15f
      }) {
        Accelerate = Enlarge
      });
    var triggers = go.GetComponent<Trigger2DListener>();
    triggers.StayListener += AccelerateTears;
    if (Difficulty == DifficultyMode.Challenge){
      triggers.ExitListener += BounceTears;
    }
  }

  public override void Execute(Boss1 caller){
    if (tearsCooldown.Try()){
      int projectileCount = 1 << (int)Difficulty;
      var projectiles = tearsPool.GetMany(projectileCount);
      for (int i = 0; i < projectileCount; i++){
        var go = projectiles[i];
        var rb = go.GetComponent<Rigidbody2D>();
        rb.position = caller.eyesPosition;
        rb.rotation = rotation + (i * 360 / projectileCount);
        go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Acceleration(8f, -0.1f, 1f, 8f);
      }
    }
    if (geyserCooldown.Try()){
      var go = geyserPool.Get();
      var rb = go.GetComponent<Rigidbody2D>();
      rb.position = caller.mouthPosition;
    }
    rotation += 0.3f;
  }

  public override void Start(Boss1 caller){
    rotation = 0;
    tearsCooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 0.01f,
      DifficultyMode.Normal => 0.005f,
      DifficultyMode.Challenge => 0.005f,
      _ => 0.5f,
    };
    geyserCooldown.WaitTime = 2f;
  }

  public override void Destroy(Boss1 caller){
    tearsPool.Destroy();
    geyserPool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    tearsPool.Revoke();
    geyserPool.Revoke();
  }

  public Boss1Pattern6_Geyser(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Tear);
    var blueprint2 = boss.Projectiles.Get(ProjectileType.Effector);
    tearsPool = new GameObjectPool(blueprint, 300, 1000) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern6 Geyser > Tears"),
    };
    geyserPool = new GameObjectPool(blueprint2, 20, 50) {
      Transform = ResetGeyserProjectile,
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern6 Geyser > Geyser"),
    };
    tearsCooldown = new CooldownTimer(0.01f);
    geyserCooldown = new CooldownTimer(2f);
    rotation = 0;
  }
}
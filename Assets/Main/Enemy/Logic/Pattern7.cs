using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

class Boss1Pattern7_Punch : BossPattern<Boss1>{
  GameObjectPool pool;
  CooldownTimer cooldown;
  CooldownTimer homingCooldown;
  Boss1 boss;

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = ResetFistProjectile(pool.Get());
      var bm = go.GetComponent<BehaviorManager>();
      bm.Behavior = new ProjectileBehavior.Acceleration(2f, 0.1f, Difficulty switch {
        DifficultyMode.Casual => 7f,
        _ => 10f
      });
    }
    if (homingCooldown.Try()){
      var go = ResetFistProjectile(pool.Get());
      var bm = go.GetComponent<BehaviorManager>();
      bm.Behavior = new ProjectileBehavior.Timing(2)
        .Then(new ProjectileBehavior.Merge(2)
          .With(new ProjectileBehavior.Acceleration(0f, 0.025f, 4f))
          .With(new ProjectileBehavior.Homing(boss.Player) {
            MaxRotation = (int) Difficulty * 1.5f,
          }), 10f)
        .Then(new ProjectileBehavior.Propulsion(8f));
    }
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 2f,
      DifficultyMode.Normal => 1.5f,
      DifficultyMode.Challenge => 1f,
      _ => 1f,
    };
    homingCooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 4f,
      DifficultyMode.Normal => 3f,
      DifficultyMode.Challenge => 2f,
      _ => 2f,
    };
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  GameObject ResetFistProjectile(GameObject go){
    go.transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);
    var dist = (go.transform.position - Camera.main.transform.position).z;
    var rb = go.GetComponent<Rigidbody2D>();
    rb.position = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, Random.Range(0.25f, 0.75f), dist));
    rb.rotation = Calculate.Vector.AngleTowards(rb.position, boss.Player.GetComponent<Rigidbody2D>().position);
    return go;
  }

  public Boss1Pattern7_Punch(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern7 Punches"),
    };
    cooldown = new CooldownTimer(0.01f);
    homingCooldown = new CooldownTimer(2f);
  }
}
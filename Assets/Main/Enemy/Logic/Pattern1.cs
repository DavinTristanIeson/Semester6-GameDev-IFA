using UnityEditor;
using UnityEngine;
class Boss1Pattern1_BOWAP : BossPattern<Boss1> {
  Boss1 boss;
  float rotationStep = 1;
  int rotation = 0;
  private GameObjectPool pool;
  private CooldownTimer cooldown;
  public Boss1Pattern1_BOWAP(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Tear);
    pool = new GameObjectPool(blueprint, 300, 1000) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern1 BOWAP"),
    };
    cooldown = new CooldownTimer(0.001f);
  }
  public override void Execute(Boss1 boss){
    if (cooldown.Try()){
      Vector2 offset = Difficulty == DifficultyMode.Challenge ? Calculate.Vector.WithAngle(rotation) * 0.7f : Vector2.zero;
      
      GameObject[] go = pool.GetMany(Difficulty == DifficultyMode.Challenge ? 2 : 1);
      foreach (var g in go){
        g.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(5f, rotation / 4f);
      }

      var rb = go[0].GetComponent<Rigidbody2D>();
      rb.position = boss.eyesPosition - offset;

      if (Difficulty == DifficultyMode.Challenge){
        var rb2 = go[1].GetComponent<Rigidbody2D>();
        rb2.position = boss.eyesPosition + offset;
        rb2.rotation = (-rb2.rotation) % 360;
      }
    };
    rotation = (rotation + (int) rotationStep) % 1440;
    rotationStep += 1f;
  }

  public override void Start(Boss1 caller){
    rotationStep = 0;
    rotation = 0;
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 0.01f,
      DifficultyMode.Normal => 0.001f,
      DifficultyMode.Challenge => 0.0005f,
      _ => cooldown.WaitTime
    };
  }
  public override void Deactivate(Boss1 caller){
    rotationStep = 0;
    pool.Revoke();
  }
  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }
}

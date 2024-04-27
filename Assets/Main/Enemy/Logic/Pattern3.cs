using UnityEditor;
using UnityEngine;

class Boss1Pattern3_Missile : BossPattern<Boss1> {
  private GameObjectPool pool;
  private GameObject blueprint;
  private CooldownTimer cooldown;
  private Boss1 boss;

  void GoToPlayer(GameObject caller){
    var rb = caller.GetComponent<Rigidbody2D>();
    float rotation = Calculate.Vector.AngleTowards(rb.position, boss.Player.GetComponent<Rigidbody2D>().position);
    rb.rotation = rotation;
  }
  void SpawnChild(GameObject source){
    var go = pool.GetMany(Difficulty == DifficultyMode.Challenge ? 2 : 1);
    ResetChildProjectile(go[0], source);
    if (Difficulty == DifficultyMode.Challenge){
      ResetChildProjectile(go[1], source);
      go[1].GetComponent<Rigidbody2D>().rotation += 180;
    }
  }
  void ResetChildProjectile(GameObject go, GameObject self){
    go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    var behavior = go.GetComponent<BehaviorManager>();
    behavior.Behavior = new ProjectileBehavior.Timing(2)  
      .Chain(0, new ProjectileBehavior.Custom(), 1.5f)
      .Chain(1, new ProjectileBehavior.Propulsion(3.0f));
    var rb = go.GetComponent<Rigidbody2D>();
    rb.position = self.GetComponent<Rigidbody2D>().position;
    rb.rotation = Mathf.Sin(Time.time) * 360;
  }

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = pool.Get();
      go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
      var behaviors = go.GetComponent<BehaviorManager>();
      go.GetComponent<Rigidbody2D>().position = boss.mouthPosition;
      behaviors.Behavior = new ProjectileBehavior.Merge(new ScriptableBehavior<GameObject>[] {
          new ProjectileBehavior.Timing(3)
            .Chain(0, new ProjectileBehavior.Propulsion(3f) {
              Rotation = Random.Range(0, 360),
            }, 1f)
            .Chain(1, new ProjectileBehavior.Custom(GoToPlayer), 0.2f)
            .Chain(2, new ProjectileBehavior.Propulsion(8f)),
          new ProjectileBehavior.Spawner(SpawnChild, Difficulty switch {
            DifficultyMode.Casual => 0.1f,
            DifficultyMode.Normal => 0.05f,
            DifficultyMode.Challenge => 0.05f,
            _ => 0.05f,
        }),
      });
    }    
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 4f,
      DifficultyMode.Normal => 3f,
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

  public Boss1Pattern3_Missile(Boss1 boss){
    this.boss = boss;
    blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint, 100, 300) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern3 Missile")
    };
    cooldown = new CooldownTimer(2f);
  }
}
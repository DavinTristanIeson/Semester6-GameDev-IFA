using UnityEditor;
using UnityEngine;

class Boss1Pattern_YawnMissile : BossPattern<Boss1> {
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
    var go = pool.GetMany(2);
    for (int i = 0; i < 2; i++){
      ResetChildProjectile(go[i], source);
    }
    go[1].GetComponent<Rigidbody2D>().rotation += 180;
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
  void ResetMainProjectile(GameObject go){
    go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
    var behaviors = go.GetComponent<BehaviorManager>();
    go.GetComponent<Rigidbody2D>().position = boss.rb.position;
    behaviors.Behavior = new ProjectileBehavior.Merge<GameObject>(new ScriptableBehavior<GameObject>[] {
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

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      ResetMainProjectile(pool.Get());
    }    
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 4f,
      DifficultyMode.Normal => 3f,
      DifficultyMode.Challenge => 3f,
      _ => 3f,
    };
    boss = caller;
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public Boss1Pattern_YawnMissile(){
    blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    pool = new GameObjectPool(blueprint, 100, 300) {
      Parent = new GameObject("Boss1: ZZZ")
    };
    cooldown = new CooldownTimer(2f);
  }
}
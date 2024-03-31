using UnityEditor;
using UnityEngine;

class Boss1Pattern_FadingYawnToTheSunrise : BossPattern<Boss1> {
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
    for (int i = 0; i < projectileCount; i++) {
      GameObject projectile = projectiles[i];
      projectile.transform.localScale = new Vector3(1f, 1f, 1f);
      
      var rb = projectile.GetComponent<Rigidbody2D>();
      rb.position = parentRb.position;
      rb.rotation = i * (360f / projectileCount);

      projectile.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
        .Chain(0, new ProjectileBehavior.Propulsion(5f), go.transform.localScale.x / 10f)
        .Chain(1, new ProjectileBehavior.Custom(), 2.5f)
        .Chain(2, new ProjectileBehavior.Propulsion(Difficulty == DifficultyMode.Challenge ? 7f : 5f)
        );
    }

    if (go.transform.localScale.x <= 1f){
      go.SetActive(false);
    }
  }

  void ResetMainProjectile(GameObject go){
    go.GetComponent<Rigidbody2D>().position = boss.rb.position;
    float size = 4f + (int) Difficulty * 3f;
    go.transform.localScale = new Vector3(size, size, size);
    go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Merge(
      new ScriptableBehavior<GameObject>[] {
        new ProjectileBehavior.Homing(boss.Player, 0.5f),
        new ProjectileBehavior.Propulsion(1f),
        new ProjectileBehavior.Spawner(SpawnChildProjectile, 1f),
      }
    );
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

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern_FadingYawnToTheSunrise(){
    var blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    pool = new GameObjectPool(blueprint, 200, 400) {
      Parent = new GameObject("Boss1: Fading Yawn to the Sunrise")
    };
    cooldown = new CooldownTimer(3f);
  }
}
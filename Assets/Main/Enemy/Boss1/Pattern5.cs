using UnityEditor;
using UnityEngine;

class Boss1Pattern_StormTroopersTearDucts : BossPattern<Boss1>{
  GameObjectPool pool;
  CooldownTimer barrierCooldown;
  CooldownTimer homingCooldown;
  CooldownTimer homingCooldown2;
  private Boss1 boss;

  float GetBarrierAngle(){
    var rb = boss.Player.GetComponent<Rigidbody2D>();
    Vector2 offset = new Vector2(0, 2f + Random.Range(-0.1f, 0.1f));
    int multipler = Random.Range(0, 2) == 0 ? -1 : 1;
    Vector2 target = rb.position + (offset * multipler);
    return Calculate.Vector.AngleTowards(boss.rb.position, target);
  }
  float GetHomingAngle(){
    return Calculate.Vector.AngleTowards(boss.rb.position, boss.Player.GetComponent<Rigidbody2D>().position);
  }
  public override void Execute(Boss1 caller){
    if (barrierCooldown.Try()){
      var go = pool.Get();
      go.GetComponent<Rigidbody2D>().position = boss.rb.position;
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(2f, GetBarrierAngle());

      var sideBarrier = pool.Get();
      sideBarrier.GetComponent<Rigidbody2D>().position = boss.rb.position;
      sideBarrier.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(10f, Random.Range(0, 2) == 0 ? 90 : 270);
    };
    if (homingCooldown.Try()){
      var go = pool.Get();
      go.GetComponent<Rigidbody2D>().position = boss.rb.position;
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(Difficulty switch {
        DifficultyMode.Casual => 5f,
        DifficultyMode.Normal => 5f,
        DifficultyMode.Challenge => 2f,
        _ => 5f
      }, GetHomingAngle());
    }
    if (Difficulty == DifficultyMode.Challenge && homingCooldown2.Try()){
      var go = pool.Get();
      go.GetComponent<Rigidbody2D>().position = boss.rb.position;
      go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Propulsion(8f, GetHomingAngle() + Random.Range(-2f, 2f));
    }
  }

  public override void Start(Boss1 caller){
    boss = caller;
    homingCooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 0.8f,
      DifficultyMode.Normal => 0.5f,
      DifficultyMode.Challenge => 1f,
      _ => 0.5f,
    };
    homingCooldown2.WaitTime = 0.4f;
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public Boss1Pattern_StormTroopersTearDucts(){
    var blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    pool = new GameObjectPool(blueprint, 300, 1000) {
      Parent = new GameObject("Boss1: Storm Troopers Tear Ducts"),
    };
    barrierCooldown = new CooldownTimer(0.01f);
    homingCooldown = new CooldownTimer(0.5f);
    homingCooldown2 = new CooldownTimer(0.4f);
  }
}
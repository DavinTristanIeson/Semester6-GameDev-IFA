using UnityEditor;
using UnityEngine;

class Boss1Pattern_YawnMissile : BossPattern<Boss1> {
  private GameObjectPool pool;
  private GameObject blueprint;
  private CooldownTimer cooldown;
  private Boss1 boss;

  void GoToPlayer(BaseProjectile projectile){
    float rotation = projectile.AngleTowards(boss.Player.GetComponent<Rigidbody2D>().position);
    projectile.RigidBody.rotation = rotation;
    projectile.Speed = 8f;
  }
  void SpawnChild(BaseProjectile source){
    var go = pool.GetMany(2);
    for (int i = 0; i < 2; i++){
      ResetChildProjectile(go[i], source);
    }
    go[1].GetComponent<BaseProjectile>().RigidBody.rotation += 180;
  }
  void ResetChildProjectile(GameObject go, BaseProjectile self){
    go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    var projectile = go.GetComponent<BaseProjectile>();
    projectile.Speed = 3f;
    projectile.RigidBody.rotation = Mathf.Sin(Time.time) * 360;

    projectile.RigidBody.position = self.RigidBody.position;
    projectile.Action = null;
    projectile.Behavior = new PropulsionBehavior(0.0f, 1.5f);
  }
  void ResetMainProjectile(GameObject go){
    go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
    var projectile = go.GetComponent<BaseProjectile>();
    projectile.Speed = 1f;
    projectile.RigidBody.position = boss.RigidBody.position;
    var behaviors = new ScriptableBehavior<BaseProjectile>[] {
      new PropulsionBehavior(2f, 0.5f) {
        Rotation = Random.Range(40, 140),
        OnFinishPropulsion = GoToPlayer,
      },
      new SpawnerBehavior(SpawnChild, Difficulty switch {
        DifficultyMode.Casual => 0.1f,
        DifficultyMode.Normal => 0.05f,
        DifficultyMode.Challenge => 0.05f,
        _ => 0.05f,
     }),
    };
    projectile.Behavior = new MergeBehavior<BaseProjectile>(behaviors);
  }

  public override void Deactivate(Boss1 caller){
    pool.Deactivate();
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
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

  public Boss1Pattern_YawnMissile(){
    blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    blueprint.SetActive(false);
    pool = new GameObjectPool(blueprint, 400) {
      ParentContainer = new GameObject("Boss1: ZZZ"),
    };
    cooldown = new CooldownTimer(2f);
  }
}
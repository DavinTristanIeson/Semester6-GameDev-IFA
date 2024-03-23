using UnityEditor;
using UnityEngine;
class Boss1Pattern_TearsOfTheCatdom : BossPattern<Boss1> {
  float rotationStep = 1;
  int rotation = 0;
  private GameObjectPool pool;
  private GameObject blueprint;
  private CooldownTimer cooldown;
  public Boss1Pattern_TearsOfTheCatdom(){
    blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    blueprint.SetActive(false);
    pool = new GameObjectPool(blueprint, 30, 60) {
      Transform = ResetProjectile,
      Parent = new GameObject("Boss1: Tears of the Catdom"),
    };
    cooldown = new CooldownTimer(0.0005f);
  }
  public void ResetProjectile(GameObject go){
    BaseProjectile projectile = go.GetComponent<BaseProjectile>();
    Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
    projectile.Speed = 5f;
    rb.rotation = rotation / 4f;
  }
  public override void Execute(Boss1 boss){
    if (cooldown.Try()){
      Vector2 offset = Difficulty == DifficultyMode.Challenge ? Calculate.Vector.WithAngle(rotation) * 0.7f : Vector2.zero;
      
      GameObject[] go = pool.GetMany(2);

      var projectile = go[0].GetComponent<BaseProjectile>();
      projectile.RigidBody.position = boss.RigidBody.position - offset;

      if (Difficulty == DifficultyMode.Challenge){
        var projectile2 = go[1].GetComponent<BaseProjectile>();
        projectile2.RigidBody.position = boss.RigidBody.position + offset;
        projectile2.RigidBody.rotation = (-projectile2.RigidBody.rotation) % 360;
      }

    };
    rotation = (rotation + (int) rotationStep) % 1440;
    rotationStep += Difficulty == DifficultyMode.Casual ? 1f : 0.1f;
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
  }
}

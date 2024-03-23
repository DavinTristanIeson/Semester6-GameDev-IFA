using System;
using UnityEditor;
using UnityEngine;
public class Boss1Pattern_TrigonometricalSleep : BossPattern<Boss1> {
  private GameObjectPool pool;
  private GameObject blueprint;
  private CooldownTimer cooldown;
  private int rotation = 0;
  void ResetProjectile(GameObject go){
    go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
  }
  public Boss1Pattern_TrigonometricalSleep(){
    blueprint = AssetDatabase.LoadAssetAtPath(Constants.Prefabs.DefaultEnemyProjectile, typeof(GameObject)) as GameObject;
    blueprint.SetActive(false);
    pool = new GameObjectPool(blueprint, 100) {
      Transform = ResetProjectile,
      Parent = new GameObject("Boss1: Trigonometrical Sleep")
    };
    cooldown = new CooldownTimer(0.05f);
  }
  void Behavior(BaseProjectile caller){
    Vector2 nowPos = caller.RigidBody.position;
    Vector2 velocity = caller.Direction * -5f;
    velocity.y *= Mathf.Sin(nowPos.x) * (Difficulty == DifficultyMode.Challenge ? 2f : 1f);
    caller.RigidBody.MovePosition(nowPos + (velocity * Time.fixedDeltaTime));
  }

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var go = pool.Get();
      var projectile = go.GetComponent<BaseProjectile>();
      Vector2 yOffset = Vector2.up * ((float) Math.Sin(Time.time) * 3f);
      projectile.RigidBody.position = caller.RigidBody.position + yOffset;
      float rotationFactor = Difficulty switch {
        DifficultyMode.Casual => 3,
        DifficultyMode.Normal => 2,
        DifficultyMode.Challenge => 1,
        _ => 2,
      };
      projectile.RigidBody.rotation = Math.Abs(rotation - 180) - 90;
      projectile.Action = Behavior;
    }
    rotation = (rotation + 1) % 360;
  }

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 0.4f,
      DifficultyMode.Normal => 0.25f,
      DifficultyMode.Challenge => 0.15f,
      _ => cooldown.WaitTime
    };
  }
  public override void Deactivate(Boss1 caller){
  }
}
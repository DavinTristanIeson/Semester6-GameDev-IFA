using System;
using UnityEngine;
#nullable enable

class Boss1Pattern9_Spirals : BossPattern<Boss1>{
  GameObjectPool pool;
  Boss1 boss;
  CooldownTimer[] spiralCooldowns;
  float[] spiralCooldownBuffers;
  int[] spiralAngles;
  int[] spiralAngularSpeed;
  bool[] spiralSpawning;
  float[] spiralVelocities;

  public override void Execute(Boss1 caller){
    for (int i = 0; i < (int) Difficulty + 1; i++){
      if (spiralCooldowns[i].Try()){
        spiralSpawning[i] = !spiralSpawning[i];

        float temp = spiralCooldowns[i].WaitTime;
        spiralCooldowns[i].WaitTime = spiralCooldownBuffers[i];
        spiralCooldownBuffers[i] = temp;
      }
      if (spiralSpawning[i]){
        var go = pool.Get();
        var rb = go.GetComponent<Rigidbody2D>();
        rb.position = caller.eyesPosition;
        rb.rotation = spiralAngles[i] / 4f;
        var timing = new ProjectileBehavior.Timing(2);
        go.GetComponent<BehaviorManager>().Behavior = timing.Then(
          new ProjectileBehavior.Acceleration(8f, -0.1f, spiralVelocities[i]) {
            WhenMin = timing.Next,
          }
        ).Then(new ProjectileBehavior.Propulsion(spiralVelocities[i]));
      }
      spiralAngles[i] = (spiralAngles[i] + spiralAngularSpeed[i]) % 1440;
    }
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern9_Spirals(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Tear);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern9 Spirals"),
    };
    spiralCooldowns = new CooldownTimer[3] {
      new CooldownTimer(0.1f),
      new CooldownTimer(0.4f),
      new CooldownTimer(0.5f)
    };
    spiralCooldownBuffers = new float[3] { 0.1f, 0.2f, 0.3f };
    spiralAngles = new int[3];
    spiralSpawning = new bool[3];
    for (int i = 0; i < 3; i++){
      spiralAngles[i] = UnityEngine.Random.Range(0, 360);
      spiralSpawning[i] = true;
    }
    spiralAngularSpeed = new int[] { 12, -6, 2 };
    spiralVelocities = new float[] { 4f, 2f, 1f };
  }
}
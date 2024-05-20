using System;
using System.Linq;
using UnityEngine;
#nullable enable

class Boss1Pattern11_BigFist : BossPattern<Boss1>{
  GameObjectPool pool;
  Boss1 boss;
  CooldownTimer cooldown;

  public override void Execute(Boss1 caller){
    if (cooldown.Try()){
      var fist = pool.Get();
      fist.GetComponent<Rigidbody2D>().position = boss.GetComponent<Rigidbody2D>().position;
      fist.transform.localScale = Vector3.one * 30.0f;
      float moveTime = Time.time + 4.0f;
      ScriptableAction<GameObject> spawn = (parent) => {
        var go = pool.Get();
        go.transform.localScale = Vector3.one;
        var rb = go.GetComponent<Rigidbody2D>();
        var parentPosition = parent.GetComponent<Rigidbody2D>().position;
        var bounds = go.transform.lossyScale;
        rb.position = parentPosition + new Vector2(UnityEngine.Random.Range(-bounds.x, bounds.x), UnityEngine.Random.Range(-bounds.y, bounds.y));
        rb.rotation = UnityEngine.Random.Range(0f, 360f);
        go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(2)
          .Then(new ProjectileBehavior.Wait() {
            Until = moveTime
          })
          .Then(new ProjectileBehavior.Propulsion(2f));

      };
      fist.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(2)
        .Then(new ProjectileBehavior.Homing(boss.Player) { Once = true })
        .Then(new ProjectileBehavior.Merge(2)
          .With(new ProjectileBehavior.Propulsion(4f))
          .With(new ProjectileBehavior.Spawner(spawn, Difficulty switch {
            DifficultyMode.Casual => 0.1f,
            DifficultyMode.Normal => 0.05f,
            DifficultyMode.Challenge => 0.025f,
            _ => 0.05f,
          }))
        );
    }
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern11_BigFist(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern11 BigFist"),
    };
    cooldown = new CooldownTimer(4f);
  }
}
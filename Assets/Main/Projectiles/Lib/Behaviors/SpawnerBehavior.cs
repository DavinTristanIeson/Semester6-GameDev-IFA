#nullable enable

using UnityEngine;

namespace ProjectileBehavior {
  /// <summary>
  /// Default behavior for projectiles that spawn other game objects every interval or so
  /// </summary>
  
  class Spawner : ScriptableBehavior<GameObject> {
    public ScriptableAction<GameObject> Spawn;
    public bool Once = false;
    private int spawnCount = 0;
    public int MaxSpawnCount = int.MaxValue;
    CooldownTimer cooldown;

    public Spawner(ScriptableAction<GameObject> spawn, float spawnInterval = 1f){
      cooldown = new CooldownTimer(spawnInterval);
      Spawn = spawn;
    }

    public void Execute(GameObject caller){
      if (cooldown.Try() && spawnCount < MaxSpawnCount){
        Spawn(caller);
        spawnCount++;
        if (Once){
          caller.GetComponent<BehaviorManager>().GetBehaviorOfType<Timing>()?.Next(caller);
        }
      }
    }
  }
}
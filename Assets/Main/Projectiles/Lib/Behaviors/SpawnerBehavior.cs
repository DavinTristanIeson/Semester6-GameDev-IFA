#nullable enable

using UnityEngine;

namespace ProjectileBehavior {
  /// <summary>
  /// Default behavior for projectiles that spawn other game objects every interval or so
  /// </summary>
  class Spawner : ScriptableBehavior<GameObject> {
    public ScriptableAction<GameObject> Spawn;
    CooldownTimer cooldown;

    public Spawner(ScriptableAction<GameObject> spawn, float spawnInterval = 1f){
      cooldown = new CooldownTimer(spawnInterval);
      Spawn = spawn;
    }

    public void Execute(GameObject caller){
      if (cooldown.Try()){
        Spawn(caller);
      }
    }
  }
}
#nullable enable

/// <summary>
/// Default behavior for projectiles that spawn other game objects every interval or so
/// </summary>
class SpawnerBehavior : ScriptableBehavior<BaseProjectile> {
  public ScriptableAction<BaseProjectile>? Action;
  public ScriptableAction<BaseProjectile> Spawn;
  CooldownTimer cooldown;

  public SpawnerBehavior(ScriptableAction<BaseProjectile> spawn, float spawnInterval = 1f){
    cooldown = new CooldownTimer(spawnInterval);
    Spawn = spawn;
  }

  public void Execute(BaseProjectile caller){
    if (cooldown.Try()){
      Spawn(caller);
    }
    if (Action is ScriptableAction<BaseProjectile> action){
      action(caller);
    }
  }
}
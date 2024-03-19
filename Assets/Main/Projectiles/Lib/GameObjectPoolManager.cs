using UnityEngine;

/// <summary>
/// Reusable script for anything that needs to spawn a lot of objects with cooldowns
/// </summary>
class GameObjectPoolManager : MonoBehaviour {
  [SerializeField]
  int poolSize = 100;
  [SerializeField]
  GameObject blueprint;
  [SerializeField]
  string parentName;
  [SerializeField]
  float cooldownSeconds = 0.05f;

  private GameObjectPool pool;
  private CooldownTimer cooldown;
  public GameObjectPool Pool {
    get => pool;
  }

  public GameObject Get(){
    return pool.Get();
  }
  public bool Try(){
    return cooldown.Try();
  }

  /// <summary>
  /// Try to get a game object if the pool is not in cooldown. Returns null if it is in cooldown.
  /// </summary>
  /// <returns></returns>
  public GameObject TryGet(){
    return cooldown.Try() ? pool.Get() : null;
  }

  void OnEnable(){
    if (pool == null){
      pool = new GameObjectPool(blueprint, poolSize);
    }
    if (parentName is string parent && parent.Length > 0){
      pool.ParentContainer = new GameObject(parent);
    }
    if (cooldown == null){
      cooldown = new CooldownTimer(cooldownSeconds);
    }
  }
  
  void OnDisable(){
    if (pool != null){
      pool.Destroy();
      pool = null;
    }
  }

  void OnDestroy(){
    if (pool != null){
      pool.Destroy();
      pool = null;
    }
  }
}
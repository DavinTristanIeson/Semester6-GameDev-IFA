using UnityEngine;

/// <summary>
/// Reusable script for anything that needs to spawn a lot of objects with cooldowns
/// </summary>
class GameObjectPoolManager : MonoBehaviour {
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

  void OnEnable(){
    if (pool == null){
      pool = new GameObjectPool(blueprint);
    }
    if (parentName is string parent && parent.Length > 0){
      pool.Parent = new GameObject(parent);
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
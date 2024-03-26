using UnityEngine;

#nullable enable

class BehaviorManager : MonoBehaviour, IGetBehavior {
  ScriptableBehavior<GameObject>? behavior;
  public ScriptableBehavior<GameObject>? Behavior {
    get => behavior;
    set {
      behavior = value;
      behavior?.Start(gameObject);
    }
  }
  public ScriptableAction<GameObject> Action {
    set => Behavior = new ProjectileBehavior.Custom(value);
  }

  void OnEnable(){
    Behavior?.Start(gameObject);
  }

  void FixedUpdate(){
    Behavior?.Execute(gameObject);
  }

  void OnDisable(){
    Behavior?.Deactivate(gameObject);
  }

  void OnDestroy(){
    Behavior?.Destroy(gameObject);
  }

  public T? GetBehaviorOfType<T>() where T : class? {
    if (Behavior is T found){
      return found;
    }
    if (Behavior is IGetBehavior finder){
      return finder.GetBehaviorOfType<T>();
    }
    return null;
  }
}
#nullable enable

using UnityEngine;

namespace ProjectileBehavior {
  /// <summary>
  /// Merges multiple scriptable behaviors together
  /// </summary>
  class Merge : ScriptableBehavior<GameObject>, IGetBehavior {
    protected ScriptableBehavior<GameObject>[] behaviors;

    public Merge(ScriptableBehavior<GameObject>[] behaviors){
      this.behaviors = behaviors;
    }
    public Merge(int count){
      behaviors = new ScriptableBehavior<GameObject>[count];
    }

    private int withIndex = 0;
    public Merge With(ScriptableBehavior<GameObject> behavior){
      behaviors[withIndex] = behavior;
      withIndex++;
      return this;
    }

    public void Destroy(GameObject caller){
      foreach (var behavior in behaviors){
        behavior.Destroy(caller);
      }
    }

    public void Deactivate(GameObject caller){
      foreach (var behavior in behaviors){
        behavior.Deactivate(caller);
      }
    }

    public void Execute(GameObject caller){
      foreach (var behavior in behaviors){
        behavior.Execute(caller);
      }
    }

    public void Start(GameObject caller){
      foreach (var behavior in behaviors){
        behavior.Start(caller);
      }
    }

    public T? GetBehaviorOfType<T>() where T : class? {
      foreach (var behavior in behaviors){
        if (behavior is T found){
          return found;
        }
      }
      foreach (var behavior in behaviors){
        if (behavior is IGetBehavior finder){
          return finder.GetBehaviorOfType<T>();
        }
      }
      return null;
    }
  }
}
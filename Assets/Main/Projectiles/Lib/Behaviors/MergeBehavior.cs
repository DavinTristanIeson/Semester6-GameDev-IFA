#nullable enable

namespace ProjectileBehavior {
  /// <summary>
  /// Merges multiple scriptable behaviors together
  /// </summary>
  /// <typeparam name="T"></typeparam>
  class Merge<T> : ScriptableBehavior<T> {
    protected ScriptableBehavior<T>[] behaviors;

    public Merge(ScriptableBehavior<T>[] behaviors){
      this.behaviors = behaviors;
    }

    public void Destroy(T caller){
      foreach (var behavior in behaviors){
        behavior.Destroy(caller);
      }
    }

    public void Deactivate(T caller){
      foreach (var behavior in behaviors){
        behavior.Deactivate(caller);
      }
    }

    public void Execute(T caller){
      foreach (var behavior in behaviors){
        behavior.Execute(caller);
      }
    }

    public void Start(T caller){
      foreach (var behavior in behaviors){
        behavior.Start(caller);
      }
    }
  }
}
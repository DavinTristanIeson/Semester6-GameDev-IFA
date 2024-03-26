using UnityEngine;

#nullable enable
public delegate void ScriptableAction<T>(T caller);
public interface ScriptableBehavior<T> {
  public void Start(T caller){}
  public void Execute(T caller){}
  public void Deactivate(T caller){}
  public void Destroy(T caller){}
}

interface IGetBehavior {
  public T? GetBehaviorOfType<T>() where T : class?;
}

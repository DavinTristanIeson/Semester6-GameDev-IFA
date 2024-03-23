using UnityEngine;
public interface ScriptableBehavior<T> {
  public void Start(T caller){}
  public void Execute(T caller);
  public void Destroy(T caller){}
  public void Deactivate(T caller){}
}
public delegate void ScriptableAction<T>(T caller);

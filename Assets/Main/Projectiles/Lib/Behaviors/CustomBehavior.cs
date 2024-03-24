using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  class Custom : ScriptableBehavior<GameObject> {
    public ScriptableAction<GameObject>? OnStart;
    public ScriptableAction<GameObject>? OnExecute;
    public ScriptableAction<GameObject>? OnDeactivate;
    public ScriptableAction<GameObject>? OnDestroy;
    public void Start(GameObject caller){
      if (OnStart is not null){
        OnStart!(caller);
      }
    }
    public void Execute(GameObject caller){
      if (OnExecute is not null){
        OnExecute!(caller);
      }
    }
    public void Deactivate(GameObject caller){
      if (OnDeactivate is not null){
        OnDeactivate!(caller);
      }
    }
    public void Destroy(GameObject caller){
      if (OnDestroy is not null){
        OnDestroy!(caller);
      }
    }

    public Custom(){}
    public Custom(ScriptableAction<GameObject> action){
      OnExecute = action;
    }
  }
}
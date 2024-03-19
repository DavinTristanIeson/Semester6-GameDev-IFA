using UnityEngine;

#nullable enable

enum PropulsionState {
  Propelling,
  Delaying,
  Executing,
}

/// <summary>
/// Default behavior for projectiles that will be propelled to a direction for some time, before letting its custom action take over.
/// </summary>
class PropulsionBehavior : ScriptableBehavior<BaseProjectile> {
  protected PropulsionState state;

  /// <summary>
  /// Custom action when state is in ``PropulsionState.Executing``. Default is forward movement.
  /// </summary>
  public ScriptableAction<BaseProjectile>? Action;
  /// <summary>
  /// Custom action when state is in ``PropulsionState.Propelling``. Default is forward movement.
  /// </summary>
  public ScriptableAction<BaseProjectile>? Propulsion;
  /// <summary>
  /// Callback when state changes from ``PropulsionState.Delaying`` to ``PropulsionState.Executing``
  /// </summary>
  public ScriptableAction<BaseProjectile>? OnFinishPropulsion;
  /// <summary>
  /// Time for default behavior to execute in ``PropulsionState.Propelling``
  /// </summary>
  public float PropulsionTime;
  /// <summary>
  /// Time to wait before ``action`` begins executing. State is in ``PropulsionState.Delaying``
  /// </summary>
  public float DelayTime;
  /// <summary>
  /// Initial rotation of the projectile when propelling.
  /// </summary>
  public float? Rotation;

  public PropulsionBehavior(float propulsionTime = 0.5f, float delayTime = 0.0f){
    PropulsionTime = propulsionTime;
    DelayTime = delayTime;
    state = PropulsionState.Propelling;
  }

  public void Start(BaseProjectile caller){
    if (Rotation is float rotation){
      caller.RigidBody.rotation = rotation;
    }
    state = PropulsionState.Propelling;
  }
  public void Execute(BaseProjectile projectile){
    if (state == PropulsionState.Executing){
      if (Action is ScriptableAction<BaseProjectile> action){
        action(projectile);
      } else {
        projectile.Move(projectile.Direction * projectile.Speed);
      }
    } else if (state == PropulsionState.Propelling){
      if (Propulsion is ScriptableAction<BaseProjectile> propulsion){
        propulsion(projectile);
      } else {
        projectile.Move(projectile.Direction * projectile.Speed);
      }
    }

    if (state == PropulsionState.Propelling && Time.time > projectile.SpawnTime + PropulsionTime){
      state = PropulsionState.Delaying;
    }
    if (state == PropulsionState.Delaying && Time.time > projectile.SpawnTime + PropulsionTime + DelayTime){
      state = PropulsionState.Executing;
      if (OnFinishPropulsion is ScriptableAction<BaseProjectile> onFinishPropulsion){
        onFinishPropulsion(projectile);
      }
    }
  }
  public void Destroy(BaseProjectile caller){

  }
  public void Deactivate(BaseProjectile caller){
  }
}
using System;
using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  delegate void AcceleratorScriptableAction(GameObject go, float value);
  class Acceleration : ScriptableBehavior<GameObject> {
    public float Increment;
    public float Speed;
    public float Min = 0;
    public float Max = float.MaxValue;
    public AcceleratorScriptableAction? Accelerate;

    public Acceleration(float initial, float increment, float min = 0, float max = float.MaxValue){
      Speed = initial;
      Increment = increment;
      Min = min;
      Max = max;
    }

    public void Execute(GameObject go){
      Speed = Mathf.Clamp(Speed + Increment, Min, Max);
      if (Accelerate is AcceleratorScriptableAction accelerate){
        accelerate(go, Speed);
        return;
      }

      var behaviorManager = go.GetComponent<BehaviorManager>();
      if (behaviorManager is BehaviorManager bm){
        var propulsion = bm.GetBehaviorOfType<Propulsion>();
        if (propulsion is Propulsion prop){
          prop.Speed = Speed;
          return;
        }
      }

      var rigidBody = go.GetComponent<Rigidbody2D>();
      if (rigidBody is Rigidbody2D rb){
        Vector2 direction = Calculate.Vector.WithAngle(rb.rotation);
        Vector2 force = direction * Speed;
        rb.MovePosition(rb.position + (force * Time.fixedDeltaTime));
      }
    }
  }
}
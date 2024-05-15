using System;
using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  delegate Vector2 GetPositionFn(GameObject go);
  class Homing : ScriptableBehavior<GameObject> {
    public GameObject? Target;
    public GetPositionFn? GetPosition;
    public Vector2? TargetPosition;
    public bool Once = false;
    public float MaxRotation;
    public Homing(GameObject target){
      Target = target;
    }
    public Homing(Vector2 target){
      TargetPosition = target;
    }
    public Homing(GetPositionFn getter){
      GetPosition = getter;
    }

    float getAngle(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      Vector2 targetPosition;
      if (Target is GameObject target){
        targetPosition = target.GetComponent<Rigidbody2D>().position;
      } else if (GetPosition is GetPositionFn getPosition){
        targetPosition = getPosition(caller);
      } else if (TargetPosition is Vector2 position){
        targetPosition = position;
      } else {
        return rb.rotation;
      }
      Vector2 currentPosition = rb.position;
      return Calculate.Vector.AngleTowards(currentPosition, targetPosition);
    }

    public void Start(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      rb.rotation = getAngle(caller);
    }

    public void Execute(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      float angle = getAngle(caller);
      rb.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, 0, rb.rotation), Quaternion.Euler(0, 0, angle), MaxRotation).eulerAngles.z;

      if (Once){
        caller.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Timing>()?.Next(caller);
      }
    }
  }
}
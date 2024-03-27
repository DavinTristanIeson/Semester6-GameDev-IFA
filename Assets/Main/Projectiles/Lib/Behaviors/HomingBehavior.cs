using System;
using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  delegate Vector2 GetPositionFn(GameObject go);
  class Homing : ScriptableBehavior<GameObject> {
    public GameObject? Target;
    public GetPositionFn? GetPosition;
    public float MaxDegreesDelta;
    public Homing(GameObject target, float maxRotation){
      Target = target;
      MaxDegreesDelta = maxRotation;
    }
    public Homing(GetPositionFn getter, float maxRotation){
      GetPosition = getter;
      MaxDegreesDelta = maxRotation;
    }

    float getAngle(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      Vector2 targetPosition;
      if (Target is GameObject target){
        targetPosition = target.GetComponent<Rigidbody2D>().position;
      } else if (GetPosition is GetPositionFn getPosition){
        targetPosition = getPosition(caller);
      } else {
        throw new Exception("GetPosition or Target must not be null!");
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
      rb.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, 0, rb.rotation), Quaternion.Euler(0, 0, angle), MaxDegreesDelta).eulerAngles.z;
    }
  }
}
using UnityEngine;

namespace ProjectileBehavior {
  class Homing : ScriptableBehavior<GameObject> {
    public GameObject Target;
    public float MaxDegreesDelta;
    public Homing(GameObject target, float maxRotation){
      Target = target;
      MaxDegreesDelta = maxRotation;
    }

    float getAngle(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      Vector2 targetPosition = Target.GetComponent<Rigidbody2D>().position;
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
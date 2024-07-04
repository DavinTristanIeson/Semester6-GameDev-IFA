using System;
using UnityEngine;

class MainMenuOrbit : MonoBehaviour {
  public float AngularSpeed = 1f;
  public float Speed = 1f;
  public float OrbitStray = 3f;

  void FixedUpdate(){
    Vector2 newPosition = Calculate.Vector.RotateAround(transform.position, Vector2.zero, Speed * Time.fixedDeltaTime);
    float angle = -1 * Calculate.Vector.AngleTowards(transform.position, Vector2.zero);
    newPosition += Calculate.Vector.WithAngle(angle) * (Mathf.Sin(Time.time) * OrbitStray * Time.fixedDeltaTime);
    transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + AngularSpeed * Time.fixedDeltaTime);
  }
}
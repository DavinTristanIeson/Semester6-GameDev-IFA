using UnityEngine;

namespace Calculate {
  public class Vector {
    public static Vector2 WithAngle(float angle){
      return Quaternion.Euler(0, 0, angle) * Vector2.right;
    }
    public static float AngleTowards(Vector2 current, Vector2 target){
      Vector2 direction = target - current;
      return Vector2.SignedAngle(Vector2.right, direction);
    }
  }
}
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
    public static Vector2 RotateAround(Vector2 point, Vector2 pivot, float angle){
      // https://discussions.unity.com/t/rotate-a-vector-around-a-certain-point/81225/2
      Vector2 direction = point - pivot;
      Vector3 rotatedDirection = Quaternion.Euler(0, 0, angle) * direction;
      return new Vector2(
        rotatedDirection.x + pivot.x,
        rotatedDirection.y + pivot.y
      );
    }
  }
}
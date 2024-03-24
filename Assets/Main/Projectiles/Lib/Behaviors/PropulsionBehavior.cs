using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  /// <summary>
  /// Default behavior for projectiles that will be propelled to a direction.
  /// </summary>
  class Propulsion : ScriptableBehavior<GameObject> {
    /// <summary>
    /// Initial rotation of the projectile when propelling.
    /// </summary>
    public float? Rotation;
    /// <summary>
    /// Speed of the projectile
    /// </summary>
    public float Speed;

    public Propulsion(float speed, float? rotation = null){
      Speed = speed;
      Rotation = rotation;
    }

    public void Start(GameObject caller){
      if (Rotation is float rotation){
        caller.GetComponent<Rigidbody2D>().rotation = rotation;
      }
    }
    public void Execute(GameObject caller){
      var rb = caller.GetComponent<Rigidbody2D>();
      Vector2 direction = Calculate.Vector.WithAngle(rb.rotation);
      Vector2 force = direction * Speed;
      rb.MovePosition(rb.position + (force * Time.fixedDeltaTime));
    }
  }
}
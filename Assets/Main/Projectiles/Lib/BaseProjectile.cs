using UnityEngine;

#nullable enable

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseProjectile : MonoBehaviour {
  public Rigidbody2D rb {
    get => GetComponent<Rigidbody2D>();
  }
  // Movement
  public Vector2 Direction {
    get => Calculate.Vector.WithAngle(rb.rotation);
  }
  public float AngleTowards(Vector2 target){
    return Calculate.Vector.AngleTowards(rb.position, target);
  }

  // Hooks

  public void Move(Vector2 force){
    rb.MovePosition(rb.position + (force * Time.fixedDeltaTime));
  }
  
  public void Deactivate(){
    gameObject.SetActive(false);
  }

  void OnBecameInvisible(){
    Deactivate();
  }
}
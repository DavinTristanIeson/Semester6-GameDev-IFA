using UnityEngine;

#nullable enable

[RequireComponent(typeof(Rigidbody2D))]
public class BaseProjectile : MonoBehaviour {
  float timeSinceInvisible = 0;
  bool invisible = false;
  
  public float DeactivationDelay = 1.0f;

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

  public void OnEnable(){
    invisible = false;
  }

  void Update(){
    if (invisible && timeSinceInvisible + DeactivationDelay <= Time.time){
      Deactivate();
    }
  }

  void OnBecameVisible(){
    invisible = false;
  }

  void OnBecameInvisible(){
    invisible = true;
    timeSinceInvisible = Time.time;
  }
}
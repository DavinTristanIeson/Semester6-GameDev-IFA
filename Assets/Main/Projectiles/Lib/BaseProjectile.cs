using System;
using UnityEngine;

#nullable enable

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseProjectile : MonoBehaviour {
  // Serialized
  public ScriptableBehavior<BaseProjectile>? Behavior;
  public ScriptableAction<BaseProjectile>? Action;


  // Accessible from other scripts

  /// <summary>
  /// The time that the projectile is enabled
  /// </summary>
  public float SpawnTime { get => spawnTime; }
  [NonSerialized]
  public float Speed;  
  public Rigidbody2D RigidBody {
    get => rigidBody;
  }

  // Personal
  private float spawnTime;
  protected Rigidbody2D rigidBody;
  

  // Movement
  public Vector2 Direction {
    get => Calculate.Vector.WithAngle(rigidBody.rotation);
  }
  public float AngleTowards(Vector2 target){
    return Calculate.Vector.AngleTowards(rigidBody.position, target);
  }

  public void Move(Vector2 force){
    RigidBody.MovePosition(RigidBody.position + (force * Time.fixedDeltaTime));
  }
  
  protected void Deactivate(){
    gameObject.SetActive(false);
  }

  protected abstract void Initialize();

  void OnEnable(){
    if (rigidBody is null){
      rigidBody = GetComponent<Rigidbody2D>();
    }
    if (Behavior is not null){
      Behavior.Start(this);
    }
    spawnTime = Time.time;
    Initialize();
  }
  void OnDisable(){
    if (Behavior is not null){
      Behavior.Deactivate(this);
    }
  }
  void OnDestroy(){
    if (Behavior is not null){
      Behavior.Destroy(this);
    }
  }
  void OnBecameInvisible(){
    Deactivate();
  }
}
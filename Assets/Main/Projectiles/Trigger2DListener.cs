using UnityEngine;

public delegate void Trigger2DListenerEvent(GameObject self, Collider2D other); 
public class Trigger2DListener : MonoBehaviour {
  public event Trigger2DListenerEvent EnterListener;
  public event Trigger2DListenerEvent StayListener;
  public event Trigger2DListenerEvent ExitListener;

  public static bool IsPlayerCollider(Collider2D collider){
    return collider.gameObject.layer == Constants.Layers.Player && collider.gameObject.tag == Constants.Tags.Hurtbox;
  }

  public static bool IsEnemyProjectileCollider(Collider2D collider){
    return collider.gameObject.layer == Constants.Layers.EnemyProjectiles;
  }

  void OnDisable(){
    ExitListener = null;
    EnterListener = null;
    StayListener = null;
  }

  void OnTriggerExit2D(Collider2D collider){
    if (IsPlayerCollider(collider) || IsEnemyProjectileCollider(collider)){
      if (ExitListener is Trigger2DListenerEvent call){
        call(gameObject, collider);
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider){
    if (IsPlayerCollider(collider) || IsEnemyProjectileCollider(collider)){
      if (EnterListener is Trigger2DListenerEvent call){
        call(gameObject, collider);
      }
    }
  }
  void OnTriggerStay2D(Collider2D collider){
    if (IsPlayerCollider(collider) || IsEnemyProjectileCollider(collider)){
      if (StayListener is Trigger2DListenerEvent call){
        call(gameObject, collider);
      }
    }
  }
}
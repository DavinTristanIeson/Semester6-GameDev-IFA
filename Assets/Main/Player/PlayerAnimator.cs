using Constants;
using UnityEngine;

[RequireComponent(typeof (Animator))]
public class PlayerAnimator : MonoBehaviour {
  Vector2 lastPosition;
  Animator animator;
  HealthManager health;
  int lastAcknowledgedHealth;
  
  Vector2 currentPosition {
    get => new Vector2(transform.position.x, transform.position.y);
  }

  void OnEnable(){
    lastPosition = currentPosition;
    animator = GetComponent<Animator>();
    health = GetComponentInParent<HealthManager>();
    lastAcknowledgedHealth = health.Health;
    GameObject.Find(GameObjectNames.Hurtbox);
  }

  void Update(){
    if (lastAcknowledgedHealth > health.Health){
      animator.Play(Constants.AnimationStates.Player.Angry);
      lastAcknowledgedHealth = health.Health;
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.AnimationStates.Player.Angry)){
      lastPosition = transform.position;
      return;
    }
    var diff = (currentPosition - lastPosition).magnitude;
    if (diff > 0){
      animator.Play(Constants.AnimationStates.Player.Run);
      animator.SetFloat(Constants.AnimationStates.Player.ParamFloatSprint, Mathf.Clamp(diff / 0.25f, 0f, 1f));
    }
    lastPosition = transform.position;
  }
}
using Constants;
using UnityEngine;

[RequireComponent(typeof (Animator))]
public class PlayerAnimator : MonoBehaviour {
  Vector2 lastPosition;
  Animator animator;
  CooldownTimer animationCooldown;
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
    animationCooldown = new CooldownTimer(0.1f);
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
    if (animationCooldown.InCooldown) return;

    var diff = (currentPosition - lastPosition).magnitude;
    if (diff > 0){
      animator.Play(Constants.AnimationStates.Player.Run);
      animator.SetFloat(Constants.AnimationStates.Player.ParamFloatSprint, Mathf.Clamp(diff / 0.4f, 0f, 1f));
    } else {
      animator.Play(Constants.AnimationStates.Player.Idle);
    }
    animationCooldown.Try();
    lastPosition = transform.position;
  }
}
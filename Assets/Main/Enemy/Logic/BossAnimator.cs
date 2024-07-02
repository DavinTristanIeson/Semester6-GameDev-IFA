using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (SpriteRenderer))]
public class Boss1Animator : MonoBehaviour {
  Animator animator;
  static public int LastPhase = -1;
  void OnEnable(){
    animator = GetComponent<Animator>();
  }
  public void TriggerPhase(int i){
    if (i == 0){
      animator.Play(Constants.AnimationStates.Boss.WakeToSleep);
    } else if (i == 1){
      animator.Play(Constants.AnimationStates.Boss.SleepToAngry);
    } else if (i == 2){
      animator.Play(Constants.AnimationStates.Boss.Cry);
    } else {
      animator.Play(Constants.AnimationStates.Boss.Idle);
    }
  }
}
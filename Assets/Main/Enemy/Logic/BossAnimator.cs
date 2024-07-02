using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (SpriteRenderer))]
public class Boss1Animator : MonoBehaviour {
  Animator animator;
  SpriteRenderer sprite;

  int phase = 0;
  static public int LastPhase = -1;
  void OnEnable(){
    animator = GetComponent<Animator>();
    sprite = GetComponent<SpriteRenderer>();
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
    phase = i;
  }

  public void Update(){
    if (phase != LastPhase) return;
    if (sprite.color.r == 1 && sprite.color.g == 1 && sprite.color.b == 1){
      sprite.color = new Color(1f, 0.5f, 0.5f, 1f);
    }
    float hue, saturation, value;
    Color.RGBToHSV(sprite.color, out hue, out saturation, out value);
    float newHue = hue + 0.0005f;
    if (newHue > 1f){
      newHue -= 1f;
    }
    sprite.color = Color.HSVToRGB(newHue, saturation, value);
  }
}
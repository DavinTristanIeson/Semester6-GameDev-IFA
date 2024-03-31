using UnityEngine;

#nullable enable

/// <summary>
/// Used for invisibility frames and bullet cooldown
/// </summary>
public class CooldownTimer {
  /// <summary>
  /// The duration of the cooldown
  /// </summary>
  public float WaitTime = 0.5f;
  private float lastValid = 0.0f;

  /// <summary>
  /// Is the timer in cooldown or not?
  /// </summary>
  public bool InCooldown {
    get => lastValid + WaitTime > Time.time;
  }

  public CooldownTimer(float CooldownSeconds){
    WaitTime = CooldownSeconds;
  }

  /// <summary>
  /// Checks if the timer is in cooldown or not; if it is not, then it initiates another cooldown.
  /// </summary>
  /// <returns>A boolean that determines if the timer is in cooldown or not.</returns>
  public bool Try(){
    if (InCooldown){
      return false;
    }
    lastValid = Time.time;
    return true;
  }

  public void Reset(){
    lastValid = Time.time;
  }
}
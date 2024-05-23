using System.Linq;
using UnityEngine;

#nullable enable

public enum DifficultyMode {
  Casual,
  Normal,
  Challenge
}

public abstract class BossPattern<T> : ScriptableBehavior<T> {
  public DifficultyMode Difficulty = DifficultyMode.Normal;
  public virtual void Start(T caller){}
  public virtual void Execute(T caller){}
  public virtual void Deactivate(T caller){}
  public virtual void Destroy(T caller){}
}

/// <summary>
/// Manages when the pattern is changed from one pattern to another.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BossPatternManager<T> where T : MonoBehaviour {
  BossPattern<T>[] patterns;
  float lastPatternTime = 0.0f;
  public float PatternLength;
  public int PatternsCount {
    get => patterns.Length;
  }
  private const int NO_PATTERN = -1;
  private int currentPattern = NO_PATTERN;
  public DifficultyMode difficulty;
  public BossPatternManager(BossPattern<T>[] patterns, DifficultyMode difficulty, float patternLength = 20.0f){
    this.patterns = patterns;
    foreach (var pattern in patterns){
      pattern.Difficulty = difficulty;
    }
    this.patterns = this.patterns.OrderBy((x) => Random.Range(0, int.MaxValue)).ToArray();

    this.difficulty = difficulty;
    PatternLength = patternLength;
  }

  BossPattern<T> Pattern {
    get => patterns[currentPattern];
  }
  /// <summary>
  /// Changes the pattern when appropriate. After all patterns have been exhausted, the patterns will be reset.
  /// </summary>
  /// <param name="caller"></param>
  public void NextPattern(T caller){
    if (Time.time < lastPatternTime + PatternLength && currentPattern != NO_PATTERN) return;
    if (currentPattern != NO_PATTERN){
      Pattern.Deactivate(caller);
    }
    if (currentPattern >= patterns.Length - 1){
      patterns = patterns.OrderBy((x) => Random.Range(0, int.MaxValue)).ToArray();
      currentPattern = 0;
    } else {
      currentPattern++;
    }
    lastPatternTime = Time.time;
    Pattern.Start(caller);
  }
  /// <summary>
  /// Executes the current pattern
  /// </summary>
  /// <param name="caller"></param>
  public void Execute(T caller){
    if (currentPattern != NO_PATTERN){
      Pattern.Execute(caller);
    }
  }

  public void Deactivate(T caller){
    Pattern.Deactivate(caller);
  }

  public void Destroy(T caller){
    foreach (var pattern in patterns){
      pattern.Destroy(caller);
    }
  }
}

class BossPhase<T> where T : MonoBehaviour {
  public BossPatternManager<T> Pattern { get; private set; }
  public int Health { get; private set; }
  public BossPhase(BossPatternManager<T> pattern, int health){
    Pattern = pattern;
    Health = health;
  }
}
class BossPhaseManager<T> where T : MonoBehaviour {
  int phase = 0;
  BossPhase<T>[] phases;
  public BossPhaseManager(BossPhase<T>[] phases){
    this.phases = phases.OrderBy((x) => -1 * x.Health).ToArray();
  }

  public void Execute(T caller, int health){
    if (phase + 1 < phases.Length && phases[phase + 1].Health >= health){
      phases[phase].Pattern.Deactivate(caller);
      phase++;
    }
    phases[phase].Pattern.NextPattern(caller);
    phases[phase].Pattern.Execute(caller);
  }

  public void Destroy(T caller){
    foreach (var phase in phases){
      phase.Pattern.Destroy(caller);
    }
  }
}
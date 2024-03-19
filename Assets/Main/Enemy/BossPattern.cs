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
  abstract public void Deactivate(T caller);
  abstract public void Destroy(T caller);
  abstract public void Execute(T caller);
  abstract public void Start(T caller);
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
  public BossPatternManager(BossPattern<T>[] patterns, DifficultyMode difficulty, float patternLength = 60.0f){
    this.patterns = patterns;
    foreach (var pattern in patterns){
      pattern.Difficulty = difficulty;
    }
    this.difficulty = difficulty;
    PatternLength = patternLength;
  }
  /// <summary>
  /// Changes the pattern when appropriate. After all patterns have been exhausted, the patterns will be reset.
  /// </summary>
  /// <param name="caller"></param>
  public void NextPattern(T caller){
    if (Time.time < lastPatternTime + PatternLength && currentPattern != NO_PATTERN) return;
    if (currentPattern != NO_PATTERN){
      patterns[currentPattern].Deactivate(caller);
    }
    if (currentPattern >= patterns.Length - 1){
      patterns = patterns.OrderBy((x) => Random.Range(0, int.MaxValue)).ToArray();
      currentPattern = 0;
    } else {
      currentPattern++;
    }
    lastPatternTime = Time.time;
    patterns[currentPattern].Start(caller);
  }
  /// <summary>
  /// Executes the current pattern
  /// </summary>
  /// <param name="caller"></param>
  public void Execute(T caller){
    if (currentPattern != NO_PATTERN){
      patterns[currentPattern].Execute(caller);
    }
  }

  public void Destroy(T caller){
    if (currentPattern != NO_PATTERN){
      patterns[currentPattern].Destroy(caller);
    }
  }
}
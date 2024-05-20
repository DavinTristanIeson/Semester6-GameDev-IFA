using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  delegate bool TimingCondition(GameObject go);
  class TimingAgent {
    public ScriptableBehavior<GameObject> Behavior;
    public float? WaitTime;
    public TimingCondition? Condition;
    public TimingAgent(ScriptableBehavior<GameObject> behavior, float? waitTime = null, TimingCondition? condition = null){
      Behavior = behavior;
      WaitTime = waitTime;
      Condition = condition;
    }
  }

  class Timing : ScriptableBehavior<GameObject>, IGetBehavior {
    TimingAgent[] behaviors;
    int behaviorIndex = 0;
    float spawnTime = 0;
    float lastChangeTime = 0;


    public bool IsRepeating = false;
    public float SpawnTime {
      get => spawnTime;
    }


    public Timing(TimingAgent[] behaviors){
      this.behaviors = behaviors;
    }
    public Timing(int count){
      behaviors = new TimingAgent[count];
    }

    private int thenIndex = 0;
    public Timing Then(ScriptableBehavior<GameObject> agent, float? waitTime = null, TimingCondition? condition = null){
      behaviors[thenIndex] = new TimingAgent(agent, waitTime, condition);
      thenIndex++;
      return this;
    }


    ScriptableBehavior<GameObject> currentBehavior {
      get => behaviors[behaviorIndex].Behavior;
    }

    public void Next(GameObject caller){
      lastChangeTime = Time.time;

      if (behaviorIndex >= behaviors.Length - 1){
        if (IsRepeating){
          currentBehavior.Deactivate(caller);
          behaviorIndex = 0;
          currentBehavior.Start(caller);
        }
        return;
      } else {
        currentBehavior.Deactivate(caller);
        behaviorIndex++;
      }

      currentBehavior.Start(caller);
    }

    public void Start(GameObject caller){
      behaviorIndex = 0;
      spawnTime = Time.time;
      lastChangeTime = Time.time;
      currentBehavior.Start(caller);
    }

    public void Execute(GameObject caller){
      var agent = behaviors[behaviorIndex];
      if (agent.WaitTime is not null && Time.time >= lastChangeTime + agent.WaitTime){
        Next(caller);
      } else if (agent.Condition is not null && agent.Condition(caller)){
        Next(caller);
      }

      currentBehavior.Execute(caller);
    }
    public void Deactivate(GameObject caller){
      currentBehavior.Deactivate(caller);
    }
    public void Destroy(GameObject caller){
      foreach (var behavior in behaviors){
        behavior.Behavior.Destroy(caller);
      }
    }

    public T? GetBehaviorOfType<T>() where T : class? {
      if (currentBehavior is T behavior){
        return behavior;
      }
      if (currentBehavior is IGetBehavior getter){
        return getter.GetBehaviorOfType<T>();
      }
      return null;
    }
  }

  class Wait : ScriptableBehavior<GameObject>, IGetBehavior {
    public float? Delay;
    public float? Until;
    private float lastTime = 0.0f;
    public TimingCondition? Condition;

    public Wait(){}

    public void Start(GameObject go){
      lastTime = Time.time;
    }

    void next(GameObject go){
      go.GetComponent<BehaviorManager>().GetBehaviorOfType<Timing>()?.Next(go);
    }

    public void Execute(GameObject go){
      float now = Time.time;
      if (Condition is TimingCondition condition && condition(go)){
        next(go);
      }
      if (Delay is float delayTime && now >= lastTime + delayTime){
        next(go);
      }
      if (Until is float untilTime && now >= untilTime){
        next(go);
      }
    }
    
    public T? GetBehaviorOfType<T>() where T : class? {
      if (this is T behavior){
        return behavior;
      }
      return null;
    }
  }
}
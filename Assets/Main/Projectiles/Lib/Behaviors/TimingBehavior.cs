using UnityEngine;

#nullable enable

namespace ProjectileBehavior {
  class TimingAgent {
    public ScriptableBehavior<GameObject> Behavior;
    public float? WaitTime;
    public TimingAgent(ScriptableBehavior<GameObject> behavior, float? waitTime = null){
      Behavior = behavior;
      WaitTime = waitTime;
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

    public Timing Chain(int index, ScriptableBehavior<GameObject> agent){
      behaviors[index] = new TimingAgent(agent);
      return this;
    }
    public Timing Chain(int index, ScriptableBehavior<GameObject> agent, float waitTime){
      behaviors[index] = new TimingAgent(agent, waitTime);
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
}
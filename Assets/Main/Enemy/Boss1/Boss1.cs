using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (HealthManager))]
public class Boss1 : MonoBehaviour {
  public DifficultyMode difficultyMode = DifficultyMode.Normal;

  private BossPatternManager<Boss1> patterns;
  private HealthManager health;
  private Rigidbody2D rigidBody;
  private GameObject player;


  public GameObject Player {
    get => player;
  }
  public Rigidbody2D RigidBody {
    get => rigidBody;
  }


  void OnEnable(){
    rigidBody = GetComponent<Rigidbody2D>();
    health = GetComponent<HealthManager>();
    player = GameObject.FindWithTag(Constants.Tags.Player);
    health.Reset();
    if (patterns == null){
      var basePatterns = new BossPattern<Boss1>[] {
        new Boss1Pattern_TearsOfTheCatdom(),
        new Boss1Pattern_TrigonometricalSleep(),
        new Boss1Pattern_YawnMissile(),
      };
      patterns = new BossPatternManager<Boss1>(basePatterns, difficultyMode, patternLength: 20);
    }
  }

  void OnDisable(){
    if (patterns != null){
      patterns.Destroy(this);
    }
  }

  void Update(){
    patterns.NextPattern(this);
    patterns.Execute(this);
  }

  void OnTriggerEnter2D(Collider2D collider){
    if (collider.gameObject.layer == Constants.Layers.PlayerProjectiles){
      if (health.Damage(1)){
        Debug.Log($"Boss HP: {health.Health}");
      }
    }
  }
}

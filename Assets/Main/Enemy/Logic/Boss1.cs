using System;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (HealthManager))]
public class Boss1 : MonoBehaviour {
  public DifficultyMode difficultyMode = DifficultyMode.Normal;

  private BossPatternManager<Boss1> patterns;
  [NonSerialized]
  public ProjectileLibrary Projectiles;
  private HealthManager health;
  private Rigidbody2D rigidBody;
  private GameObject player;
  public HealthBar healthBar;


  public GameObject Player {
    get => player;
  }
  public Rigidbody2D rb {
    get => rigidBody;
  }
  public Vector2 eyesPosition {
    get => new Vector2(rb.position.x - 0.15f, rb.position.y + 0.8f);
  }
  public Vector2 mouthPosition {
    get => new Vector2(rb.position.x - 0.15f, rb.position.y + 0.6f);
  }

  void OnEnable(){
    var ss = SessionStorage.GetInstance();
    if (ss.Has(SessionStorage.Keys.GameDifficulty)){
      difficultyMode = ss.Get<DifficultyMode>(SessionStorage.Keys.GameDifficulty);
    }
    rigidBody = GetComponent<Rigidbody2D>();
    Projectiles = GameObject.Find(Constants.GameObjectNames.ProjectileLibrary).GetComponent<ProjectileLibrary>();
    health = GetComponent<HealthManager>();
    player = GameObject.FindWithTag(Constants.Tags.Player);
    health.Reset();
    healthBar.SetMaxHealth(health.OriginalHealth);
    if (patterns == null){
      var basePatterns = new BossPattern<Boss1>[] {
        // Sleeping Phase
        new Boss1Pattern2_SineWave(this),
        new Boss1Pattern3_Missile(this),
        new Boss1Pattern4_SpawnRing(this),
        new Boss1Pattern10_BigZ(this),
        new Boss1Pattern15_Orbit(this),

        // Violent Phase
        new Boss1Pattern7_Punch(this),
        new Boss1Pattern8_BounceCrush(this),
        new Boss1Pattern11_BigFist(this),
        new Boss1Pattern12_Meteors(this),
        new Boss1Pattern13_AirPunch(this),

        // Breakdown Phase
        new Boss1Pattern1_BOWAP(this),
        new Boss1Pattern5_Tunnel(this),
        new Boss1Pattern6_Geyser(this),
        new Boss1Pattern9_Spirals(this),
        new Boss1Pattern14_Rain(this),
      };
      patterns = new BossPatternManager<Boss1>(basePatterns, difficultyMode, patternLength: 20.0f);
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
        healthBar.SetHealth(health.Health);
      }
    }
  }
}

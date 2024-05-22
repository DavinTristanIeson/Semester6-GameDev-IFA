using System;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (HealthManager))]
public class Boss1 : MonoBehaviour {
  public DifficultyMode difficultyMode = DifficultyMode.Normal;

  private BossPhaseManager<Boss1> phases;
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
    if (phases == null){
      var sleepingPhase = new BossPatternManager<Boss1>(
        new BossPattern<Boss1>[] {
          new Boss1Pattern2_SineWave(this),
          new Boss1Pattern3_Missile(this),
          new Boss1Pattern4_SpawnRing(this),
          new Boss1Pattern10_BigZ(this),
          new Boss1Pattern15_Orbit(this)
        },
        difficultyMode,
        20f
      );
      var violentPhase = new BossPatternManager<Boss1>(
        new BossPattern<Boss1>[] {
          new Boss1Pattern7_Punch(this),
          new Boss1Pattern8_BounceCrush(this),
          new Boss1Pattern11_BigFist(this),
          new Boss1Pattern12_Meteors(this),
          new Boss1Pattern13_AirPunch(this),
        },
        difficultyMode,
        20f
      );
      var breakdownPhase = new BossPatternManager<Boss1>(
        new BossPattern<Boss1>[] {
           new Boss1Pattern1_BOWAP(this),
            new Boss1Pattern5_Tunnel(this),
            new Boss1Pattern6_Geyser(this),
            new Boss1Pattern9_Spirals(this),
            new Boss1Pattern14_Rain(this),
        },
        difficultyMode,
        20f
      );
      var finalPhase = new BossPatternManager<Boss1>(
        new BossPattern<Boss1>[] {
          new Boss1PatternFinal(this),
        },
        difficultyMode,
        60f
      );
      phases = new BossPhaseManager<Boss1>(new BossPhase<Boss1>[] {
        new BossPhase<Boss1>(sleepingPhase, health.HealthWhen(1)),
        new BossPhase<Boss1>(violentPhase, health.HealthWhen(0.6666f)),
        new BossPhase<Boss1>(breakdownPhase, health.HealthWhen(0.3333f)),
        new BossPhase<Boss1>(finalPhase, health.HealthWhen(0))
      });
    }
  }

  void OnDisable(){
    if (phases != null){
      phases.Destroy(this);
    }
  }

  void Update(){
    phases.Execute(this, health.Health);
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

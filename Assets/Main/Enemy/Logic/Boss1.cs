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

  private float finalPhaseStartTime = float.MaxValue;
  void OnSetFinalPhase(){
    finalPhaseStartTime = Time.time;
    healthBar.SetMaxHealth(60);
    var playerHealth = player.GetComponent<HealthManager>();
    var playerHealthbar = GameObject.FindWithTag(Constants.Tags.Hurtbox).GetComponent<PlayerHurtbox>().healthBar;
    player.GetComponentInChildren<GunController>().gameObject.SetActive(false);
    playerHealth.Reset();
    playerHealthbar.SetMaxHealth(playerHealth.Health);

    GameObject.Find(Constants.GameObjectNames.Camera).GetComponent<PlayerStateBasedCameraEffects>().StartFinalPhaseSequence();
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
    player.GetComponentInChildren<GunController>().gameObject.SetActive(true);
    health.Reset();
    healthBar.SetMaxHealth(health.OriginalHealth);
    var sleepingPhase = new BossPhase<Boss1>(new BossPatternManager<Boss1>(
      new BossPattern<Boss1>[] {
        new Boss1Pattern2_SineWave(this),
        new Boss1Pattern3_Missile(this),
        new Boss1Pattern4_SpawnRing(this),
        new Boss1Pattern10_BigZ(this),
        new Boss1Pattern15_Orbit(this)
      },
      difficultyMode,
      20f
    ), health.HealthWhen(1f));
    var violentPhase = new BossPhase<Boss1>(new BossPatternManager<Boss1>(
      new BossPattern<Boss1>[] {
        new Boss1Pattern7_Punch(this),
        new Boss1Pattern8_BounceCrush(this),
        new Boss1Pattern11_BigFist(this),
        new Boss1Pattern12_Meteors(this),
        new Boss1Pattern13_AirPunch(this),
      },
      difficultyMode,
      20f
    ), health.HealthWhen(0.6666f));
    var breakdownPhase = new BossPhase<Boss1>(new BossPatternManager<Boss1>(
      new BossPattern<Boss1>[] {
          new Boss1Pattern1_BOWAP(this),
          new Boss1Pattern5_Tunnel(this),
          new Boss1Pattern6_Geyser(this),
          new Boss1Pattern9_Spirals(this),
          new Boss1Pattern14_Rain(this),
      },
      difficultyMode,
      20f
    ), health.HealthWhen(0.3333f));
    
    BossPhase<Boss1> finalPhase = new BossPhase<Boss1>(new BossPatternManager<Boss1>(
        new BossPattern<Boss1>[] {
          new Boss1PatternFinal(this),
        },
        difficultyMode,
        60f
      ), 0) {
      Before = OnSetFinalPhase,
      Timeout = 60f
    };
    if (difficultyMode == DifficultyMode.Casual){
      finalPhase.Timeout = 0.2f;
      finalPhase.Before = null;
    }
    BossPhase<Boss1>[] phaseArray = new BossPhase<Boss1>[] {
      sleepingPhase,
      violentPhase,
      breakdownPhase,
      finalPhase
    };
    phases = new BossPhaseManager<Boss1>(phaseArray) {
      OnEnd = EndGame
    };
    finalPhaseStartTime = float.MaxValue;
  }

  void OnDisable(){
    if (phases != null){
      phases.Destroy(this);
    }
  }

  void EndGame(){
    Debug.Log("Game end");
  }

  void Update(){
    phases.Execute(this, health.Health);
    if (finalPhaseStartTime < Time.time){
      healthBar.SetHealth(Mathf.Max(0f, 60f - (Time.time - finalPhaseStartTime)));
    }
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

namespace Constants {
  public sealed class ZAxis {
    public const float Bullet = 5f;
    public const float EnemyBullet = -2f;
    public const float Player = 0f;
    public const float Gun = -0.2f;
    public const float Enemy = -1f;
  }
  public sealed class Layers {
    public const int Player = 3;
    public const int Enemy = 6;
    public const int PlayerProjectiles = 7;
    public const int EnemyProjectiles = 8;
  }
  public sealed class Prefabs {
    public const string DefaultEnemyProjectile = "Assets/Main/Projectiles/EnemyProjectile.prefab";
    public const string EnemyEffectorProjectile = "Assets/Main/Projectiles/EnemyEffectorProjectile.prefab";
  }
  public sealed class Tags {
    public const string Hurtbox = "Hurtbox";
    public const string Player = "Player";
    public const string Boss = "Boss";
  }
  public sealed class GameObjectNames {
    public const string ProjectileLibrary = "ProjectileLibrary";
    public const string Camera = "Main Camera";
    public const string PauseMenu = "PauseMenu";
    public const string BackgroundMusicManager = "BackgroundMusicManager";
    public const string SfxManager = "SfxManager";
    public const string Hurtbox = "Hurtbox";
  }
  public sealed class MusicAssetNames {
    public const string Battle = "Battle";
    public const string GameOver = "GameOver";
    public const string FinalPhase = "FinalPhase";
    // Sfx
    public const string HealthPickup = "HealthPickup";
    public const string HealthDamage = "HealthDamage";
    public const string Gun = "Gun";
  }
  namespace AnimationStates {
    public sealed class Player {
      public const string Run = "Run";
      public const string Idle = "Idle";
      public const string Angry = "Angry";
      // Parameter

      public const string ParamFloatSprint = "Sprint";
    }
    public sealed class Boss {
      public const string WakeToSleep = "Wake To Sleep";
      public const string Sleep = "Sleep";
      public const string Idle = "Idle";
      public const string SleepToAngry = "Sleep To Angry";
      public const string Cry = "Cry";
    }
  }
}

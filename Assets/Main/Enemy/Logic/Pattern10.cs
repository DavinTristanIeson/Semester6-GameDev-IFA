using System;
using System.Linq;
using UnityEngine;
#nullable enable

class Boss1Pattern10_BigZ : BossPattern<Boss1>{
  GameObjectPool pool;
  Boss1 boss;
  CooldownTimer cooldown;

  public override void Start(Boss1 caller){
    cooldown.WaitTime = Difficulty switch {
      DifficultyMode.Casual => 3f,
      DifficultyMode.Normal => 2f,
      DifficultyMode.Challenge => 1f,
      _ => 1f,
    };
  }
  public override void Execute(Boss1 caller){
    int elementsInLine = Difficulty switch {
      DifficultyMode.Casual => 8,
      DifficultyMode.Normal => 12,
      DifficultyMode.Challenge => 16,
      _ => 11,
    };
    var centerX = boss.mouthPosition.x;
    var playerRb = boss.Player.GetComponent<Rigidbody2D>();
    if (cooldown.Try()){
      GameObject[] horizontalLines = pool.GetMany(elementsInLine * 2);
      GameObject[] diagonalLines = pool.GetMany(elementsInLine);
      
      float xLeft = centerX - 0.2f;
      float xRight = centerX;
      float topY = boss.mouthPosition.y + 2f;
      float bottomY = boss.mouthPosition.y - 2f;

      Rigidbody2D topRight = horizontalLines[horizontalLines.Length - 2].GetComponent<Rigidbody2D>();
      Rigidbody2D bottomLeft = horizontalLines[horizontalLines.Length - 3].GetComponent<Rigidbody2D>();
      Vector3 scale = new Vector3(1.5f, 1.5f, 1.5f);
      for (int i = 0; i < elementsInLine * 2; i++){
        var go = horizontalLines[i];
        go.transform.localScale = scale;
        var rb = go.GetComponent<Rigidbody2D>();
        var y = i % 2 == 0 ? topY : bottomY;
        if (Math.Floor(i / 2f) % 2 == 0){
          rb.position = new Vector2(xLeft, y);
          xLeft -= 0.2f;
        } else {
          rb.position = new Vector2(xRight, y);
          xRight += 0.2f;
        }
        go.name = $"{go.name} {(i % 2 == 0 ? "Top" : "Bottom")}";
      }

      for (int i = 0; i < elementsInLine; i++){
        var go = diagonalLines[i];
        go.transform.localScale = scale;
        var rb = go.GetComponent<Rigidbody2D>();
        float xOffset = Math.Abs(topRight.position.x - bottomLeft.position.x) * i / elementsInLine;
        float yOffset = Math.Abs(topRight.position.y - bottomLeft.position.y) * i / elementsInLine;
        rb.position = bottomLeft.position + new Vector2(xOffset, yOffset);
      }
      float diagonalAngle = Calculate.Vector.AngleTowards(bottomLeft.position, topRight.position);

      var angle = Calculate.Vector.AngleTowards(boss.mouthPosition, playerRb.position);
      var reverseYinyang = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
      ProjectileBehavior.GetPositionFn CalculateHorizontalLinesHoming = (GameObject go) => {
        float sumX = 0f;
        float sumY = 0f;
        foreach (var horizontalLineItem in horizontalLines){
          var pos = horizontalLineItem.GetComponent<Rigidbody2D>().position;
          sumX += pos.x;
          sumY += pos.y;
        }
        var center = new Vector2(sumX / horizontalLines.Length, sumY / horizontalLines.Length);
        var offset = Calculate.Vector.WithAngle(Calculate.Vector.AngleTowards(center, playerRb.position));
        return center + offset * (go.name.EndsWith("Top") ? -2f : 2f) * reverseYinyang;
      };

      ScriptableAction<GameObject> CalculateDiagonalLinesHoming = (GameObject go) => {
        float sumX = 0f;
        float sumY = 0f;
        foreach (var diagonalLineItem in diagonalLines){
          var pos = diagonalLineItem.GetComponent<Rigidbody2D>().position;
          sumX += pos.x;
          sumY += pos.y;
        }
        var center = new Vector2(sumX / horizontalLines.Length, sumY / horizontalLines.Length);
        float angle = Calculate.Vector.AngleTowards(center, playerRb.position);
        foreach (var diagonalLineItem in diagonalLines){
          var rb = diagonalLineItem.GetComponent<Rigidbody2D>();
          rb.MovePosition(Calculate.Vector.RotateAround(
            rb.position,
            center,
            angle + diagonalAngle
          ));
          rb.MoveRotation(angle);
        }
        go.GetComponent<BehaviorManager>().GetBehaviorOfType<ProjectileBehavior.Timing>()?.Next(go);
      };

      foreach (var go in horizontalLines){
        var rb = go.GetComponent<Rigidbody2D>();
        rb.position = Calculate.Vector.RotateAround(rb.position, boss.mouthPosition, angle);
        rb.rotation = angle;
        go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
          .Then(new ProjectileBehavior.Propulsion(6f), 1f)
          .Then(new ProjectileBehavior.Homing(CalculateHorizontalLinesHoming!) {
            Once = true,
          })
          .Then(new ProjectileBehavior.Propulsion(6f));
      }
      foreach (var go in diagonalLines){
        var rb = go.GetComponent<Rigidbody2D>();
        rb.position = Calculate.Vector.RotateAround(rb.position, boss.mouthPosition, angle);
        rb.rotation = angle;
        go.GetComponent<BehaviorManager>().Behavior = new ProjectileBehavior.Timing(3)
          .Then(new ProjectileBehavior.Propulsion(6f), 1f)
          .Then(new ProjectileBehavior.Custom(CalculateDiagonalLinesHoming!))
          .Then(new ProjectileBehavior.Acceleration(1f, 0.02f));
      }
    }
  }

  public override void Destroy(Boss1 caller){
    pool.Destroy();
  }

  public override void Deactivate(Boss1 caller){
    pool.Revoke();
  }

  public Boss1Pattern10_BigZ(Boss1 boss){
    this.boss = boss;
    var blueprint = boss.Projectiles.Get(ProjectileType.Regular);
    pool = new GameObjectPool(blueprint) {
      Parent = ProjectileLibrary.CreateContainer("Boss1 Pattern10 BigZ"),
    };
    cooldown = new CooldownTimer(2f);
  }
}
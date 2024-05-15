#nullable enable
using System;
using UnityEngine;

class BoundaryInformation {
  public float x0 {get; private set; }
  public float y0 {get; private set; }
  public float x1 {get; private set; }
  public float y1 {get; private set; }

  private static BoundaryInformation? instance;
  public static BoundaryInformation GetInstance(){
    if (instance is not null){
      return instance;
    }
    instance = new BoundaryInformation();
    // https://forum.unity.com/threads/keeping-the-player-within-the-boundaries-of-the-camera.245693/
    var dist = 0 - Camera.main.transform.position.z;
    instance.x0 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
    instance.x1 = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
    instance.y0 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
    instance.y1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
    return instance;
  }

  public bool IntersectsWithBounds(Bounds bounds){
    var pos = bounds.center;
    // We extend the point position to the borders and check if that point intersects with the bounds or not.
    return bounds.Contains(new Vector3(x0, pos.y, pos.z)) ||
      bounds.Contains(new Vector3(x1, pos.y, pos.z)) ||
      bounds.Contains(new Vector3(pos.x, y0, pos.z)) ||
      bounds.Contains(new Vector3(pos.x, y1, pos.z));
  }

  public Vector2 Clamp(Vector2 position){
    return new Vector2(
      Mathf.Clamp(position.x, x0, x1),
      Mathf.Clamp(position.y, y0, y1)
    );
  }

  public float Height { get => Math.Abs(y0 - y1); }
  public float Width { get => Math.Abs(x0 - x1); }

  public float GetRandomY(float lowerBound = 0.0f, float upperBound = 1.0f){
    float y0 = this.y0 + lowerBound * Height;
    float y1 = this.y0 + upperBound * Height;
    return UnityEngine.Random.Range(y0, y1);
  }
  public float GetRandomX(float lowerBound = 0.0f, float upperBound = 1.0f){
    float x0 = this.x0 + lowerBound * Width;
    float x1 = this.x0 + upperBound * Width;
    return UnityEngine.Random.Range(x0, x1);
  }

  public bool IsPointOnBoundary(Vector2 point){
    return point.x == x0 || point.x == x1 || point.y == y0 || point.y == y1;
  }
}
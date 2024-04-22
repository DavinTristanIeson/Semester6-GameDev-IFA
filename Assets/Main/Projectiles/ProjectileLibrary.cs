using UnityEngine;

public enum ProjectileType {
  Tear,
  Yawn,
  Sleep,
  Regular,
  Player,
  Effector,
}

public class ProjectileLibrary : MonoBehaviour {
  public GameObject Tear;
  public GameObject Yawn;
  public GameObject Sleep;
  public GameObject Regular;
  public GameObject Player;
  public GameObject Effector;

  void OnEnable(){
    Tear.SetActive(false);
    Yawn.SetActive(false);
    Sleep.SetActive(false);
    Regular.SetActive(false);
    Player.SetActive(false);
  }
  public GameObject Get(ProjectileType type){
    return type switch {
      ProjectileType.Tear => Tear,
      ProjectileType.Yawn => Yawn,
      ProjectileType.Sleep => Sleep,
      ProjectileType.Effector => Effector,
      ProjectileType.Player => Player,
      _ => Regular,
    };
  }
  public static GameObject CreateContainer(string name){
    var go = new GameObject(name);
    go.transform.Translate(0, 0, Constants.ZAxis.EnemyBullet);
    return go;
  }
}
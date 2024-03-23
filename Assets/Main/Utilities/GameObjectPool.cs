using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

#nullable enable

// https://medium.com/@HMarcusWilliamson/how-to-object-pooling-with-unitys-objectpool-class-aa41dfb1bdad
public class GameObjectPool {
  public GameObject Blueprint;
  public ScriptableAction<GameObject>? Transform;

  private ObjectPool<GameObject> pool;
  public GameObject? Parent;
  

  GameObject OnCreate(){
    var go = GameObject.Instantiate(Blueprint);
    go.SetActive(false);
    go.AddComponent<GameObjectReturnToPool>();
    go.GetComponent<GameObjectReturnToPool>().Pool = this;
    return go;
  }
  void OnGet(GameObject obj){
    if (Parent is GameObject parent){
      obj.transform.parent = parent.transform;
    }
    if (Transform is ScriptableAction<GameObject> tf){
      tf(obj);
    }
    obj.SetActive(true);
  }
  void OnRelease(GameObject obj){
    obj.SetActive(false);
  }
  void OnDestroy(GameObject obj){
    GameObject.Destroy(obj);
  }

  public GameObjectPool(GameObject blueprint, int size){
    Blueprint = blueprint;
    blueprint.SetActive(false);
    pool = new ObjectPool<GameObject>(
      OnCreate,
      actionOnGet: OnGet,
      actionOnRelease: OnRelease,
      actionOnDestroy: OnDestroy,
      defaultCapacity: size,
      maxSize: size
    );
  }

  public GameObjectPool(
    GameObject blueprint,
    int poolMin,
    int poolMax
  ){
    Blueprint = blueprint;
    pool = new ObjectPool<GameObject>(
      OnCreate,
      actionOnGet: OnGet,
      actionOnRelease: OnRelease,
      actionOnDestroy: OnDestroy,
      defaultCapacity: poolMin,
      maxSize: poolMax
    );
  }
  ~GameObjectPool(){
    pool.Dispose();
  }

  public GameObject Get(){
    return pool.Get();
  }
  public GameObject[] GetMany(int count){
    GameObject[] list = new GameObject[count];
    for (int i = 0; i < count; i++){
      list[i] = pool.Get();
    }
    return list;
  }
  public void Release(GameObject obj){
    pool.Release(obj);
  }

  public void Destroy(){
    pool.Dispose();
    if (Parent is GameObject parent){
      GameObject.Destroy(parent);
    }
  }
}

class GameObjectReturnToPool : MonoBehaviour {
  public GameObjectPool? Pool;
  void OnDisable(){
    if (Pool is GameObjectPool p){
      p.Release(gameObject);
    }
  }
}
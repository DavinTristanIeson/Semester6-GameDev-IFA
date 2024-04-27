using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

#nullable enable

// https://medium.com/@HMarcusWilliamson/how-to-object-pooling-with-unitys-objectpool-class-aa41dfb1bdad
public class GameObjectPool {
  public GameObject Blueprint;
  public ScriptableAction<GameObject>? Transform;

  private ObjectPool<GameObject> pool;
  private HashSet<GameObject> outgoing;
  public GameObject? Parent;
  

  GameObject OnCreate(){
    var go = GameObject.Instantiate(Blueprint, new Vector3(-1000.0f, 0.0f, Blueprint.transform.position.z), Quaternion.identity);
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
    outgoing.Add(obj);
  }
  void OnRelease(GameObject obj){
    obj.SetActive(false);
    outgoing.Remove(obj);
  }
  void OnDestroy(GameObject obj){
    GameObject.Destroy(obj);
    outgoing.Remove(obj);
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
      maxSize: size,
      collectionCheck: false
    );
    outgoing = new HashSet<GameObject>();
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
      maxSize: poolMax,
      collectionCheck: false
    );
    outgoing = new HashSet<GameObject>();
  }
  ~GameObjectPool(){
    pool.Dispose();
    outgoing.Clear();
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

  public void Revoke(){
    foreach (GameObject go in outgoing.ToArray()){
      go.transform.Translate(new Vector3(-1000f, 0f, 0f));
      Release(go);
    }
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
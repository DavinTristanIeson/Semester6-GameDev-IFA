using UnityEngine;

#nullable enable

// https://learn.unity.com/tutorial/introduction-to-object-pooling#5ff8d015edbc2a002063971c

/// <summary>
/// Utility class for object pooling. Consider using Unity's ObjectPool (need to find a way to properly call Release)
/// </summary>
public class GameObjectPool {
  /// <summary>
  /// Group each spawned game object inside a parent container to tidy up the hierarchy
  /// </summary>
  private GameObject? parent;

  /// <summary>
  /// Blueprint of object to be instantiated
  /// </summary>
  private GameObject blueprint;

  /// <summary>
  /// Actual pool/cache object. Decided to use array rather than ObjectPool so the GameObjects don't need to have access to a Release method
  /// </summary>
  private readonly GameObject[] pool;
  readonly int poolSize;

  /// <summary>
  /// Previously used objects.
  /// </summary>
  private UniqueQueue<int> usedObjects;
  public ScriptableAction<GameObject>? Transform;
  public GameObjectPool(
    GameObject blueprint,
    int poolSize = 20
  ){
    this.blueprint = blueprint;
    this.poolSize = poolSize;
    blueprint.SetActive(false);
    pool = new GameObject[poolSize];
    for (int i = 0; i < poolSize; i++){
      pool[i] = Create();
    }
    usedObjects = new UniqueQueue<int>(poolSize + 1);
  }

  private GameObject Create(){
    var cur = GameObject.Instantiate(blueprint);
    cur.SetActive(false);
    if (parent is GameObject father){
      cur.transform.parent = father.transform;
    }
    return cur;
  }

  public GameObject? ParentContainer {
    get => parent;
    set {
      if (value is GameObject go){
        if (!go.activeInHierarchy){
          go.SetActive(true);
        }
        for (int i = 0; i < poolSize; i++){
          pool[i].transform.parent = go.transform;
        }
      }
    }
  }

  /// <summary>
  /// Gets a GameObject from the pool. This reuses old active game objects if there are no more spare game objects in the pool.
  /// </summary>
  /// <returns></returns>
  public GameObject Get(){
    for (int i = 0; i < poolSize; i++){
      GameObject cur = pool[i];
      if (cur == null){
        cur = Create();
        pool[i] = cur;
      }
      if (cur.activeInHierarchy){
        continue;
      }
      activate(cur, i);
      return cur;
    }
    int fallbackIndex = usedObjects.IsNotEmpty() ? usedObjects.Dequeue() : 0;
    GameObject fallback = pool[fallbackIndex];
    activate(fallback, fallbackIndex);
    return fallback;
  }

  /// <summary>
  /// Gets many GameObjects from the pool. This reuses old active game objects if there are no more spare game objects in the pool.
  /// ``count`` should not be higher than ``poolSize``
  /// </summary>
  /// <returns></returns>
  public GameObject[] GetMany(int count){
    GameObject[] arr = new GameObject[count];
    int index = 0;

    for (int i = 0; i < poolSize; i++){
      GameObject cur = pool[i];
      if (cur == null){
        cur = Create();
        pool[i] = cur;
      }
      if (cur.activeInHierarchy){
        continue;
      }
      activate(cur, i);
      arr[index] = cur;
      index++;
      if (index == count){
        return arr;
      }
    }

    while (index < count && usedObjects.IsNotEmpty()){
      int fallbackIndex = usedObjects.Dequeue();
      GameObject fallback = pool[fallbackIndex];
      activate(fallback, fallbackIndex);
      arr[index] = fallback;
      index++;
    }
    return arr;
  }

  public void Destroy(){
    for (int i = 0; i < poolSize; i++){
      if (pool[i] == null) continue;
      pool[i].SetActive(false);
    }
    for (int i = 0; i < poolSize; i++){
      if (pool[i] == null) continue;
      GameObject.Destroy(pool[i]);
    }
    if (parent != null){
      GameObject.Destroy(parent);
    }
  }

  public void Deactivate(){
    for (int i = 0; i < poolSize; i++){
      if (pool[i] != null){
        pool[i].SetActive(false);
      }
    }
  }

  
  private void activate(GameObject cur, int i){
    cur.SetActive(true);
    if (Transform is ScriptableAction<GameObject> tf){
      tf(cur);
    }
    usedObjects.Enqueue(i);
  }
}

// https://medium.com/@HMarcusWilliamson/how-to-object-pooling-with-unitys-objectpool-class-aa41dfb1bdad
// public class GameObjectPool {
//   public GameObject Blueprint;
//   public GameObjectTransformFn? Transform;

//   private ObjectPool<GameObject> pool;
//   private int poolMin;
//   private int poolMax;
//   private GameObject? parent;
  

//   GameObject OnCreate(){
//     return GameObject.Instantiate(Blueprint);
//   }
//   void OnGet(GameObject obj){
//     obj.SetActive(true);
//     if (Transform is GameObjectTransformFn tf){
//       tf(obj);
//     }
//   }
//   void OnRelease(GameObject obj){
//     obj.SetActive(false);
//   }
//   void OnDestroy(GameObject obj){
//     GameObject.Destroy(obj);
//   }

//   public GameObjectPool(
//     GameObject blueprint,
//     GameObject? parent = null,
//     GameObjectTransformFn? transform = null,
//     int poolMin = 10,
//     int poolMax = 100
//   ){
//     Blueprint = blueprint;
//     Transform = transform;
//     this.parent = parent;
//     pool = new ObjectPool<GameObject>(
//       OnCreate,
//       actionOnGet: OnGet,
//       actionOnRelease: OnRelease,
//       actionOnDestroy: OnDestroy,
//       defaultCapacity: poolMin,
//       maxSize: poolMax
//     );
//   }

//   public GameObject Get(){
//     return pool.Get();
//   }
//   public void Release(GameObject obj){
//     pool.Release(obj);
//   }

//   public void Destroy(){
//     pool.Dispose();
//     GameObject.Destroy(parent);
//   }
// }
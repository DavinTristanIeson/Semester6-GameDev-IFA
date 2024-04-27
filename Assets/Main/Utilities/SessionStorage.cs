using System.Collections.Generic;

#nullable enable
class SessionStorage {
  public enum Keys {
    GameDifficulty,
  }

  static SessionStorage? instance;
  Dictionary<Keys, object> storage;

  SessionStorage(){
    storage = new Dictionary<Keys, object>();
  }
  public void Set(Keys key, object o){
    storage.Add(key, o);
  }
  public void Remove(Keys key){
    storage.Remove(key);
  }
  public bool Has(Keys key){
    return storage.ContainsKey(key);
  }
  public T Get<T>(Keys key) {
    return (T)storage[key];
  }

  public static SessionStorage GetInstance(){
    if (instance is SessionStorage ss){
      return ss;
    }
    instance = new SessionStorage();
    return instance;
  }
}
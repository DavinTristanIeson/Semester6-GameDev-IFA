using System.Collections.Generic;

#nullable enable

/// <summary>
/// Queue that can only hold unique values. New values that are not unique will not be enqueued.
/// </summary>
/// <typeparam name="T"></typeparam>
class UniqueQueue<T> {
  Queue<T> queue;
  HashSet<T> uniques;
  int? queueLimit = null;

  public UniqueQueue(int? queueLimit = null){
    queue =  queueLimit is int limit ? new Queue<T>(limit) : new Queue<T>();
    this.queueLimit = queueLimit;
    uniques = new HashSet<T>();
  }

  public bool IsNotEmpty(){
    return queue.Count > 0;
  }

  public void Enqueue(T item){
    if (uniques.Contains(item)){
      return;
    }
    queue.Enqueue(item);
    uniques.Add(item);
    if (queue.Count >= queueLimit){
      Dequeue();
    }
  }

  public T Dequeue(){
    T item = queue.Dequeue();
    uniques.Remove(item);
    return item;
  }


  public int Count {
    get => queue.Count;
  }
}
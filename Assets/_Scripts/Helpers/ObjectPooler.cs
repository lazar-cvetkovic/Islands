using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    private Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new();

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!_poolDictionary.ContainsKey(prefab))
        {
            _poolDictionary[prefab] = new Queue<GameObject>();
        }

        if (_poolDictionary[prefab].Count > 0)
        {
            GameObject obj = _poolDictionary[prefab].Dequeue();
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            return obj;
        }
    }

    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        foreach (var kvp in _poolDictionary)
        {
            if (kvp.Key.name == obj.name.Replace("(Clone)", "").Trim())
            {
                _poolDictionary[kvp.Key].Enqueue(obj);
                return;
            }
        }

        _poolDictionary[obj] = new Queue<GameObject>();
        _poolDictionary[obj].Enqueue(obj);
    }
}

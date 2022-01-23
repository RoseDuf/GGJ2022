using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private PoolableObject _prefab;
    private List<PoolableObject> _availableobjects;

    private ObjectPool(PoolableObject prefab, int size)
    {
        _prefab = prefab;
        _availableobjects = new List<PoolableObject>(size);
    } 

    public static ObjectPool CreateInstance(PoolableObject prefab, int size)
    {
        ObjectPool pool = new ObjectPool(prefab, size);

        GameObject poolobject = new GameObject(prefab.name + "_Pool");
        pool.CreateObjects(poolobject.transform, size);

        return pool;
    }

    private void CreateObjects(Transform parent, int size)
    {
        for(int i = 0; i < size; i++)
        {
            PoolableObject poolableObject = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false);
        }
    }

    public void ReturnobjectToPool(PoolableObject poolableObject)
    {
        _availableobjects.Add(poolableObject);
    }

    public PoolableObject GetObject()
    {
        if (_availableobjects.Count > 0)
        {
            PoolableObject instance = _availableobjects[0];
            _availableobjects.RemoveAt(0);

            instance.gameObject.SetActive(true);

            return instance;
        }
        else
        {
            Debug.LogWarning($"Could not get object from pool {_prefab.name}.");
            return null;
        }
    }
}

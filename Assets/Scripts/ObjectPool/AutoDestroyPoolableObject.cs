using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyPoolableObject : PoolableObject
{
    private const string k_DisableMethodName = "Disable";
    public float _autoDestroyTime = 5f;

    public virtual void OnEnable()
    {
        CancelInvoke(k_DisableMethodName);
        Invoke(k_DisableMethodName, _autoDestroyTime);
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }
}

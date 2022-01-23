using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerDatabase : MonoBehaviour
{
    private static VillagerDatabase _instance;

    public static VillagerDatabase Instance { get { return _instance; } }

    [SerializeField]
    private List<VillagerScriptableObject> _villagerData;

    public List<VillagerScriptableObject> VillagerData { get { return _villagerData; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}

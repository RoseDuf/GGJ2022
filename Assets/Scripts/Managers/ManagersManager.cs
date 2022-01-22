using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Managers = new List<GameObject>();

    private void Awake()
    {
        foreach(GameObject manager in Managers)
        {
            GameObject managerInstance = Instantiate(manager, transform.position, transform.rotation, transform);
        }
    }

}

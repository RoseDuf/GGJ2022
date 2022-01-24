using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetScope : MonoBehaviour
{
    public List<IDamageable> Damageables = new List<IDamageable>();
    public List<Transform> TargetList = new List<Transform>();
    [SerializeField]
    private float _stopDistance;
    [SerializeField]
    private Transform _player;

    public float StopDistance { get { return _stopDistance; } }
    

    private void OnTriggerStay(Collider collider)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null && !Damageables.Contains(damageable))
        {
            Damageables.Add(damageable);
            TargetList.Add(damageable.GetTransform());
        }
    }
    
    private void OnTriggerExit(Collider collider)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Damageables.Remove(damageable);
            TargetList.Remove(damageable.GetTransform());
            Villager villager = collider.GetComponent<Villager>();

            if (villager != null)
            {
                villager.UIArrow.ShowArrow(false);
            }
        }
    }
}

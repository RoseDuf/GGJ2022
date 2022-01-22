using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetScope : MonoBehaviour
{
    private List<Collider> _targetList;
    public List<Collider> TargetList { get { return _targetList; } }
    [SerializeField]
    private float _stopDistance;
    [SerializeField]
    private Transform _player;

    public float StopDistance { get { return _stopDistance; } }

    private void Start()
    {
        _targetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Target" && (collider.transform.position - _player.position).magnitude > 1 && !_targetList.Contains(collider))
        {
            _targetList.Add(collider.GetComponent<Collider>());
        }
    }
    
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Target" && _targetList.Contains(collider))
        {
            _targetList.Remove(collider);

            Villager villager = collider.GetComponent<Villager>();

            if (villager != null)
            {
                villager.UIArrow.ShowArrow(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class VillagerLineOfSightChecker : MonoBehaviour
{
    [SerializeField]
    private SphereCollider _collider;
    [SerializeField]
    private float _fieldOfView = 90f;
    [SerializeField]
    private LayerMask _lineOfSightLayers;

    public delegate void GainSightEvent(Player player);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent();
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLineOfSightoroutine;
    private Player playerTarget;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerTarget == null)
            {
                playerTarget = other.GetComponent<Player>();
                OnGainSight?.Invoke(playerTarget);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerTarget = null;
            OnLoseSight?.Invoke();
        }
    }

    //private void CheckLineOfSight(Player player)
    //{
    //    OnGainSight?.Invoke(player);
    //}

    //private IEnumerator CheckForLineOfSight(Player player)
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.1f);

    //    while (!CheckLineOfSight(player))
    //    {
    //        yield return wait;
    //    }
    //}
}

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
    public delegate void LoseSightEvent(Player player);
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLineOfSightoroutine;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player;
            if (other.TryGetComponent<Player>(out player))
            {
                if (!CheckLineOfSight(player))
                {
                    CheckForLineOfSightoroutine = StartCoroutine(CheckForLineOfSight(player));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player;
            if (other.TryGetComponent<Player>(out player))
            {
                OnLoseSight?.Invoke(player);
                if (CheckForLineOfSightoroutine != null)
                {
                    StopCoroutine(CheckForLineOfSightoroutine);
                }
            }
        }
    }

    private bool CheckLineOfSight(Player player)
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        //if (Vector3.Dot(transform.forward, direction) >= Mathf.Cos(_fieldOfView))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, _collider.radius, _lineOfSightLayers))
            {
                if (hit.transform.GetComponentInParent<Player>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(Player player)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}

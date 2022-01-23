using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class VillagerMovement : MonoBehaviour
{
    [SerializeField]
    private float UpdateRate = 0.1f;
    [SerializeField]
    private NavMeshAgent Agent;
    [SerializeField]
    private float IdleLocationRadius = 4f;
    [SerializeField]
    private float IdleMovespeedMultiplier = 0.5f;

    [SerializeField]
    private Animator Animator = null;

    [SerializeField]
    private VillagerState DefaultState;
    private VillagerState _state;
    public VillagerState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }
    public Transform Target { get; set; }

    public delegate void StateChangeEvent(VillagerState oldState, VillagerState newState);
    public StateChangeEvent OnStateChange;

    private const string IsWalking = "IsWalking";
    private const string IsWimpering = "IsWimpering";
    private const string IsAttacking = "IsAttacking";

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        OnStateChange += HandleStateChange;
    }

    private void OnDisable()
    {
        _state = DefaultState;
    }

    private void Update()
    {
        
    }

    public void Spawn()
    {
        OnStateChange?.Invoke(VillagerState.Spawn, DefaultState);
    }

    //private void StartChasing()
    //{
    //    if (FollowCoroutine == null)
    //    {
    //        FollowCoroutine = StartCoroutine(FollowTarget());
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Called StartChasing on Enemy tht is already chasing!");
    //    }
    //}

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);

        Agent.speed *= IdleMovespeedMultiplier;

        while (true)
        {
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return wait;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);

        while (gameObject.activeSelf)
        {
            if (Agent.enabled)
            {
                Agent.SetDestination(Target.position);
            }
            yield return wait;
        }
    }

    private void HandleStateChange(VillagerState oldSatte, VillagerState newState)
    {
        if (oldSatte != newState)
        {
            if (FollowCoroutine != null)
            {
                StopCoroutine(FollowCoroutine);
            }

            if (oldSatte == VillagerState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
            }

            switch (newState)
            {
                case VillagerState.Idle:
                    FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case VillagerState.Chase:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }
}

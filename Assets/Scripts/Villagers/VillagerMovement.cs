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
    private float IdleMovespeed = 2f;
    [SerializeField]
    private float RunMoveSpeed = 4f;
    [SerializeField]
    private VillagerLineOfSightChecker _lineOfSightChecker;

    [SerializeField]
    private Animator _animator = null;

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
    private bool isWalking;
    private const string IsRunningAway = "IsRunningAway";
    private const string IsAttacking = "IsAttacking";

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        _lineOfSightChecker.OnGainSight = HandleGainSight;
        _lineOfSightChecker.OnLoseSight = HandleLoseSight;
        OnStateChange += HandleStateChange;
    }

    private void OnDisable()
    {
        _state = DefaultState;
    }

    Vector3 curPos;
    Vector3 lastPos;
    private void Update()
    {
        Vector3 curPos = transform.position;
        if (curPos == lastPos && isWalking)
        {
            isWalking = false;
        }
        else if (curPos != lastPos && !isWalking)
        {
            isWalking = true;
            
        }
        _animator.SetBool("IsWalking", isWalking);
        _animator.SetBool("IsIdle", !isWalking);
        lastPos = curPos;
    }

    private void HandleGainSight(Player player)
    {
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (GetComponent<Villager>().Aggressivity == 0)
            {
                State = VillagerState.RunAway;
            }
            else
            {
                State = VillagerState.Chase;
            }
        }
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }

    public void Spawn()
    {
        OnStateChange?.Invoke(VillagerState.Spawn, DefaultState);
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);

        Agent.speed = IdleMovespeed;
        Agent.isStopped = true;
        Agent.ResetPath();

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

        if (Agent.enabled)
        {
            Agent.speed = RunMoveSpeed;
            Agent.isStopped = true;
            Agent.ResetPath();
        }

        while (gameObject.activeSelf)
        {
            if (Agent.enabled)
            {
                Agent.SetDestination(Target.position - (Target.position - transform.position).normalized * 0.5f);
            }
            yield return wait;
        }
    }

    private IEnumerator RunAwayFromTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);

        Agent.speed = RunMoveSpeed;
        Agent.isStopped = true;
        Agent.ResetPath();

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

            switch (newState)
            {
                case VillagerState.Idle:
                    FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case VillagerState.Chase:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
                case VillagerState.RunAway:
                    FollowCoroutine = StartCoroutine(RunAwayFromTarget());
                    break;
            }
        }
    }
    
}

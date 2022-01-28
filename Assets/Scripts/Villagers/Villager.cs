using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.AI;

public class Villager : PoolableObject, IDamageable
{
    public enum FoodType
    {
        Apple,
        Fish,
        Cake,
        Cheese
    }

    public VillagerMovement Movement;
    public NavMeshAgent Agent;

    public UIArrow UIArrow { get; set; }

    [SerializeField]
    private FoodType _typeOfFood;
    [SerializeField]
    private InteractionRadius _interactionRadius;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private Animator _animator;
    private Coroutine LookCoroutine;

    private SkinnedMeshRenderer _meshRenderer;
    public int Fatness;
    public int Aggressivity;

    [SerializeField]
    private int _baseHealth;
    public int _health;

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }

    private const string k_Attack = "Attack";

    private Coroutine AttackCoroutine;
    private bool _isMoving = true;

    private float deathLength;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _interactionRadius.OnAttack += OnAttack;
        _interactionRadius.OnGive += OnGive;
        
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            if (clip.name == "Armature|Die")
            {
                deathLength = clip.length;
            }
        }
    }

    public void PlayHappyAnimation()
    {
        _animator.SetTrigger("Happy");
    }

    void Start()
    {
        InitializeVillager();
        
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 0;
        Aggressivity = 1; //TODO: change this to increase per level
        _health = _baseHealth;
        
        if (GameManager.HasInstance)
            UpdateStatsForDay(GameManager.Instance.CurrentDay);
    }

    private void InitializeVillager()
    {
        Material[] newMaterials = new Material[1];
        newMaterials[0] = VillagerDatabase.Instance.VillagerData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString()).Material;
        _meshRenderer.materials = newMaterials;
    }

    private void OnGive(IDamageable target)
    {
        _animator.SetTrigger(k_Attack);
        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private bool attackRight = true;

    public void DebugAttack()
    {
        OnAttack(null);
    }
    
    private void OnAttack(IDamageable target)
    {
        if (attackRight)
            _animator.SetTrigger("AttackRight");
        else
            _animator.SetTrigger("AttackLeft");

        attackRight = !attackRight;

        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        var lookPos = target.position - transform.position;
        lookPos.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(lookPos);

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
    }

    private void Update()
    {
        if (!_isMoving)
            return;
        
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_interactionRadius.CanDoAction && AttackCoroutine == null)
            {
                AttackCoroutine = StartCoroutine(_interactionRadius.Attack(InteractionRadius.AttackStyle.Repeat, _attackDelay));
            }
            if (!_interactionRadius.CanDoAction && AttackCoroutine != null)
            {
                AttackCoroutine = null;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0) //Dies
        {
            _animator.SetTrigger("Die");
            StartCoroutine(WaitForTime(deathLength));
        }
    }

    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void ReceiveItem()
    {
        Fatness += 1;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void UpdateStatsForDay(int dayNumber)
    {
        var stats = DayStatsSystem.Instance.GetForDay(dayNumber);
        Aggressivity = (int) stats.BaseAggressivity;
    }

    public void StopMoving()
    {
        Agent.SetDestination(transform.position);
        Movement.enabled = false;
        _isMoving = false;
        Agent.enabled = false;
    }

    public void ResumeMoving()
    {
        Movement.enabled = true;
        _isMoving = true;
        Agent.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _interactionRadius.OnAttack += OnAttack;
        _interactionRadius.OnGive += OnGive;
    }

    void Start()
    {
        InitializeVillager();
        
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 0;
        Aggressivity = 1; //TODO: change this to increase per level
        _health = _baseHealth;
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

    private void OnAttack(IDamageable target)
    {
        _animator.SetTrigger(k_Attack);

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
            gameObject.SetActive(false);
        }
    }

    public void ReceiveItem()
    {
        Fatness += 1;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

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
    private Animator _animator;
    private Coroutine LookCoroutine;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public int Fatness;
    public int Aggressivity;

    [SerializeField]
    private int _base_health;
    public int _health { get; set; }

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }

    private const string k_Attack = "Attack";

    private void Awake()
    {
        _interactionRadius.OnAttack += OnAttack;
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
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 0;
        Aggressivity = 1; //TODO: change this to increase per level
        _health = _base_health;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
    }

    public void EatFood()
    {
        Fatness += 1;
    }

    //public void Die()
    //{
    //    StartCoroutine(DieCoroutine());
    //} 

    //private IEnumerator DieCoroutine()
    //{
    //    yield return null;
    //    Destroy(gameObject);
    //}

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

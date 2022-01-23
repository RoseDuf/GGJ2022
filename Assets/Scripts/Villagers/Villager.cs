using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : PoolableObject
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
    private SphereCollider _interactionCollider;
    [SerializeField]
    private SphereCollider _lineOfSightCollider;

    [SerializeField]
    private FoodType _typeOfFood;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public int Fatness;
    public int Aggressivity;

    [SerializeField]
    private int _baseLife;
    public int Life { get; set; }

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }


    // Start is called before the first frame update
    void Start()
    {
        UIArrow = GetComponentInChildren<UIArrow>();
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            _lineOfSightCollider.enabled = false;
        }
        else
        {
            _lineOfSightCollider.enabled = true;
        }
        Fatness = 0;
        Aggressivity = 0; //TODO: change this to increase per level
        Life = _baseLife;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    } 

    private IEnumerator DieCoroutine()
    {
        yield return null;
        Destroy(gameObject);
    }
}

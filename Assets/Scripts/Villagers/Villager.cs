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
    private FoodType _typeOfFood;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public int Fatness;

    [SerializeField]
    private int _baseLife;
    public int Life { get; set; }

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }


    // Start is called before the first frame update
    void Start()
    {
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 0;
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

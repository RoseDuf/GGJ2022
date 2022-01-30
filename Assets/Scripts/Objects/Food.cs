using UnityEngine;
using UnityEngine.AI;

public class Food : PoolableObject, IGrabable
{
    public enum FoodType
    {
        Apple,
        Fish,
        Cake,
        Cheese
    }
    public NavMeshAgent Agent;

    [SerializeField]
    private UIFood _uiFood;
    public UIFood GetUIFood {get { return _uiFood; } }
    public FoodType _typeOfFood;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public FoodData FoodData { get; set; }

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }
    
    void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshFilter = GetComponentInChildren<MeshFilter>();
    }

    void Start()
    {
        InitializeFood();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
    }

    public void InitializeFood()
    {
        FoodData = FoodDatabase.Instance.FoodData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString());
        _meshRenderer.material = FoodData.Material;
        _meshFilter.mesh = FoodData.Mesh;
        _meshFilter.transform.eulerAngles = FoodData.Rotation;
        _meshFilter.transform.localScale = new Vector3(FoodData.Scale, FoodData.Scale, FoodData.Scale);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _uiFood.ShowArrow(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _uiFood.ShowArrow(false);
        }
    }
}

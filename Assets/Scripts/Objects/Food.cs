using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public enum FoodType
    {
        Apple,
        Fish,
        Bread,
        Cheese
    }

    [SerializeField]
    private FoodType _typeOfFood;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        InitializeFood();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeFood()
    {
        _meshRenderer.material = FoodDatabase.Instance.FoodData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString()).Material;
        _meshFilter.mesh = FoodDatabase.Instance.FoodData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString()).Mesh;
    }
}

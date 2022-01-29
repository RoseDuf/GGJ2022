using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    private int _numberFoodsToSpawn = 5;
    [SerializeField]
    private float _spawnDelay = 0f;

    public List<Food> FoodPrefabs = new List<Food>();
    public SpawnMethod FoodSpawnMethod = SpawnMethod.RoundRobin;

    private bool foodWasSpawned;

    private NavMeshTriangulation _triangulation;
    private Dictionary<int, ObjectPool> _FoodObjectPools = new Dictionary<int, ObjectPool>();

    private void Awake()
    {
        for (int i = 0; i < FoodPrefabs.Count; i++)
        {
            _FoodObjectPools.Add(i, ObjectPool.CreateInstance(FoodPrefabs[i], _numberFoodsToSpawn));
        }
    }

    private void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        foodWasSpawned = false;
    }

    private void Update()
    {
        if (!foodWasSpawned && DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            StartCoroutine(SpawnFoods());
            foodWasSpawned = true;
        }
        if (foodWasSpawned && DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            DespawnFoods();
            foodWasSpawned = false;
        }
    }

    private IEnumerator SpawnFoods()
    {
        WaitForSeconds Wait = new WaitForSeconds(_spawnDelay);

        int spawnedFoods = 0;

        while (spawnedFoods < _numberFoodsToSpawn)
        {
            if (FoodSpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinFood(spawnedFoods);
            }
            else if (FoodSpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomFood();
            }

            spawnedFoods++;
            yield return Wait;
        }
    }

    private void DespawnFoods()
    {
        for (int i = 0; i < _FoodObjectPools.Count; i++)
        {
            PoolableObject poolableobject = _FoodObjectPools[i].GetObject();
            if (poolableobject != null)
            {
                Food food = poolableobject.GetComponent<Food>();

                food.gameObject.SetActive(false);
                food.Agent.enabled = true;
            }
        }
    }

    private void SpawnRoundRobinFood(int spawnedFoods)
    {
        int spawnIndex = spawnedFoods % FoodPrefabs.Count;

        DoSpawnFood(spawnIndex);
    }

    private void SpawnRandomFood()
    {
        DoSpawnFood(Random.Range(0, FoodPrefabs.Count));
    }

    private void DoSpawnFood(int index)
    {
        PoolableObject poolableobject = _FoodObjectPools[index].GetObject();

        if (poolableobject != null)
        {
            Food Food = poolableobject.GetComponent<Food>();

            Food.FoodType[] foodTypes = { Food.FoodType.Apple, Food.FoodType.Fish, Food.FoodType.Cake, Food.FoodType.Cheese };
            int randomFoodType = Random.Range(0, foodTypes.Length);

            Food.Agent.Warp(GetRandomLocation());
            Food.Agent.enabled = true;
            Food.Type = foodTypes[randomFoodType];

        }
        else
        {
            Debug.LogWarning($"Unable to fetch enemy of type {index} from object pool.");
        }
    }

    private Vector3 GetRandomLocation()
    {
        int maxIndices = _triangulation.indices.Length - 3;

        // Pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = Random.Range(0, maxIndices);
        int secondVertexSelected = Random.Range(0, maxIndices);

        //Spawn on Verticies
        Vector3 point = _triangulation.vertices[_triangulation.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = _triangulation.vertices[_triangulation.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = _triangulation.vertices[_triangulation.indices[secondVertexSelected]];

        //Eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GetRandomLocation(); //Re-Roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // Select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, Random.Range(0.05f, 0.95f));
        }

        return point;
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }
}

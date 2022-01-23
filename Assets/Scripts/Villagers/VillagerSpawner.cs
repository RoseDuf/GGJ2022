using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private int _numberVillagersToSpawn = 5;
    [SerializeField]
    private float _spawnDelay = 1f;

    public List<Villager> VillagerPrefabs = new List<Villager>();
    public SpawnMethod villagerSpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation _triangulation;
    private Dictionary<int, ObjectPool> _villagerObjectPools = new Dictionary<int, ObjectPool>();

    private void Awake()
    {
        for (int i = 0; i < VillagerPrefabs.Count; i++)
        {
            _villagerObjectPools.Add(i, ObjectPool.CreateInstance(VillagerPrefabs[i], _numberVillagersToSpawn));
        }
    }

    private void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnVillagers());
    }

    private IEnumerator SpawnVillagers()
    {
        WaitForSeconds Wait = new WaitForSeconds(_spawnDelay);

        int spawnedVillagers = 0;

        while (spawnedVillagers < _numberVillagersToSpawn)
        {
            if (villagerSpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinVillager(spawnedVillagers);
            }
            else if (villagerSpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomVillager();
            }

            spawnedVillagers++;
            yield return Wait;
        }
    }

    private void SpawnRoundRobinVillager(int spawnedVillagers)
    {
        int spawnIndex = spawnedVillagers % VillagerPrefabs.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomVillager()
    {
        DoSpawnEnemy(Random.Range(0, VillagerPrefabs.Count));
    }

    private void DoSpawnEnemy(int index)
    {
        PoolableObject poolableobject = _villagerObjectPools[index].GetObject();

        if (poolableobject != null)
        {
            Villager villager = poolableobject.GetComponent<Villager>();
            
            villager.Agent.Warp(GetRandomLocation());
            villager.Movement.Target = _player;
            villager.Agent.enabled = true;

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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> objectsToSpawn; // List of different objects to spawn
    public int numberOfSpawns; // Total number of objects to spawn
    public float spawnRadius = 50f;
    public float cullRadius = 100f;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> availableObjectsToSpawn;

    void Start()
    {
        availableObjectsToSpawn = new List<GameObject>(objectsToSpawn);
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            ManageSpawnedObjects();
            if (spawnedObjects.Count < numberOfSpawns)
            {
                TrySpawnObject();
            }
        }
    }

    void TrySpawnObject()
    {
        if (availableObjectsToSpawn.Count == 0) return;

        Vector3 spawnPosition;
        if (TryGetSpawnPosition(out spawnPosition))
        {
            int index = Random.Range(0, availableObjectsToSpawn.Count);
            GameObject objectToSpawn = availableObjectsToSpawn[index];
            availableObjectsToSpawn.RemoveAt(index);

            GameObject newObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            spawnedObjects.Add(newObject);
        }
    }

    bool TryGetSpawnPosition(out Vector3 position)
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection += player.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    void ManageSpawnedObjects()
    {
        // Cull objects too far from the player
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(player.transform.position, spawnedObjects[i].transform.position) > cullRadius)
            {
                GameObject objectToRecycle = spawnedObjects[i];
                availableObjectsToSpawn.Add(objectToRecycle);
                Destroy(objectToRecycle);
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.transform.position, spawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, cullRadius);
        }
    }
}

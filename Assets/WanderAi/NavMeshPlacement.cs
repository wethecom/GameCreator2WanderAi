using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NavMeshPlacement : MonoBehaviour
{
    public bool infiniteRetries = false; // Set to true for infinite retries
    public int maxAttempts = 3; // Used only if infiniteRetries is false
    public float initialSearchRadius = 10.0f;
    public float searchRadiusIncrement = 5.0f;

    void Start()
    {
        StartCoroutine(PlaceOnNavMesh());
    }

    IEnumerator PlaceOnNavMesh()
    {
        bool placed = false;
        int attempts = 0;
        float searchRadius = initialSearchRadius;

        // Loop condition checks if object is not placed and either infinite retries are allowed or the max attempts are not exceeded
        while (!placed && (infiniteRetries || attempts < maxAttempts))
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                placed = true;
            }
            else
            {
                attempts++;
                searchRadius += searchRadiusIncrement;
                yield return null; // Wait for the next frame
            }
        }

        if (!placed)
        {
            Debug.LogWarning("Failed to place object on NavMesh: " + gameObject.name);
        }
    }
}

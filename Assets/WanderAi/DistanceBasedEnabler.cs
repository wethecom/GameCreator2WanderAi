using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedEnabler : MonoBehaviour
{
    public List<GameObject> gameObjects; // List of GameObjects to check
    private GameObject player; // Player GameObject
    public float maxDistance = 500f; // Max distance for enabling/disabling
    public float checkInterval = 1f; // Time interval in seconds for distance check
    private float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            CheckDistances();
            timer = 0;
        }
    }

    void CheckDistances()
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                float distance = Vector3.Distance(player.transform.position, obj.transform.position);
                obj.SetActive(distance <= maxDistance);
            }
        }
    }
}


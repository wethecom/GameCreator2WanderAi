using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class ObjectEmitter : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    public Texture2D customImage;
    public GameObject[] objectsToEmit; // Assign your prefabs here
    public Vector2 spawnAreaSize = new Vector2(1f, 1f);
    public Vector3 rotationRange = new Vector3(360, 360, 360); // Rotation range in degrees for each axis
    public Vector3 positionAdjustment = Vector3.zero; // Position adjustment offset
    public bool MultiplyByArea = false; // New boolean field
    public int numberOfTimesToEmit = 1; // Specify the number of times to emit objects
    public LayerMask emissionLayerMask = -1; // Layer mask for emission

 

    private void Start()
    {
        // EmitObjects();
    }

    public void Clear()
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            DestroyImmediate(child);
        }
    }

    private void EmitObjects(int numberOfTimes)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            int randomIndex = Random.Range(0, objectsToEmit.Length); // Select a random index
            GameObject prefabToEmit = objectsToEmit[randomIndex]; // Get the prefab at the random index
            Vector3 spawnPosition = GetRandomPositionInSpawnArea();
            Quaternion spawnRotation = GetRandomRotation();

            // Perform a raycast with the specified layer mask
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity, emissionLayerMask))
            {
                // Use the point of intersection as the new position and apply the position adjustment offset
                spawnPosition = hit.point + positionAdjustment;
                GameObject emittedObject = Instantiate(prefabToEmit, spawnPosition, spawnRotation, transform);
            }
        }
    }

    public void EmitNew()
    {
        int calculatedNumberOfTimes = numberOfTimesToEmit; // Initialize to the specified value

        if (MultiplyByArea)
        {
            float area = spawnAreaSize.x * spawnAreaSize.y;
            calculatedNumberOfTimes = Mathf.RoundToInt(area) * numberOfTimesToEmit;
            calculatedNumberOfTimes = Mathf.Max(calculatedNumberOfTimes, numberOfTimesToEmit);
        }

        EmitObjects(calculatedNumberOfTimes);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + new Vector3(0, -0.5f, 0);
        Vector3 size = new Vector3(spawnAreaSize.x, 0.01f, spawnAreaSize.y);
        Gizmos.DrawWireCube(center, size);
    }

    private Vector3 GetRandomPositionInSpawnArea()
    {
        Vector3 randomPosition = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            -0.5f,
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );

        RaycastHit hit;

        // Perform a raycast from the random position downwards with the specified layer mask
        if (Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity, emissionLayerMask))
        {
            // Use the point of intersection as the new position and apply the position adjustment offset
            return hit.point + positionAdjustment;
        }

        // If no intersection is found, return the original random position
        return randomPosition;
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(
            Random.Range(-rotationRange.x / 2, rotationRange.x / 2),
            Random.Range(-rotationRange.y / 2, rotationRange.y / 2),
            Random.Range(-rotationRange.z / 2, rotationRange.z / 2)
        );
    }
}

[CustomEditor(typeof(ObjectEmitter))]
public class ObjectEmitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectEmitter script = (ObjectEmitter)target;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Add flexible space to center the image horizontally

        if (script.customImage != null)
        {
            GUILayout.Label(script.customImage, GUILayout.Width(256), GUILayout.Height(128));
        }

        GUILayout.FlexibleSpace(); // Add flexible space to center the image horizontally
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(); // Add some spacing below the image

        DrawDefaultInspector(); // Draws the default inspector below the image

        EditorGUILayout.Space(); // Add some spacing between the default inspector and other fields

        script.MultiplyByArea = EditorGUILayout.Toggle("Multiply By Area", script.MultiplyByArea);

        if (GUILayout.Button("Emit Objects"))
        {
            script.EmitNew(); // Calls the method when the button is pressed
        }

        if (GUILayout.Button("Clear"))
        {
            script.Clear(); // Calls the method when the button is pressed
        }
    }
}
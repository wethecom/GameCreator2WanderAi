using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WaypointDrawerComponent : MonoBehaviour
{
    public Color lineColor = Color.yellow; // Customizable color
    public float sphereSize = 0.1f; // Size of the sphere markers

    // Get all child objects
    [HideInInspector]
    public List<Transform> waypoints = new List<Transform>();

    // Control whether waypoints are movable in the editor
    [HideInInspector]
    public bool enableEditing = true;

    private void OnDrawGizmos()
    {
        DrawWaypointGizmos();
    }

    public void DrawWaypointGizmos()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }

        // Sort them based on the custom logic
        waypoints = waypoints.OrderBy(wp => GetNumberFromName(wp.name)).ToList();

        // Draw the blue sphere at the parent's position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, sphereSize);

        // Set the Gizmo color for lines
        Gizmos.color = lineColor;

        // Draw Gizmo lines and spheres between the waypoints
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (i < waypoints.Count - 1)
            {
                // Draw line to the next waypoint
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            // Change color for the first waypoint
            if (i == 0)
            {
                Gizmos.color = Color.green;
            }
            // Change color for the last waypoint
            else if (i == waypoints.Count - 1)
            {
                Gizmos.color = Color.red;
            }

            // Draw sphere at waypoint position
            Gizmos.DrawSphere(waypoints[i].position, sphereSize);

            // Reset color back to line color for the next iteration
            Gizmos.color = lineColor;
        }
    }

    private int GetNumberFromName(string name)
    {
        string numberStr = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(numberStr, out int number) ? number : -1;
    }
}

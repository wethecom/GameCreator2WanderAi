using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

[Version(1, 0, 0)]
[Title("Path Selector")]
[Description("Selects a path for use in game logic, on the path will be child waypoints another instruction will Handle")]
[Category("Pathfinding/Path Selector")]
[Keywords("Walk", "Wander", "Follow", "Patrol", "Roaming", "Run", "Position", "Location", "Destination", "Track", "Path", "Waypoint")]
[Image(typeof(IconCharacterWalk), ColorTheme.Type.Blue)]
[Serializable]
public class InstructionPathSelector : Instruction
{
    [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
    [SerializeField] private int numberOfPathsToConsider = 5;
    [SerializeField] private LocalNameVariables m_NameVariables; // Reference to a Variable
    [SerializeField] private string ChosenPathNamedVariable; // Reference to a Variable
    [SerializeField] private string PathTagName; // Reference to a Variable
    [SerializeField] private CollectorListVariable m_WaypointCollector;
    //[SerializeField] private LocalNameVariables m_RearestWay; // Reference to a Variable
    [SerializeField] private string RearestWayNamedVariable; // Reference to a Variable

    protected override Task Run(Args args)
    {
        GameObject chosenPath = null;
        GameObject[] allPaths = GameObject.FindGameObjectsWithTag(PathTagName);
        GameObject target = this.m_Target.Get(args);

        if (target == null)
        {
            Debug.LogError("Target is null");
            return DefaultResult;
        }

        List<GameObject> sortedPaths = allPaths.OrderBy(path => (path.transform.position - target.transform.position).sqrMagnitude).ToList();

        List<GameObject> closestPaths = sortedPaths.Take(numberOfPathsToConsider).ToList();

        if (closestPaths.Any())
        {
            chosenPath = closestPaths[UnityEngine.Random.Range(0, closestPaths.Count)];
            m_NameVariables.Set(ChosenPathNamedVariable, chosenPath);
            Debug.Log("Chosen Path: " + chosenPath.name);
        }
        else
        {
            Debug.LogError("No paths found.");
            return DefaultResult;
        }

        List<GameObject> waypointsList = new List<GameObject>();
        foreach (Transform child in chosenPath.transform)
        {
            waypointsList.Add(child.gameObject);
        }

        this.m_WaypointCollector.Fill(waypointsList.Cast<object>().ToArray(), args);

        foreach (GameObject waypoint in waypointsList)
        {
            Debug.Log("Added Waypoint: " + waypoint.name);
        }

        List<GameObject> sortedWays = waypointsList.OrderBy(way => (way.transform.position - target.transform.position).sqrMagnitude).ToList();

        if (sortedWays.Count == 0)
        {
            Debug.LogError("No waypoints found in chosen path.");
            return DefaultResult;
        }

        GameObject nearestWaypoint = sortedWays[0];
        m_NameVariables.Set(RearestWayNamedVariable, nearestWaypoint);

        Debug.Log("Nearest Waypoint: " + nearestWaypoint.name);

        return DefaultResult;
    }
}


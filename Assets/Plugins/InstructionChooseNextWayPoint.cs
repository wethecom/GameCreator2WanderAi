using System;
using System.Collections.Generic;
using System.Linq;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
[Version(1, 0, 0)]
[Title("Choose Next Path")]
[Description("Selects the next waypoint based on the order of numbers in their names")]

[Category("YourCategory/Choose Next Path")]

[Parameter("Waypoint Collector", "The collector that holds the list of waypoints")]

//[Image(typeof(IconPath), ColorTheme.Type.Blue)]

[Keywords("Path", "Waypoint", "Navigation", "Sequence")]

[Serializable]
public class InstructionChooseNextPath : Instruction
{
    [SerializeField] private CollectorListVariable m_WaypointCollector;
    private GameObject currentWay;
    [SerializeField] private LocalNameVariables m_NameVariables; // Reference to a Variable
    [SerializeField] private string RearestWayNamedVariable; // Reference to a Variable
    //[SerializeField] private Actions EndOfWays;
    [Header("Run a instruction at the end of a ways")]
    [SerializeField] private InstructionList m_EndOfWays = new InstructionList();
    public override string Title => "Choose Next Path from " + m_WaypointCollector;

    
    public InstructionList EndOfWays => this.m_EndOfWays;
    protected override async Task Run(Args args)
    {

        currentWay = (GameObject)m_NameVariables.Get(RearestWayNamedVariable);
        List<object> source = m_WaypointCollector.Get(args);
        List<GameObject> waypoints = source.Cast<GameObject>().ToList();

        waypoints = waypoints.OrderBy(wp => GetNumberFromName(wp.name)).ToList();

        int currentIndex = currentWay != null ? waypoints.IndexOf(currentWay) : -1;

        if (currentIndex >= 0 && currentIndex < waypoints.Count - 1)
        {
            currentWay = waypoints[currentIndex + 1];
            Debug.Log("Next Path: " + currentWay.name);
            m_NameVariables.Set(RearestWayNamedVariable, currentWay);
        }
        else
        {
            await this.EndOfWays.Run(args);
            Debug.Log("End of ways");
        }

        return ;//DefaultResult
    }

    private int GetNumberFromName(string name)
    {
        string numberStr = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(numberStr, out int number) ? number : -1;
    }
}

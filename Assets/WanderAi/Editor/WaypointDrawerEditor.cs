using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointDrawerComponent))]
public class WaypointDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaypointDrawerComponent waypointDrawer = (WaypointDrawerComponent)target;

        // Draw the default inspector
        DrawDefaultInspector();

        // Add a button to duplicate and modify the object
        if (GUILayout.Button("Duplicate and Opposite Direction"))
        {
            DuplicateAndModify(waypointDrawer);
        }
        // Add a button to sort the children numerically
        if (GUILayout.Button("BROKEN? Sort Children Numerically"))
        {
            SortChildrenNumerically(waypointDrawer);
        }
        // Add a button to rotate the parent by 45 degrees in the Y position
        if (GUILayout.Button("Rotate Parent by 45° in Y"))
        {
            RotateParentBy45DegreesY(waypointDrawer);
        }
    }

    private void DuplicateAndModify(WaypointDrawerComponent waypointDrawer)
    {
        if (waypointDrawer != null)
        {
            // Duplicate the GameObject
            GameObject duplicatedObject = Instantiate(waypointDrawer.gameObject);

            // Rename the parent to "Path"
            duplicatedObject.name = "Path";

            // Rename children in reverse order
            Transform[] children = duplicatedObject.GetComponentsInChildren<Transform>();
            int childCount = children.Length;

            for (int i = 0; i < childCount; i++)
            {
                children[i].name = "Ways (" + (childCount - i) + ")";
            }

            // Re-order the children to match the renaming
            for (int i = 1; i < childCount; i++)
            {
                children[i].SetSiblingIndex(childCount - i - 1);
            }

            // Move the duplicate slightly to the side (adjust the position as needed)
            duplicatedObject.transform.position += Vector3.right * 2f;
        }
    }

    private void RotateParentBy45DegreesY(WaypointDrawerComponent waypointDrawer)
    {
        if (waypointDrawer != null)
        {
            // Rotate the parent GameObject by 45 degrees in the Y position
            waypointDrawer.transform.Rotate(Vector3.up, 45f);
        }
    }

        private void SortChildrenNumerically(WaypointDrawerComponent waypointDrawer)
    {
        if (waypointDrawer != null)
        {
            // Get the children of the waypointDrawer's GameObject
            Transform[] children = waypointDrawer.transform.GetComponentsInChildren<Transform>();

            // Sort the children numerically based on their names
            System.Array.Sort(children, (x, y) => {
                int xNum, yNum;
                if (int.TryParse(x.name, out xNum) && int.TryParse(y.name, out yNum))
                {
                    return xNum.CompareTo(yNum);
                }
                return 0;
            });

            // Re-parent the sorted children
            for (int i = 1; i < children.Length; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }
    }
}



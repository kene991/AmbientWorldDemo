using UnityEditor;
using UnityEngine;

[InitializeOnLoad()]
public class PathNodeEditor
{
    //Draw Gizmo attribute allows to draw a debugger and allow for different picks.
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(PathNode waypoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            if (waypoint.nextWaypoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(waypoint.transform.position, waypoint.nextWaypoint.transform.position);
            }

            if (waypoint.waypointBranches != null)
            {
                foreach (var item in waypoint.waypointBranches)
                {
                    if (item == null) continue;

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(waypoint.transform.position, item.transform.position);
                }
            }

            ///if node is selected in the editor, the color will turn red
            Gizmos.color = Color.red;
        }
        else
        {

            if (waypoint.nextWaypoint != null)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawLine(waypoint.transform.position, waypoint.nextWaypoint.transform.position);
            }

            //if node is deselected then the color will be yellow
            Gizmos.color = Color.white * 0.5f;

        }

        //drawing node sphere for each waypoint presented in the scene
        Gizmos.DrawSphere(waypoint.transform.position, .5f);    
      
    }
}

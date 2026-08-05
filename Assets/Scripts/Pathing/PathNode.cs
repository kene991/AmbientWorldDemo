using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Node Point Direction")]
    public PathNode nextWaypoint;

    [Header("Node Direction Branch")]
    public List<PathNode> waypointBranches = new List<PathNode>();

    [Header("NPC Variables")]
    [Range(0, 1)]
    [Tooltip("Determine if NPC to go along another point which leads to a different branch")]
    public float branchRatio = 0.5f;

    [Range(0f, 3f)]
    public float idleTime = 0f;

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}

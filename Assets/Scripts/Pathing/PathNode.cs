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

    [Range(0f, 5f)]
    public float width = 1f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0, 1f));
    }
}

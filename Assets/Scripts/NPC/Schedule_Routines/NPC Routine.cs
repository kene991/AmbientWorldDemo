using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCRoutine : MonoBehaviour
{
    private NavMeshAgent agent;

    //place routine scheduler here
    [Header("Schedule Settings")]
    private int lastHour;
    public RoutineBlock currentRoutine;
    public RoutineSO routineSchedule;
    [SerializeField] bool onFreeTime; //if the the npc has free time

    [Header("Pathing Settings")]
    public PathNode currentPathNode;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void UpdateRoutine()
    {
        int currentHour = GameClockManager.Instance.GetTimeSpanned;

        if (currentHour != lastHour)
        {
            lastHour = currentHour;
            currentRoutine = routineSchedule.GetBlock(currentHour);
            routineSchedule.IsTimeWithinBlock(currentHour, currentRoutine);
        }

    }

    #region Default Pathing
    public PathNode GetClosestNode()
    {
        PathNode[] pathNodes = FindObjectsByType<PathNode>(FindObjectsSortMode.None);

        PathNode closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (PathNode node in pathNodes)
        {
            float distance = Vector3.SqrMagnitude(transform.position - node.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public PathNode SetNextNode()
    {
        if (currentPathNode == null)
            return null;

        print("next");

        currentPathNode = ChooseNextNode(currentPathNode);
        return currentPathNode;
    }

    public void UpdateNodePath()
    {
        if (currentPathNode == null)
        {
            currentPathNode = GetClosestNode();
            return;
        }

        currentPathNode = SetNextNode();
    }

    private PathNode ChooseNextNode(PathNode node)
    {
        if (node.waypointBranches.Count > 0 && Random.value <= node.branchRatio)
        {
            return node.waypointBranches[Random.Range(0, node.waypointBranches.Count)];
        }

        print("next again");
        return node.nextWaypoint;
    }
    #endregion
}

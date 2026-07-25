using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FreeTimeActivity
{
    Idle,
    Wander,
    ReturnHome,
    //Socialize,
    //InteractIGA,
    //Sit
}

public class NPCRoutine : MonoBehaviour
{
    private NPCStateMachine _agentMachine;
    private int _intervalTime;

    [Header("Home Settings")]
    public Transform defualtHome;

    [Header("Schedule Settings")]
    public FreeTimeActivity activityOnFreeTime;
    public RoutineBlock currentRoutine;
    public RoutineSO routineSchedule;
    [SerializeField] bool isInFreeTime; //if the the npc has free time

    [Header("Pathing Settings")]
    public PathNode currentPathNode;

    private void Awake()
    {
        _agentMachine = GetComponent<NPCStateMachine>();
    }

    private void Update()
    {
        UpdateRoutine();

        if (isInFreeTime && !_agentMachine.Agent.pathPending && (!_agentMachine.Agent.hasPath ||_agentMachine.Agent.pathStatus != NavMeshPathStatus.PathComplete))
        {
            switch (activityOnFreeTime)
            {
                case FreeTimeActivity.Idle:
                    _agentMachine.StopNPC();
                    //play random idle animation
                break;
                    
                case FreeTimeActivity.Wander:

                    if (_agentMachine.Agent.isStopped)
                        _agentMachine.ResumeNPC();

                    UpdateNodePath();

                break;

                case FreeTimeActivity.ReturnHome:

                    if (_agentMachine.Agent.isStopped)
                        _agentMachine.ResumeNPC();

                    _agentMachine.MoveToPosition(defualtHome.position);

                    break;

                default:
                break;
            }

        }
    }

    private void UpdateRoutine()
    {
        if (!GameClockManager.Instance) return;

        int currentHour = GameClockManager.Instance.GetTimeSpanned;

        if (currentHour != _intervalTime)
        {
            _intervalTime = currentHour;
            currentRoutine = routineSchedule.GetBlock(currentHour);

            isInFreeTime = currentRoutine == null;
        }

    }

    #region Default Wander Pathing
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
        if (!isInFreeTime) return;

        if (currentPathNode == null)
        {
            currentPathNode = GetClosestNode();
            return;
        }

        currentPathNode = SetNextNode();
        _agentMachine.MoveToPosition(currentPathNode.transform.position);
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

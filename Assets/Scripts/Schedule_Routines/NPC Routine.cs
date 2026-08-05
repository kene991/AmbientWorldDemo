using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FreeTimeActivity
{
    Free_Roam,
    ReturnHome,
    //Socialize,
    //InteractIAO,
    //Sit
}

public class NPCRoutine : MonoBehaviour
{
    [SerializeField] private LocationManager locationManager;
    private NPCStateMachine _agentMachine;
    private int _intervalTime;

    [Header("Destination Location")]
    public bool IsInTaskLocation;
    public Location currentTaskLocation;

    [Header("Activity Settings")]
    [SerializeField] bool isInFreeTime; //if the the npc has free time
    public bool IsInFreeTime { get { return isInFreeTime; } set { isInFreeTime = value; } }
    public FreeTimeActivity activityOnFreeTime;

    [Header("Routine Settings")]
    public RoutineSO routineSchedule;
    public RoutineBlock currentRoutineBlock;

    private void Awake()
    {
        _agentMachine = GetComponent<NPCStateMachine>();
    }

    private void Start()
    {
        UpdateRoutine(true);
    }

    private void Update()
    {
        UpdateRoutine();
    }

    private void UpdateRoutine(bool forceUpdate = false)
    {
        if (!GameClockManager.Instance)
            return;

        int currentHour = GameClockManager.Instance.GetTimeSpanned;

        // Only skip if we're NOT forcing an update
        // AND the hour hasn't changed.
        if (!forceUpdate && currentHour == _intervalTime)
            return;

        _intervalTime = currentHour;

        if (routineSchedule != null)
        {
            RoutineBlock newBlock = routineSchedule.GetBlock(currentHour);

            // Nothing changed
            if (newBlock?.blockName == currentRoutineBlock?.blockName)
                return;

            currentRoutineBlock = newBlock;

            OnRoutineChanged();
        }

        isInFreeTime = (currentRoutineBlock == null) || routineSchedule == null;

    }

    private void OnRoutineChanged()
    {
        // based on a free-time activity do either of these
        if (isInFreeTime)
        {
            //exit routine task location if npc has one
            if (currentTaskLocation)
            {
                currentTaskLocation.OnNPCExit(_agentMachine);
                currentTaskLocation = null;
                IsInTaskLocation = false;
            }

            //set agent back to active on
            _agentMachine.NPCModel.SetActive(true);

            if (activityOnFreeTime == FreeTimeActivity.Free_Roam)
                _agentMachine.UpdateNodePath();
            
            if (activityOnFreeTime == FreeTimeActivity.ReturnHome)
                _agentMachine.MoveToPosition(locationManager.GetLocation(3546).entryPoint.position);

            //if (activityOnFreeTime == FreeTimeActivity.InteractIAO)

        }
        else
        {
            //go to the location set on their current routine block
            _agentMachine.currentPathNode = null;
            GoToTaskLocation(locationManager.GetLocation(currentRoutineBlock.destinationID));
        }

    }

    public void GoToTaskLocation(Location location)
    {
        IsInTaskLocation = false;

        if (currentTaskLocation != null)
        {
            currentTaskLocation.OnNPCExit(_agentMachine);
            currentTaskLocation = null;
        }

        currentTaskLocation = location;
        _agentMachine.Agent.ResetPath();
        _agentMachine.MoveToPosition(currentTaskLocation.entryPoint.position);
    }

   
}

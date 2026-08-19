using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class NPCRoutine : MonoBehaviour
{
    private NPCStateMachine _agentMachine;
    private NPCInteraction NPCInteraction;
    private int _intervalTime;

    [Header("Destination Location")]
    public bool IsInTaskLocation;
    public Location currentTaskLocation;

    [Header("Activity Settings")]
    [SerializeField] bool isInFreeTime; //if the the npc has free time
    public bool IsInFreeTime { get { return isInFreeTime; } set { isInFreeTime = value; } }

    [Header("Routine Settings")]
    public RoutineSO routineSchedule;
    public RoutineBlock currentRoutineBlock;

    private void Awake()
    {
        _agentMachine = GetComponent<NPCStateMachine>();
        NPCInteraction = GetComponent<NPCInteraction>();
    }

    private void OnEnable()
    {
        GameClockManager.Instance.OnHourChanged += UpdateRoutine;
    }

    private void OnDisable()
    {
        GameClockManager.Instance.OnHourChanged -= UpdateRoutine;
    }

    public void UpdateRoutine(int currentHour)
    {
        if (WeatherManager.instance.isRaining)
            _agentMachine.UpdateWalkSpeed(_agentMachine.MaxSpeed);
        else
            _agentMachine.UpdateWalkSpeed(_agentMachine.MaxSpeed / 2);

        if (routineSchedule != null)
        {
            RoutineBlock newBlock = routineSchedule.GetBlock(currentHour);

            // Nothing changed
            if (newBlock?.blockName == currentRoutineBlock?.blockName)
                return;

            currentRoutineBlock = newBlock;

            if (currentTaskLocation)
            {
                IsInTaskLocation = false;
                currentTaskLocation.OnNPCExit(_agentMachine);
                currentTaskLocation = null;
            }
        }

        bool stillInFreeTime = isInFreeTime;

        isInFreeTime = (currentRoutineBlock == null || routineSchedule == null);

        if (stillInFreeTime == isInFreeTime)
            return;

        OnRoutineChanged();
    }

    private void OnRoutineChanged()
    {
        // based on a free-time activity do either of these
        if (isInFreeTime)
        {
            EnterFreeTime();
            return;
        }
    }

    private void EnterFreeTime()
    {
        //exit routine task location if npc has one
        if (currentTaskLocation)
        {
            IsInTaskLocation = false;
            currentTaskLocation.OnNPCExit(_agentMachine);
            currentTaskLocation = null;
        }

        //set agent back to active on
        _agentMachine.NPCModel.SetActive(true);
        _agentMachine.UpdateNodePath();
    }

    public void GoToTaskLocation(Location location)
    {
        currentTaskLocation = location; 

        _agentMachine.Agent.ResetPath();

        if (location.isArea)
        {
            _agentMachine.MoveToPosition(currentTaskLocation.GetRandomPointInArea());
        }
        else
        {
            _agentMachine.MoveToPosition(currentTaskLocation.entryPoint.position);
        }

    }
}

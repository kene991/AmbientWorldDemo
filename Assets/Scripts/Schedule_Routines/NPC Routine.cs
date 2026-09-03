using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class NPCRoutine : MonoBehaviour
{
    private NPCStateMachine NPCStateMachine;

    [Header("Destination Location")]
    public bool IsInTaskLocation;
    public GoalLocation currentTaskLocation;

    [Header("Activity Settings")]
    [SerializeField] bool isInFreeTime; //if the the npc has free time
    public bool IsInFreeTime { get { return isInFreeTime; } set { isInFreeTime = value; } }

    [Header("Routine Settings")]
    public RoutineSO routineSchedule;
    public RoutineBlock currentRoutineBlock;

    private void Awake()
    {
        NPCStateMachine = GetComponent<NPCStateMachine>();
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
            NPCStateMachine.UpdateWalkSpeed(NPCStateMachine.MaxSpeed);
        else
            NPCStateMachine.UpdateWalkSpeed(NPCStateMachine.MaxSpeed / 2);

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
                currentTaskLocation.OnNPCExit(NPCStateMachine);
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
        if (NPCStateMachine.Interaction && NPCStateMachine.Interaction.currentInteractionObject)
        {
            NPCStateMachine.Interaction.currentInteractionState = InteractionState.Aborting;
        }

        //exit routine task location if npc has one
        if (currentTaskLocation)
        {
            IsInTaskLocation = false;
            currentTaskLocation.OnNPCExit(NPCStateMachine);
            currentTaskLocation = null;
        }

        //set agent back to active on
        NPCStateMachine.NPCModel.SetActive(true);
        NPCStateMachine.UpdateNodePath();
    }

    public void GoToGoalLocation(GoalLocation location)
    {
        currentTaskLocation = location; 

        NPCStateMachine.Agent.ResetPath();

        if (location.isArea)
        {
            NPCStateMachine.MoveToPosition(currentTaskLocation.GetRandomPointInArea());
        }
        else
        {
            NPCStateMachine.MoveToPosition(currentTaskLocation.entryPoint.position);
        }

    }
}

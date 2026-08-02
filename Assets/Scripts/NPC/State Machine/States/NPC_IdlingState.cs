using System.Runtime.InteropServices.ComTypes;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NPC_IdlingState : NPCBaseState
{
    public NPC_IdlingState(NPCStateMachine context) : base(context)
    {
    }


    public override void EnterState(NPCStateMachine state)
    {
        if (state.Routine.IsInFreeTime && state.Routine.activityOnFreeTime == FreeTimeActivity.Free_Roam)
            state.UpdateNodePath();


        // if the task location is an area (park, beach, playground, etc.)
        if (!state.NPCModel.activeSelf)
            return;

        if (state.Routine.currentTaskLocation != null && state.Routine.currentTaskLocation.isArea)
        {
            if (!state.Routine.IsInTaskLocation)
            {
                state.Routine.IsInTaskLocation = true;
                Vector3 randomPoint = state.Routine.currentTaskLocation.GetRandomPointInArea();
                state.MoveToPosition(randomPoint);
                return;
            }

            state.ReplaceAnimationClip(state.Routine.currentTaskLocation.PlayAnimationAtLocation(), "_Routine");
            state.Animator.SetTrigger("SetRoutine");
        }

    }

    public override void IntializeState(NPCBaseState state)
    {
        
    }

    public override void ExitState(NPCStateMachine state)
    {
    }

    public override void OnCollisionEnter(NPCStateMachine state, Collision collide)
    {
        
    }

    public override void OnTriggerEnter(NPCStateMachine state, Collider collide)
    {
        
    }

    public override void UpdateState(NPCStateMachine state)
    {

        ///plan to track via velocity for better accuratcy 
        if (Mathf.Abs(_npc.Agent.velocity.sqrMagnitude) > 0.05f)
        {
            _npc.OnStateSwitch(_npc.movingState);
        }
    }

}

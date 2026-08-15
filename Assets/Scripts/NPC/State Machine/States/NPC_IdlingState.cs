using System.Runtime.InteropServices.ComTypes;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class NPC_IdlingState : NPCBaseState
{
    public NPC_IdlingState(NPCStateMachine context) : base(context)
    {
    }

    public override void EnterState(NPCStateMachine state)
    {
        // if I have free time?
        if (state.Routine.IsInFreeTime)
        {
            // I stopped to interact with something
            if (state.Interaction.currentInteractionObject)
            {
                state.OnStateSwitch(state.useInteractionActionState);
                return;
            }

            // or should I continue free roaming
            state.UpdateNodePath();
            return;
        }

         IntializeRoutineEntry(state.Routine);

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
        if (Mathf.Abs(_npc.Agent.velocity.sqrMagnitude) > 0.05f)
        {
            _npc.OnStateSwitch(_npc.movingState);
        }
    }

    private void IntializeRoutineEntry(NPCRoutine routine)
    {
        if (routine.currentRoutineBlock == null)
            return;

        if (!routine.currentTaskLocation)
        {
            routine.GoToTaskLocation(LocationManager.instance.GetLocation(routine.currentRoutineBlock.destinationID));
            return;
        }

        if (!routine.currentTaskLocation.isArea)
        {
            // do I currently have a task? and I ain't there??
            if (!routine.IsInTaskLocation)
            {
                _npc.currentPathNode = null;
                routine.IsInTaskLocation = true;
                routine.currentTaskLocation.OnNPCEnter(_npc);
                return;
            }
        }
       

        // can I go a random point in this area
        if (routine.currentTaskLocation.isArea)
        {
            //am I at the random point in the area
            if (!routine.IsInTaskLocation)
            {
                Vector3 randomPoint = routine.currentTaskLocation.GetRandomPointInArea();
                _npc.MoveToPosition(randomPoint);
                routine.IsInTaskLocation = true;
                return;
            }

            _npc.ReplaceAnimationClip(_npc.Routine.currentTaskLocation.PlayAnimationAtLocation(), "_Routine");
            _npc.Animator.SetTrigger("SetRoutine");
        }

    }

}

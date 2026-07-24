using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NPC_IdlingState : NPCBaseState
{
    public NPC_IdlingState(NPCStateMachine context) : base(context)
    {
    }


    public override void EnterState(NPCStateMachine state)
    {
        state.Routine.UpdateNodePath();

        if (state.Routine.currentPathNode != null)
            state.MoveToPosition(state.Routine.currentPathNode.transform.position);
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

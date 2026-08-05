using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_MovingState : NPCBaseState
{
    public NPC_MovingState(NPCStateMachine context) : base(context)
    {
    }

    public override void EnterState(NPCStateMachine state)
    {

    }

    public override void ExitState(NPCStateMachine state)
    {
        _npc.Animator.SetFloat("Velocity", 0);
    }

    public override void IntializeState(NPCBaseState state)
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
        if (_npc.Animator != null)
        {
            _npc.Animator.SetFloat("Velocity", Mathf.Clamp01(_npc.Agent.speed/_npc.MaxSpeed));
        }

        if (!state.HasReachedDestination())
            return;

        _npc.OnStateSwitch(_npc.idleState);
    }



}

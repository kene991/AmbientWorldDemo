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
        if (collide.TryGetComponent(out InteractionAction interaction))
        {
            if (!state.Routine.IsInFreeTime)
                return;

            if (state.Interaction.currentInteractionState != InteractionState.None)
                return;

            if (!state.Interaction.canInteract)
                return;

            if (!interaction.CanInteract(state.Interaction))
                return;

            if (interaction.ReserveSlot(state.Interaction, out state.Interaction.currentSlot))
            {
                state.MoveToPosition(state.Interaction.CurrentSlot.interactionMarker.position);
                state.Interaction.currentInteractionState = InteractionState.Reserved;
            }
        }
    }

    public override void UpdateState(NPCStateMachine state)
    {
        if (_npc.Animator != null)
        {
            _npc.Animator.SetFloat("Velocity", Mathf.Clamp01(_npc.Agent.speed/_npc.MaxSpeed));
        }

        if (state.Interaction.currentInteractionState == InteractionState.Reserved)
            state.Interaction.currentInteractionState = InteractionState.Entering;

        if (!state.HasReachedDestination())
            return;

        _npc.OnStateSwitch(_npc.idleState);
    }



}

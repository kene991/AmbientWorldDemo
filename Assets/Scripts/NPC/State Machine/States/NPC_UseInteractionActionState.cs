using UnityEngine;

public class NPC_UseInteractionActionState : NPCBaseState
{
    public NPC_UseInteractionActionState(NPCStateMachine context) : base(context)
    {
    }

    public override void EnterState(NPCStateMachine state)
    {
        state.StopNPC(false);
        state.Obstacle.enabled = true;
        state.OrientToPosition(state.Interaction.CurrentSlot.interactionMarker);
        state.Interaction.isAtInteractionMarker = true;

        //can call any interaction state
        if (state.Interaction.currentInteractionObject.TryGetComponent<InteractionAction>(out var actionZone))
            actionZone.OnInteractionStart(state.Interaction);
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

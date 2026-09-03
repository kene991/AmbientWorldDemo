using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class InteractionActionZone : InteractionAction
{

    private void Start()
    {
        interactionDurationTime = SetInteractionDurationTime();
    }

    public override void OnInteractionStart(NPCInteractionAction npc)
    {
        CanPerform(npc);
    }

    public override void OnInteractionEnd(NPCInteractionAction npc)
    {
        EndInteractionCheck(npc);

        interactionDurationTime = SetInteractionDurationTime();
    }

    // checks if the interaction required all slots fill or not before firing
    public void CanPerform(NPCInteractionAction npc)
    {

        StartSingleInteraction(npc);
    }

    public void EndInteractionCheck(NPCInteractionAction npc)
    {
        EndSingleInteraction(npc);
    }

    private void StartSingleInteraction(NPCInteractionAction npc)
    {
        if (!npc.isAtInteractionMarker)
            return;

        npc.GetNPCStateMachine().ReplaceAnimationClip
            (AnimationManager.instance.GetRandomAnimation(npc.CurrentSlot.interactionAnimation), "_Interact");

        npc.GetNPCStateMachine().Animator.SetTrigger("SetInteraction");
        npc.CurrentSlot.OnInteractionStart.Invoke();
    }

    private void EndSingleInteraction(NPCInteractionAction npc)
    {
        npc.interactionCooldownTime += npc.currentInteractionObject.postInteractionWaitTime;

        npc.CurrentSlot.OnInteractionEnd.Invoke();
        npc.currentInteractionObject.ReleaseSlot(npc, npc.CurrentSlot);
    }
}

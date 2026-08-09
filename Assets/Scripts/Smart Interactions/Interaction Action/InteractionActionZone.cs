using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionActionZone : InteractionAction
{
    [Header("Zone Condition Settings")]
    public bool ShouldAllBeOccupied; //is all slots occupied

    // checks if the interaction required all slots fill or not before firing
    public void CanPerform(NPCInteraction npc)
    {
        //should all agents be at the marker before executing?
        if (ShouldAllBeOccupied)
        {
            if (interactionRoles.All(slot => slot.occupant != null && slot.occupant.isAtInteractionMarker))
            {
                StartGroupInteraction();
            }
        }
        else
        {
            StartSingleInteraction(npc);
        }
    }

    public void EndInteractionCheck(NPCInteraction npc)
    {
        if (ShouldAllBeOccupied)
        {
            EndGroupInteraction(npc);
            return;
        }

        EndSingleInteraction(npc);
    }

    private void StartSingleInteraction(NPCInteraction npc)
    {
        if (!npc.isAtInteractionMarker)
            return;

        npc.GetNPCStateMachine().ReplaceAnimationClip(npc.CurrentSlot.interactionClip, "_Interact");
        npc.GetNPCStateMachine().Animator.SetTrigger("SetInteraction");
    }

    private void EndSingleInteraction(NPCInteraction npc)
    {
        npc.GetNPCStateMachine().Obstacle.enabled = false;
        npc.GetNPCStateMachine().ResumeNPC();
        npc.interactionCooldownTime += npc.currentInteractionObject.postInteractionWaitTime;

        Debug.Log($"Released {npc.currentInteractionObject.DisplayName}, {npc.CurrentSlot.interactionMarker.name} has been opened!");
        npc.currentInteractionObject.ReleaseSlot(npc, npc.CurrentSlot);
    }

    private void StartGroupInteraction()
    {
        foreach (var slot in interactionRoles)
        {
            if (slot.occupant == null)
                continue;
            if (!slot.occupant.isAtInteractionMarker)
                continue;

            StartSingleInteraction(slot.occupant);
        }
    }

    private void EndGroupInteraction(NPCInteraction npc)
    {
        foreach (var slot in interactionRoles)
        {
            if (slot.occupant == null)
                continue;

            EndSingleInteraction(slot.occupant);
        }
    }

}

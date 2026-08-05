using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionActionObject : MonoBehaviour
{
    [System.Serializable]
    public class InteractionSlot
    {
        [HideInInspector] public NPCInteraction occupant;
        public Transform interactionMarker;
        public AnimationClip interactionClip;
        public bool isOccupied;
    }

    [Header("Object Settings")]
    [SerializeField] protected string _displayName;

    [Header("Trigger Settings")]
    public bool ShouldAllBeOccupied; //is all slots occupied

    [Header("Interaction Settings")]
    [SerializeField] protected string _interactionTag;
    public float interactionDuration;
    public float postInteractionWaitTime;

    [Header("Actor Settings")]
    public InteractionSlot[] interactionSlots;

    [Header("Debug")]
    public Color interactionMarkerColor;

    public string InteractionTag => _interactionTag;
    public string DisplayName => _displayName;

    private Coroutine interactionRoutine;

    public void CheckInteractionReady()
    {
        if (!CanPerform())
            return;

        interactionRoutine = StartCoroutine(InteractionRoutine());
    }

    public virtual bool CanInteract(NPCInteraction npc)
    {
        foreach (var item in npc.interactableTags)
        {
            if (item.ToLower() != _interactionTag.ToLower())
                return false;
        }

        //world condition checks
        return true;
    }

    public InteractionSlot GetFreeSlot()
    {
        foreach (var slot in interactionSlots)
        {
            if (slot.occupant == null)
                return slot;
        }

        return null;
    }

    public bool ReserveSlot(NPCInteraction npc, out InteractionSlot slot)
    {
        slot = GetFreeSlot();

        if (slot == null)
            return false;

        npc.currentInteractionObject = this;
        slot.occupant = npc;
        return true;
    }

    public void ReleaseSlot(NPCInteraction npc, InteractionSlot slot)
    {
        slot.isOccupied = false;
        slot.occupant = null;

        npc.CurrentSlot = null;
        npc.currentInteractionObject = null;
        npc.isAtInteractionMarker = false;
    }

    private IEnumerator InteractionRoutine()
    {
        StartInteraction();

        yield return new WaitForSeconds(interactionDuration);

        EndInteraction();
    }

    //this is the default ending to an interaction, overrides and factors will come into play
    private void EndInteraction()
    {
        foreach(var slot in interactionSlots)
        {
            // prevent interaction from immediately happening
            slot.occupant.interactionCooldownTime += postInteractionWaitTime;

            // default logic for now
            slot.occupant.GetNPCStateMachine().UpdateNodePath();
        }
    }

    private bool CanPerform()
    {
        //should all agents be at the marker before executing?
        if (ShouldAllBeOccupied)
        {
            return interactionSlots.All(slot => slot.occupant != null && slot.occupant.isAtInteractionMarker);
        }

        return interactionSlots.Any(slot => slot.occupant != null && slot.occupant.isAtInteractionMarker);
    }

    private void StartInteraction()
    {
        foreach (var slot in interactionSlots)
        {
            if (slot.occupant == null)
                continue;
            if (!slot.occupant.isAtInteractionMarker)
                continue;

            slot.occupant.GetNPCStateMachine().ReplaceAnimationClip(slot.occupant.CurrentSlot.interactionClip, "_Interact");
            slot.occupant.GetNPCStateMachine().Animator.SetTrigger("SetInteraction");
        }
    }


    private void OnDrawGizmos()
    {
        if (interactionSlots.Length > 0)
        {
            foreach (var slot in interactionSlots)
            {
                if (slot.interactionMarker == null)
                    continue;

                if (slot.isOccupied)
                    Gizmos.color = Color.red;
                else 
                    Gizmos.color = interactionMarkerColor;

                Gizmos.DrawSphere(slot.interactionMarker.position, 0.3f);

                GUI.color = Color.white;
                Handles.Label(slot.interactionMarker.transform.position + Vector3.up * 0.5f, slot.interactionMarker.name);
            }
        }
    }
}

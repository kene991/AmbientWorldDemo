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

    public void StartGroupInteraction()
    {
        foreach (var slot in interactionSlots)
        {
            if (slot.occupant == null)
                continue;
            if (!slot.occupant.isAtInteractionMarker)
                continue;

            slot.occupant.StartSingleInteraction();
        }
    }

    public bool CanInteract(NPCInteraction npc)
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

        npc.isAtInteractionMarker = false;
        npc.CurrentSlot = null;
        npc.currentInteractionObject = null;
    }

    public void CanPerform(NPCInteraction npc)
    {
        //should all agents be at the marker before executing?
        if (ShouldAllBeOccupied)
        {
            if (interactionSlots.All(slot => slot.occupant != null && slot.occupant.isAtInteractionMarker))
            {
                StartGroupInteraction();
            } 
        }
        else
        {
            npc.StartSingleInteraction();
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

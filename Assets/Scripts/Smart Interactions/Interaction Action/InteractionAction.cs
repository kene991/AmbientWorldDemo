using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class InteractionAction : MonoBehaviour
{
    [System.Serializable]
    public class InteractionSlot
    {
        [HideInInspector] public NPCInteractionAction occupant;
        public string roleName;
        public Transform interactionMarker;
        public AnimationType interactionAnimation;

        [Header("Seated")]
        public bool requiredSeated;

        [Header("Role Events")]
        public UnityEvent OnInteractionStart;
        public UnityEvent OnInteractionEnd;
    }

    [Header("Object Settings")]
    [SerializeField] protected string _displayName;

    //Add world conditions here

    [Header("Interaction Settings")]
    [SerializeField] protected string _interactionTag;

    [Header("Interaction Duration")]
    public float interactionDurationTime;

    [Header("Interaction Duration Settings")]
    public float interactionDurationMin;
    public float interactionDurationMax;
    public float postInteractionWaitTime;

    [Header("Actor Settings")]
    public InteractionSlot[] interactionRoles;

    [Header("Debug")]
    public Color interactionMarkerColor;

    public string InteractionTag => _interactionTag;
    public string DisplayName => _displayName;

    public abstract void OnInteractionStart(NPCInteractionAction npc);
    public abstract void OnInteractionEnd(NPCInteractionAction npc);


    // basically conditions (world and npc influenced conditions for the interaction object to be interactable)
    public virtual bool CanInteract(NPCInteractionAction npc)
    {
        bool requiredTagFound = false;

        foreach (var tag in npc.interactableTags)
        {
            if (tag.ToLower() == _interactionTag.ToLower())
            {
                requiredTagFound = true;
            }
            else
                continue;
        }

        if (!requiredTagFound) return false;

        //world condition checks
        return true;
    }

    // finds any open slots for the npcs
    public virtual InteractionSlot GetFreeSlot()
    {
        foreach (var slot in interactionRoles)
        {
            if (slot.occupant == null)
                return slot;
        }

        return null;
    }

    // assigns a role and slot for the npc that enters the slot
    public virtual bool ReserveSlot(NPCInteractionAction npc, out InteractionSlot slot)
    {
        slot = GetFreeSlot();

        if (slot == null)
            return false;

        npc.currentInteractionObject = this;
        slot.occupant = npc;
        Debug.Log($"Reserved {npc.currentInteractionObject.DisplayName}, {slot.interactionMarker.name} has been selected!");
        return true;
    }

    // release the slot for the role designed for the slot
    public virtual void ReleaseSlot(NPCInteractionAction npc, InteractionSlot slot)
    {
        slot.occupant = null;

        Debug.Log($"Released {npc.currentInteractionObject.DisplayName}, {npc.CurrentSlot.interactionMarker.name} has been opened!");

        npc.isAtInteractionMarker = false;
        npc.CurrentSlot = null;
        npc.currentInteractionObject = null;

        npc.GetNPCStateMachine().ResumeNPC();
        //checking if npc has no block in there schedule as time is updated via interval
        if (npc.GetNPCRoutine().IsInFreeTime)
            npc.GetNPCStateMachine().UpdateNodePath();
    }

    // used to debug slot points, can be overriden in other derived slots
    public virtual void InteractionDebugger()
    {
        if (interactionRoles == null || interactionRoles.Length == 0)
            return;

        foreach (var slot in interactionRoles)
        {
            if (slot.interactionMarker == null)
                continue;

            if (slot.occupant)
                Gizmos.color = Color.red;
            else
                Gizmos.color = interactionMarkerColor;

            if (slot.roleName != string.Empty)
                slot.interactionMarker.gameObject.name = slot.roleName;

            Gizmos.DrawSphere(slot.interactionMarker.position, 0.3f);

            GUI.color = Color.white;
            Handles.Label(slot.interactionMarker.transform.position + Vector3.up * 0.5f, slot.interactionMarker.name);
        }

    }

    public float SetInteractionDurationTime()
    {
        return Mathf.Round(Random.Range(interactionDurationMin, interactionDurationMax));
    }

    private void OnDrawGizmos()
    {
        InteractionDebugger();
    }

}

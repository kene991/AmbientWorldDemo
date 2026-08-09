using UnityEditor;
using UnityEngine;

public abstract class InteractionAction : MonoBehaviour
{
    [System.Serializable]
    public class InteractionSlot
    {
        [HideInInspector] public NPCInteraction occupant;
        public string roleName;
        public Transform interactionMarker;
        public AnimationClip interactionClip;
        public bool isOccupied;
    }

    [Header("Object Settings")]
    [SerializeField] protected string _displayName;

    //Add world conditions here

    [Header("Interaction Settings")]
    [SerializeField] protected string _interactionTag;
    public float interactionDuration;
    public float postInteractionWaitTime;

    [Header("Actor Settings")]
    public InteractionSlot[] interactionRoles;

    [Header("Debug")]
    public Color interactionMarkerColor;

    public string InteractionTag => _interactionTag;
    public string DisplayName => _displayName;

    // basically conditions (world and npc influenced conditions for the interaction object to be interactable)
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

    // finds any open slots for the npcs
    public InteractionSlot GetFreeSlot()
    {
        foreach (var slot in interactionRoles)
        {
            if (slot.occupant == null)
                return slot;
        }

        return null;
    }

    // assigns a role and slot for the npc that enters the slot
    public bool ReserveSlot(NPCInteraction npc, out InteractionSlot slot)
    {
        slot = GetFreeSlot();

        if (slot == null)
            return false;

        npc.currentInteractionObject = this;
        slot.occupant = npc;
        return true;
    }

    // release the slot for the role designed for the slot
    public void ReleaseSlot(NPCInteraction npc, InteractionSlot slot)
    {
        slot.isOccupied = false;
        slot.occupant = null;

        npc.isAtInteractionMarker = false;
        npc.CurrentSlot = null;
        npc.currentInteractionObject = null;

        //checking if npc has no block in there schedule as time is updated via interval
        if (npc.GetNPCRoutine().IsInFreeTime)
            npc.GetNPCStateMachine().UpdateNodePath();
    }

    private void OnDrawGizmos()
    {
        if (interactionRoles.Length > 0)
        {
            foreach (var slot in interactionRoles)
            {
                if (slot.interactionMarker == null)
                    continue;

                if (slot.isOccupied)
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
    }
}

using System.Linq;
using UnityEngine;

public enum InteractionType
{
    Instant = 0,
    Overtime = 1,
}

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

    [Header("Interaction Settings")]
    [SerializeField] protected InteractionType _interactionType;
    [SerializeField] protected string _interactionTag;

    [Header("Actor Settings")]
    public InteractionSlot[] interactionSlots;

    [Header("Debug")]
    public Color interactionMarkerColor;

    public string InteractionTag => _interactionTag;
    public string DisplayName => _displayName;
    public InteractionType InteractionType => _interactionType;

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
        slot = null;

        if (!CanInteract(npc))
            return false;

        slot = GetFreeSlot();

        if (slot == null)
            return false;

        npc.currentInteractionObject = this;
        slot.occupant = npc;
        return true;
    }

    public void ReleaseSlot(NPCInteraction npc, InteractionSlot slot)
    {
        npc.currentInteractionObject = null;
        npc.CurrentSlot = null;
        slot.occupant = null;
    }

    private void OnDrawGizmos()
    {
        if (interactionSlots.Length > 0)
        {
            foreach (var slot in interactionSlots)
            {
                if (slot.interactionMarker == null)
                    continue;

                Gizmos.color = interactionMarkerColor;
                Gizmos.DrawSphere(slot.interactionMarker.position, 0.3f);
            }
        }
    }
}

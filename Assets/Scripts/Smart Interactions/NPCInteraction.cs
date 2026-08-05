using Unity.VisualScripting;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private NPCStateMachine NPCStateMachine;
    public NPCStateMachine GetNPCStateMachine() { return NPCStateMachine; }
    private NPCRoutine NPCRoutine;
    public NPCRoutine GetNPCRoutine() { return NPCRoutine; }

    [Header("Interaction Settings")]
    public string[] interactableTags;
    public float interactionCooldownTime;
    public bool isAtInteractionMarker;
    public bool canInteract;

    [Header("Interaction Action Settings")]
    public InteractionActionObject currentInteractionObject;
    private InteractionActionObject.InteractionSlot currentSlot;
    public InteractionActionObject.InteractionSlot CurrentSlot {  get { return currentSlot; } set { currentSlot = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NPCStateMachine = GetComponent<NPCStateMachine>();
        NPCRoutine = GetComponent<NPCRoutine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactionCooldownTime > 0f)
        {
            interactionCooldownTime -= Time.deltaTime;
            interactionCooldownTime = Mathf.Max(0f, interactionCooldownTime);

            if (interactionCooldownTime <= 0f)
            {
                interactionCooldownTime = 0f;
                canInteract = true;
            }
        }
    }

    //public void FindNearbyInteractionActionObject()
    //{

    //}

    //public void PickRandomInteractionActionObject()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InteractionActionObject interaction))
        {
            if (!NPCRoutine.IsInFreeTime)
                return;

            if (!interaction.CanInteract(this))
                return;

            if(interaction.ReserveSlot(this, out currentSlot))
            {
                Debug.Log($"Reserved {interaction.DisplayName}, {currentSlot.interactionMarker.name} has been selected!");
                currentSlot.isOccupied = true;
                NPCStateMachine.MoveToPosition(currentSlot.interactionMarker.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractionActionObject interaction))
        {
            if (currentInteractionObject == null)
                return;

            Debug.Log($"Released {interaction.DisplayName}, {currentSlot.interactionMarker.name} has been opened!");
            interaction.ReleaseSlot(this, currentSlot);
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using static InteractionActionObject;

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
            canInteract = false;
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

    public void StartSingleInteraction()
    {
        if (!isAtInteractionMarker)
            return;

        GetNPCStateMachine().ReplaceAnimationClip(CurrentSlot.interactionClip, "_Interact");
        GetNPCStateMachine().Animator.SetTrigger("SetInteraction");  
    }

    public void EndInteraction()
    {
        interactionCooldownTime += currentInteractionObject.postInteractionWaitTime;

        Debug.Log($"Released {currentInteractionObject.DisplayName}, {currentSlot.interactionMarker.name} has been opened!");
        currentInteractionObject.ReleaseSlot(this, currentSlot);
    }

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

}

using Unity.VisualScripting;
using UnityEngine;
using static InteractionActionZone;

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
    public InteractionAction currentInteractionObject;
    private InteractionAction.InteractionSlot currentSlot;
    public InteractionAction.InteractionSlot CurrentSlot {  get { return currentSlot; } set { currentSlot = value; } }

    void Start()
    {
        NPCStateMachine = GetComponent<NPCStateMachine>();
        NPCRoutine = GetComponent<NPCRoutine>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldownInteraction();
    }

    private void UpdateCooldownInteraction()
    {
        if (interactionCooldownTime > 0f)
        {
            canInteract = false;
            interactionCooldownTime -= Time.deltaTime;
            interactionCooldownTime = Mathf.Max(0f, interactionCooldownTime);
        }

        if (interactionCooldownTime <= 0f)
        {
            interactionCooldownTime = 0f;
            canInteract = true;
        }
    }

    //public void FindNearbyInteractionActionObject()
    //{

    //}

    //public void PickRandomInteractionActionObject()
    //{

    //}

    // just learned about accessing bones via animation
    // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/HumanBodyBones.html

    public Transform GetBone(HumanBodyBones bone)
    {
        return NPCStateMachine.Animator.GetBoneTransform(bone);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out InteractionAction interaction))
        {
            if (currentInteractionObject)
                return;

            if (!canInteract || !NPCRoutine.IsInFreeTime)
                return;

            if (!interaction.CanInteract(this))
                return;

            if(interaction.ReserveSlot(this, out currentSlot))
            {          
                NPCStateMachine.MoveToPosition(currentSlot.interactionMarker.position);
            }
        }
    }

}

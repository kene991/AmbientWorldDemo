using Unity.VisualScripting;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private NPCStateMachine NPCStateMachine;
    public string[] interactableTags;
    public InteractionActionObject currentInteractionObject;
    private InteractionActionObject.InteractionSlot currentSlot;
    public InteractionActionObject.InteractionSlot CurrentSlot {  get { return currentSlot; } set { currentSlot = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NPCStateMachine = GetComponent<NPCStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if(interaction.ReserveSlot(this, out currentSlot))
            {
                Debug.Log($"Reserved {interaction.DisplayName}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractionActionObject interaction))
        {
            if (currentInteractionObject == null)
                return;

            interaction.ReleaseSlot(this, currentSlot);
            Debug.Log($"Released {interaction.DisplayName}");
        }
    }
}

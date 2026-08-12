using UnityEditor;
using UnityEngine;


public class InteractionActionObject : InteractionAction
{
    [System.Serializable]
    public class ObjectAction
    {
        public InteractionSlot slot;
        public GameObject actionObject;
        public HumanBodyBones occupantSocketAttachment;
        public Vector3 offsetPosition;
        public Vector3 offsetRotation;
    }

    [Header("Object Settings")]
    public ObjectAction actionObjectRole;

    public override void OnInteractionStart(NPCInteraction npc)
    {
        StartObjectInteraction();
    }

    public override void OnInteractionEnd(NPCInteraction npc)
    {
        EndObjectInteraction();
    }

    public override InteractionSlot GetFreeSlot()
    {
        if (actionObjectRole.slot.occupant == null)
            return actionObjectRole.slot;

        return null;
    }

    public override bool ReserveSlot(NPCInteraction npc, out InteractionSlot slot)
    {
        slot = GetFreeSlot();

        if (slot == null)
            return false;

        npc.currentInteractionObject = this;
        slot.occupant = npc;
        Debug.Log($"Reserved {npc.currentInteractionObject.DisplayName}, {slot.interactionMarker.name} has been selected!");
        return true;
    }

    public override void ReleaseSlot(NPCInteraction npc, InteractionSlot slot)
    {
        base.ReleaseSlot(actionObjectRole.slot.occupant, actionObjectRole.slot);
    }

    public void StartObjectInteraction()
    {
        if (actionObjectRole.slot == null || actionObjectRole.slot.occupant == null)
            return;

        if (!actionObjectRole.slot.occupant.isAtInteractionMarker)
            return;

        AttachObjectToBone(actionObjectRole, actionObjectRole.slot.occupant);
        actionObjectRole.slot.OnInteractionStart.Invoke();
        actionObjectRole.slot.occupant.GetNPCStateMachine().ReplaceAnimationClip
            (AnimationManager.instance.GetRandomAnimation(actionObjectRole.slot.occupant.CurrentSlot.interaction), "_Interact");
        actionObjectRole.slot.occupant.GetNPCStateMachine().Animator.SetTrigger("SetInteraction");
    }

    public void EndObjectInteraction()
    {
        actionObjectRole.actionObject.transform.SetParent(null);
        actionObjectRole.slot.OnInteractionEnd.Invoke();
        ReleaseSlot(actionObjectRole.slot.occupant, actionObjectRole.slot);
    }

    private void AttachObjectToBone(ObjectAction actionObject, NPCInteraction npc)
    {
        Transform bone = npc.GetBone(actionObject.occupantSocketAttachment);

        if (bone == null)
            return;

        Transform objectTransform = actionObjectRole.actionObject.transform;

        objectTransform.SetParent(bone);

        objectTransform.localPosition = actionObjectRole.offsetPosition;
        objectTransform.localEulerAngles = actionObjectRole.offsetRotation;
    }

    public override void InteractionDebugger()
    {
        if (actionObjectRole.slot.interactionMarker == null)
            return;

        if (actionObjectRole.slot.occupant)
            Gizmos.color = Color.red;
        else
            Gizmos.color = interactionMarkerColor;

        if (actionObjectRole.slot.roleName != string.Empty)
            actionObjectRole.slot.interactionMarker.gameObject.name = actionObjectRole.slot.roleName;

        Gizmos.DrawSphere(actionObjectRole.slot.interactionMarker.position, 0.3f);

        GUI.color = Color.white;
        Handles.Label(actionObjectRole.slot.interactionMarker.transform.position + Vector3.up * 0.5f, actionObjectRole.slot.interactionMarker.name);
    }

}

// implemented a quick editor logic to help cut out certain variables that not nesscary
[CustomEditor(typeof(InteractionActionObject))]
public class InteractionActionObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayName"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("actionObjectRole"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_interactionTag"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("postInteractionWaitTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionMarkerColor"));

        serializedObject.ApplyModifiedProperties();
    }
}
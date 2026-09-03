using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GoalLocation : MonoBehaviour
{
    public string locationName;
    public int locationID;
    public Transform entryPoint;
    public bool activeAtEntryPoint;

    [Header("Optional Area Settings")]
    public bool isArea;
    public Transform areaCenter;
    public float areaRadius;

    [Header("Animations To Play At Location")]
    public AnimationType[] clipsAtLocation;

    [Header("Goal Events")]
    public UnityEvent OnGoalEnter;
    public UnityEvent OnGoalExit;

    public void OnNPCEnter(NPCStateMachine npc)
    {
        npc.NPCModel.SetActive(activeAtEntryPoint);
        // npcs enter buildings and houses, no need to further the logic!
        if (!npc.NPCModel.activeSelf)
        {
            return;
        }

        npc.StopNPC(false);

        OnGoalEnter.Invoke();
    }

    public void OnNPCExit(NPCStateMachine npc)
    {
        if (!npc.NPCModel.activeSelf)
            npc.NPCModel.SetActive(true);

        npc.ResumeNPC();

        //refreshing npc
        if (isArea)
            npc.UpdateNodePath();

        OnGoalExit.Invoke();
    }

    public AnimationClip PlayAnimationAtLocation()
    {
        AnimationClip animationToPlay;

        if (clipsAtLocation.Length > 1)
        {
            animationToPlay = AnimationManager.instance.GetRandomAnimation(clipsAtLocation[UnityEngine.Random.Range(0, clipsAtLocation.Length)]);
            return animationToPlay;
        }
        else
        {
            animationToPlay = AnimationManager.instance.GetRandomAnimation(clipsAtLocation[0]);
            return animationToPlay;
        }
    }

    public Vector3 GetRandomPointInArea()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random = UnityEngine.Random.insideUnitCircle * areaRadius;
            Vector3 point = areaCenter.position + new Vector3(random.x, 0f, random.y);

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return areaCenter.position;
    }

    private void OnDrawGizmos()
    {
        if (isArea)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(areaCenter.transform.position, areaRadius);
        }
    }
}

[CustomEditor(typeof(GoalLocation))]
public class LocationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GoalLocation location = (GoalLocation)target;

        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate ID"))
        {

            location.locationID = GenerateID();

            serializedObject.ApplyModifiedProperties();
        }
        
    }

    int GenerateID()
    {
        int[] availableDigits = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
        int[] id = new int[] { 0, 0, 0, 0 };

        id[0] = availableDigits[UnityEngine.Random.Range(0, availableDigits.Length)];
        id[1] = availableDigits[UnityEngine.Random.Range(0, availableDigits.Length)];
        id[2] = availableDigits[UnityEngine.Random.Range(0, availableDigits.Length)];
        id[3] = availableDigits[UnityEngine.Random.Range(0, availableDigits.Length)];


        var finalID = int.Parse(string.Concat(id));

        return finalID;
    }

}

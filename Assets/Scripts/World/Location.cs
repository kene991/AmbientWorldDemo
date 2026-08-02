using UnityEngine;
using UnityEngine.AI;

public class Location : MonoBehaviour
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
    public AnimationClip[] clipsAtLocation;

    public void OnNPCEnter(NPCStateMachine npc)
    {
        npc.NPCModel.SetActive(activeAtEntryPoint);
        npc.StopNPC();

        // npcs enter buildings and houses, no need to further the logic!
        if (!npc.NPCModel.activeSelf)
        {
            return;
        }
    }

    public void OnNPCExit(NPCStateMachine npc)
    {
        if (!npc.NPCModel.activeSelf)
            npc.NPCModel.SetActive(true);

        npc.ResumeNPC();
    }

    public AnimationClip PlayAnimationAtLocation()
    {
        if (clipsAtLocation.Length > 1)
        {
            return clipsAtLocation[Random.Range(0, clipsAtLocation.Length)];
        }
        else
        {
            return clipsAtLocation[0];
        }
    }

    public Vector3 GetRandomPointInArea()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random = Random.insideUnitCircle * areaRadius;
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

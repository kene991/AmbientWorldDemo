using System.Collections.Generic;
using UnityEngine;

public class AIDirectorManager : MonoBehaviour
{
    public static AIDirectorManager instance;

    [Header("NPC Spawning")]
    [SerializeField] private NPCStateMachine[] npcPrefab;
    [SerializeField] private int maxNPCs = 20;

    private List<PathNode> pathNodes = new List<PathNode>();
    private List<NPCStateMachine> activeNPCs = new List<NPCStateMachine>();

    public RoutineSO[] routinesToGive;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pathNodes.AddRange(FindObjectsByType<PathNode>(FindObjectsSortMode.None));

        SpawnNPCs();
    }

    private void SpawnNPCs()
    {
        for (int i = 0; i < maxNPCs; i++)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (pathNodes.Count == 0)
            return;

        PathNode spawnNode = pathNodes[Random.Range(0, pathNodes.Count)];

        Vector3 spawnPosition = spawnNode.GetPosition();

        NPCStateMachine npc = Instantiate(npcPrefab[Random.Range(0, npcPrefab.Length)], spawnPosition, Quaternion.identity);

        if (Random.value > 0.5f && routinesToGive.Length > 0)
        {
            npc.Routine.routineSchedule = routinesToGive[Random.Range(0, routinesToGive.Length)];

            //helps capture time which I can now randomize
            npc.Routine.UpdateRoutine(GameClockManager.Instance.GetTimeSpanned);
        }
           

        npc.UpdateNodePath();

        activeNPCs.Add(npc);
    }
}

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

    //for only regular npcs, if no routine is selected.
    public List<RoutineSO> availableRoutines;

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

        if (!npc.Routine.routineSchedule)
        {
            if (Random.value > 0.5f && availableRoutines.Count > 0)
            {
                RoutineSO selectedRoutine = availableRoutines[Random.Range(0, availableRoutines.Count)];
                npc.Routine.routineSchedule = selectedRoutine;

                //helps capture time which I can now randomize
                npc.Routine.UpdateRoutine(GameClockManager.Instance.GetTimeSpanned);

                availableRoutines.Remove(selectedRoutine);
            }
        }  

        npc.UpdateNodePath();

        activeNPCs.Add(npc);
    }
}

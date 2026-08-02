using System.Collections.Generic;
using UnityEngine;

public class RoutineManager : MonoBehaviour
{
    [Header("Available Idle Actions")]
    public AnimationClip[] idleClips;

    [HideInInspector] public List<NPCRoutine> routineNPCs;

}



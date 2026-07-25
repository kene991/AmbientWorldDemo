using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Routine", menuName = "NPC/Create A New Routine")]
public class RoutineSO : ScriptableObject
{
    public List<RoutineBlock> blocks;

    public RoutineBlock GetBlock(float currentTime)
    {
        foreach (RoutineBlock block in blocks)
        {
            if (IsTimeWithinBlock(currentTime, block))
            {
                return block;
            }
        }

        return null;
    }

    // checking is block takes place during the day or overnight
    private bool IsTimeWithinBlock(float currentTime, RoutineBlock block)
    {
        if (block.startHour < block.endHour)
        {
            // during the day
            return currentTime >= block.startHour && currentTime < block.endHour;
        }
        else
        {
            // during overnight hours
            return currentTime >= block.startHour || currentTime < block.endHour;
        }
    }
}

[System.Serializable]
public class RoutineBlock
{
    [Header("Setup")]
    public string blockName;

    [Range(0, 23)]
    public int startHour;

    [Range(0, 23)]
    public int endHour;

    //[Header("Destination Settings")]
    //public Transform destination;
    //public bool FindRandomPointAtDestination;

    //[Header("Reached Destination Settings")]
    //public bool activeAtDestinationReached;
    //public bool OrientToDestinationPosition;
    //public AnimationClip clipAtDestination;

    //[Header("Debug")]
    //public Color blockDebugColor;
}


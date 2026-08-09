using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Day Cycle", menuName = "Environment/Create New Day Cycle", order = 1)]
public class EnvironmentForcastCycle : ScriptableObject
{
    public Gradient skyLighting;
    public Gradient sky;
    public Gradient equator;
    public Gradient ground;

    [Header("Weather")]
    public bool enableRain;
    [Range(0, 24)]
    public int startTimespanForRain;
    [Range(0, 24)]
    public int endTimespanForRain;    

    [Header("Stars")]
    public AnimationCurve starHeightMask;    
    
    [Header("Cloud")]
    public AnimationCurve cloudHeightMask;

}

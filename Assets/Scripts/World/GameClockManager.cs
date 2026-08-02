using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum TimeOfDay
{
    MORNING,
    AFTERNOON,
    EVENING,
    OVERNIGHT
}
public class GameClockManager : MonoBehaviour
{
    public static GameClockManager Instance;

    public TimeOfDay timeOfDay;
    [SerializeField] private string currentTime;

    [Header("Time Clock Settings")]
    public bool UseMilitaryTime;
    [SerializeField] private float timeScale = 1.0f;
    public string CurrentTimeString { get { return currentTime; } }
    public float ElaspedTime { get { return elaspedTime; } }
    public int GetTimeSpanned { get { return timeSpan.Hours; } }

    public Action<int> OnHourChanged;

    float elaspedTime;
    TimeSpan timeSpan;
    int h;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        TimeTick();
    }

    private void TimeTick()
    {
        elaspedTime += Time.deltaTime * timeScale;

        //using TimeSpan from the Systems library to format the time.
        timeSpan = TimeSpan.FromSeconds(elaspedTime);
        currentTime = DisplayTime();
        UpdateTimeOfDay();
    }

    /// <summary>
    /// Display Timer
    /// </summary>
    /// <returns>How time is being formatted</returns>
    public string DisplayTime()
    {
        if (UseMilitaryTime)
        {
            return string.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
        }
        else
        {
            DateTime time = DateTime.Today.Add(timeSpan);
            return time.ToString("hh:mm tt"); //interesting this updated the clock using the AM or PM format 
        }
    }


    private void UpdateTimeOfDay()
    {
        h = timeSpan.Hours;

        if (h >= 0 && 6 >= h)
        {
            timeOfDay = TimeOfDay.OVERNIGHT;
        }

        if (h >= 6 && 12 >= h)
        {
            timeOfDay = TimeOfDay.MORNING;
        }        
        
        if (h >= 12 && 18 >= h)
        {
            timeOfDay = TimeOfDay.AFTERNOON;
        }        
        
        if (h >= 18 && 24 >= h)
        {
            timeOfDay = TimeOfDay.EVENING;
        }
    }

}

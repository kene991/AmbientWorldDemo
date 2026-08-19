using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance;

    private GameClockManager _clockManager;
    [SerializeField] private Light directionalLight;
    [SerializeField] Material _skybox;
    int selectedForcast;
    private readonly float duration = 24f;

    //Think of this as customizing graident for a full day 
    [Header("Picked Forcast")]
    [SerializeField] EnvironmentForcastCycle debugForcast;
    public EnvironmentForcastCycle[] forcastCycles;

    [Header("Weather Forcast Settings")]
    public ParticleSystem rainWeather;
    public bool isRaining;

    private void Awake()
    {
        _clockManager = GetComponent<GameClockManager>();
        instance = this;
    }

    private void OnEnable()
    {
        _clockManager.OnHourChanged += UpdateForcast;
    }

    private void OnDisable()
    {
        _clockManager.OnHourChanged -= UpdateForcast;
    }

    void Start()
    {
        _skybox = new Material(_skybox);
        UpdateForcast(_clockManager.GetTimeSpanned);
    }

    // Update is called once per frame
    void Update()
    {
        if (!debugForcast)
            return;

        SkyTimeTranisition();
    }

    public void SkyTimeTranisition()
    {
        //applying update from the select forcast
        ApplyCycleMaterialUpdate();
        RenderSettings.skybox = _skybox;
    }

    private void UpdateForcast(int hour)
    {
        if (debugForcast)
        {
            ShouldRain(hour);
        }

        if (hour != 0)
            return;

        selectedForcast = UnityEngine.Random.Range(0, forcastCycles.Length);
        debugForcast = forcastCycles[selectedForcast];
    }

    private void ApplyCycleMaterialUpdate()
    {
        float normalizedTime = Mathf.Clamp01((float)_clockManager.GetTimeSpanned / duration);

        directionalLight.color = EvaluateTimeSpanColor(debugForcast.skyLighting);
        _skybox.SetColor("_SkyColor", EvaluateTimeSpanColor(debugForcast.sky));
        _skybox.SetColor("_EquatorColor", EvaluateTimeSpanColor(debugForcast.equator));
        _skybox.SetColor("_GroundColor", EvaluateTimeSpanColor(debugForcast.ground));

        _skybox.SetFloat("_StarsHeightMask", debugForcast.starHeightMask.Evaluate(normalizedTime));
        _skybox.SetFloat("_CloudsHeight", debugForcast.cloudHeightMask.Evaluate(normalizedTime));
    }

    private void ShouldRain(int hour)
    {
        if (!debugForcast.enableRain)
            return;

        SetRain(IsWithinRainTimeSpan(hour));
    }

    private void SetRain(bool shouldRain)
    {
        ////so we don't call this again
        //if (isRaining == shouldRain)
        //    return;

        isRaining = shouldRain;

        if (isRaining)
        {
            if (!rainWeather.isPlaying)
                rainWeather.Play();
            //rain moves with player
            //newPosition.y = rainWeather.transform.position.y;
            //rainWeather.transform.position = newPosition;
        }
        else
        {
            if (rainWeather.isPlaying)
                rainWeather.Stop();
        }
    }

    private Color EvaluateTimeSpanColor(Gradient color)
    {
        return color.Evaluate(Mathf.Clamp01((float)_clockManager.GetTimeSpanned / duration));
    }

    private bool IsWithinRainTimeSpan(float currentTime)
    {
        if (debugForcast.startTimespanForRain < debugForcast.endTimespanForRain)
        {
            // during the day
            return currentTime >= debugForcast.startTimespanForRain && currentTime < debugForcast.endTimespanForRain;
        }
        else
        {
            // during overnight hours
            return currentTime >= debugForcast.startTimespanForRain || currentTime < debugForcast.endTimespanForRain;
        }
    }
}

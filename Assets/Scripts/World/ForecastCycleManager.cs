using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForecastCycleManager : MonoBehaviour
{

    private GameClockManager _clockManager;
    [SerializeField] private Light directionalLight;
    [SerializeField] Material _skybox;
    int selectedForcast;
    bool _isNewDay = false;
    private readonly float duration = 24f;

    //Think of this as customizing graident for a full day 
    [Header("Picked Forcast")]
    [SerializeField] EnvironmentForcastCycle debugForcast;
    public EnvironmentForcastCycle[] forcastCycles;

    [Header("Weather Forcast Settings")]
    public ParticleSystem rainWeather;
    public bool isRaining;

    // Start is called before the first frame update
    void Start()
    {
        _skybox = new Material(_skybox);
        _clockManager = GetComponent<GameClockManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SkyTimeTranisition();
    }

    public void SkyTimeTranisition()
    {
        UpdateForcast();

        //applying update from the select forcast
        ApplyCycleMaterialUpdate();
        RenderSettings.skybox = _skybox;
    }

    private void UpdateForcast()
    {
        //Changing the weather cycle throughout the night
        if (_clockManager.GetTimeSpanned == 0 && !_isNewDay)
        {
            selectedForcast = Random.Range(0, forcastCycles.Length);
            debugForcast = forcastCycles[selectedForcast];
            _isNewDay = true;
        }
        else if (_clockManager.GetTimeSpanned == 1 && _isNewDay)
        {
            _isNewDay = false;
        }
    }

    private void ApplyCycleMaterialUpdate()
    {
        directionalLight.color = EvaluateTimeSpanColor(debugForcast.skyLighting);

        if (debugForcast.enableRain)
        {
            if (_clockManager.GetTimeSpanned > debugForcast.startTimespanForRain && debugForcast.endTimespanForRain > _clockManager.GetTimeSpanned)
            {
                isRaining = true;

                rainWeather.Play();

                //rain moves with player
                //newPosition.y = rainWeather.transform.position.y;
                //rainWeather.transform.position = newPosition;
            }
            else
            {
                rainWeather.Stop();
                isRaining = false;
            }
        }

        _skybox.SetColor("_SkyColor", EvaluateTimeSpanColor(debugForcast.sky));
        _skybox.SetColor("_EquatorColor", EvaluateTimeSpanColor(debugForcast.equator));
        _skybox.SetColor("_GroundColor", EvaluateTimeSpanColor(debugForcast.ground));

        _skybox.SetFloat("_StarsHeightMask", debugForcast.starHeightMask.Evaluate((float)_clockManager.GetTimeSpanned / duration));
        _skybox.SetFloat("_CloudsHeight", debugForcast.cloudHeightMask.Evaluate((float)_clockManager.GetTimeSpanned / duration));
    } 

    private Color EvaluateTimeSpanColor(Gradient color)
    {
        return color.Evaluate((float)_clockManager.GetTimeSpanned / duration);
    }
}

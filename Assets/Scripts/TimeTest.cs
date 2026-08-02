using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeTest : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay;

    // Update is called once per frame
    void Update()
    {
        timeDisplay.text = GameClockManager.Instance.CurrentTimeString + "\n" + GameClockManager.Instance.timeOfDay; 
    }
}

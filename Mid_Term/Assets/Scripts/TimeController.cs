using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TimeController : MonoBehaviour
{
    [SerializeField] private float timeMultiplier;
    [SerializeField] private float startHour;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Light sunLight;
    [SerializeField] private float sunriseHour;
    [SerializeField] private float sunsetHour;
    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;
    [SerializeField] AnimationCurve lightChangeCurve;
    [SerializeField] private float maxSunLightIntensity;
    [SerializeField] private Light moonLight;
    [SerializeField] private float maxMoonLightIntensity;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    private DateTime currentTime;


    // Start is called before the first frame update
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if(timeText != null )
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
        float sunlightRotation;

        //checking if current time is between sunrise and susnset to determin if in daytime
        if(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime) 
        {
            //if it is daytime calculate the total time between sunrise and sunset
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            //find how much time has passed since sunrise
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            //find the total percentage of daytime that has passed
            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            //sets the rotaton to 0 at sunrise and 180 at sunset
            sunlightRotation = Mathf.Lerp(0, 180, (float)(percentage));
        }
        else
        {
            //calculate the time between sunset and sunrise
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            // find how much time has passed since sunset
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            //find the total percentage of night that has passed
            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            //sets the rotation to 180 and 360
            sunlightRotation = Mathf.Lerp(180, 360, (float)(percentage));
        }
        //apply the rotation to the sunlight
        sunLight.transform.rotation = Quaternion.AngleAxis(sunlightRotation, Vector3.right);
        

    }

    private void UpdateLightSettings()
    {
        //gets a value between 1 and -1 depedending on how similar the vectors are, if sun is pointing directly down you get 1. 
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        //set the intensity of the sun between 0 and max
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, dotProduct);
        //lerp the other way around for moonlight
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, dotProduct);
        //set the ambient lerp to transition from day to night ambient
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    //gets the difference between times to help determin how close it is to sunset or sunrise
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;

        if(diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24);
        }
        return diff;
    }
}

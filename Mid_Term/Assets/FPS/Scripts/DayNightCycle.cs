/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class DayNightCycle : MonoBehaviour
    {
        [Header("----Time of Day----")]
        [Tooltip("Length of Day in Minutes")]
        [SerializeField] float dayLength;
        [SerializeField] [Range(0 , 1)] float timeOfDay;
        [SerializeField] int dayNumber;
        [SerializeField] int yearNumber;
        [SerializeField] int monthNumber;
        [SerializeField] int yearLength;
        private float timeScale;
        

        [Header("----Sun Light----")]
        [SerializeField]Transform dayRotation;
        [SerializeField] Light sun;
        [SerializeField] float sunIntensity;
        [SerializeField] float sunVariation;
        [SerializeField] float sunBaseIntensity;
        [SerializeField] Gradient sunColor;



        private void Update()
        {
            if(!GameManager.instance.isPaused)
            {
                UpdateTimeScale();
                UpdateTime();
            }
            SunRotation();
            SunLightIntensity();
        }

        private void UpdateTimeScale()
        {
            // dayLength / 60 gives the fraction of a hour that we want the day to be. Then dividing 24 by that gives us the time scale.
            timeScale = 24 / (dayLength / 60);
        }

        private void UpdateTime()
        {
            // takes length of the last frame in seconds, 86400 is the amount of seconds in a 24 hour day, this gives you the current time.
            timeOfDay += Time.deltaTime * timeScale / 86400;
            // if true then it is a new day
            if (timeOfDay > 1)
            {
                // increase our day
                dayNumber++;
                // subtract one from timeOfDay so the day restarts
                timeOfDay -= 1;
                // if true then it's a new year
                if (dayNumber > yearLength)
                {
                    yearNumber++;
                    dayNumber = 0;
                }
            }
        }

        private void SunRotation()
        {
            float sunAngle = timeOfDay * 360;
            //rotates the sun on the z axis
            dayRotation.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, sunAngle));
        }

        private void SunLightIntensity()
        {
            sunIntensity = Vector3.Dot(sun.transform.forward, Vector3.down);
            // makes sure valuse is 0 and not negative
            sunIntensity = Mathf.Clamp01(sunIntensity);

            sun.intensity = sunIntensity * sunVariation + sunBaseIntensity;
        }

        private void ChangeSunColor()
        {
            sun.color = sunColor.Evaluate(sunIntensity);
        }

    }
}

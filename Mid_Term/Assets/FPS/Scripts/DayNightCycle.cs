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
using UnityEngine.Rendering.HighDefinition;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class DayNightCycle : MonoBehaviour
    {
<<<<<<< Updated upstream
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


        public bool pause = false;

=======
        ///**----------------------------------------------------------------
        // * @brief
        // */
        [Range(0, 1)][SerializeField] float time;
        [SerializeField] float dayLength;
        [SerializeField] float timeStart;

        private float timeRate;
        public Vector3 noon;

        public Light sun;
        public Gradient sunColor;
        public AnimationCurve sunIntensity;
        public Light moon;
        public Gradient moonColor;
        public AnimationCurve moonIntensity;

        public AnimationCurve lightingIntensityMulti;
        public AnimationCurve reflectionIntensityMulti;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            timeRate = 1.0f / dayLength;
            time = timeStart;
        }
>>>>>>> Stashed changes

        private void Update()
        {
<<<<<<< Updated upstream
            if(!pause)
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
            if(timeOfDay > 1 )
            {
                // increase our day
                dayNumber++;
                // subtract one from timeOfDay so the day restarts
                timeOfDay -= 1;
                // if true then it's a new year
                if(dayNumber > yearLength ) 
                {
                    yearNumber++;
                    dayNumber = 0;
                }
=======
            // increase time
            time += timeRate * Time.deltaTime;
            if (time >= 1.0f) 
            {
                time = 0.0f;
>>>>>>> Stashed changes
            }

            // light rotation
            sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
            moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;


            // light intensity
            sun.intensity = sunIntensity.Evaluate(time);
            moon.intensity = moonIntensity.Evaluate(time);
        }

<<<<<<< Updated upstream

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
=======

>>>>>>> Stashed changes

    }
}

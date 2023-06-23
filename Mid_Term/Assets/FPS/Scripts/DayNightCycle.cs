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
        ///**----------------------------------------------------------------
        // * @brief
        // */
        [Range(0, 24)][SerializeField] float timeOfDay;
        public Light sun;
        public Light moon;
        public float orbitSpeed = 1.0f;
        bool isNight;
        public Volume skyVolume;
        public PhysicallyBasedSky sky;
        public AnimationCurve starsCurve;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            skyVolume.profile.TryGet(out sky);
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            timeOfDay += Time.deltaTime * orbitSpeed;
            if (timeOfDay > 24)
            {
                timeOfDay = 0;
            }
            UpdateTime();
        }


        private void OnValidate()
        {
            skyVolume.profile.TryGet(out sky);
            UpdateTime();
        }

        private void UpdateTime()
        {
            float alpha = timeOfDay / 24.0f;
            float sunRotation = Mathf.Lerp(-90, 270, alpha);
            float moonRotations = sunRotation - 180;

            sun.transform.rotation = Quaternion.Euler(sunRotation, 0, 0);
            moon.transform.rotation = Quaternion.Euler(moonRotations, 0, 0);

            sky.spaceEmissionMultiplier.value = starsCurve.Evaluate(alpha) * 100f;

            DayToNight();
        }

        private void DayToNight()
        {
            if(isNight) 
            {
                if(moon.transform.rotation.eulerAngles.x > 180)
                {
                    StartDay();
                }
            }
            else
            {
                if(sun.transform.rotation.eulerAngles.x > 180)
                {
                    StartNight();
                }
            }

        }

        private void StartNight()
        {
            isNight = true;
            sun.shadows = LightShadows.None;
            moon.shadows = LightShadows.Soft;
        }

        private void StartDay()
        {
            isNight = false;
            sun.shadows = LightShadows.Soft;
            moon.shadows = LightShadows.None;
        }
    }
}

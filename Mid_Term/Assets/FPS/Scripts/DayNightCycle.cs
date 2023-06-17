/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class DayNightCycle : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        public float currentTime;
        public float dayLengthMinutes;
        private float rotationSpeed;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            rotationSpeed = 360 / dayLengthMinutes / 60;
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            currentTime += 1 * Time.deltaTime;

            transform.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime);
        }
    }
}

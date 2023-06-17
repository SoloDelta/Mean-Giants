/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class AimScript : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        public GameObject Gun;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Gun.GetComponent<Animator>().Play("Aim");
            }

            if (Input.GetMouseButtonUp(1))
            {
                Gun.GetComponent<Animator>().Play("UnAim");
            }
        }
    }
}

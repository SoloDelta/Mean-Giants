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
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief 
     */
    public class LaserSight : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            lineRenderer.SetPosition(0, transform.position);
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit))
            {
                lineRenderer.SetPosition(1, hit.point);

            }
            else
            {
                lineRenderer.SetPosition(1, transform.position +  (transform.forward * 5000));
            }
        }
    }
}

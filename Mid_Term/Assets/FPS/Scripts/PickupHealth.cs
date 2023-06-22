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
    public class PickupHealth : MonoBehaviour
    {
        public int healthAmount;

        private void OnTriggerEnter(Collider other)
        {
            IHealth hasAmmo = other.GetComponent<IHealth>();

            if (hasAmmo != null)
            {
                hasAmmo.healthPickup(healthAmount, gameObject);
            }
        }
    }
}
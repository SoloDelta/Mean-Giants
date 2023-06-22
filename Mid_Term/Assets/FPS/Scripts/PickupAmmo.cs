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
     * @brief Simple class to handle picking up addition ammo.
     */
    public class PickupAmmo : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        public int ammoAmount;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void OnTriggerEnter(Collider other)
        {
            IAmmo hasAmmo = other.GetComponent<IAmmo>();

            if (hasAmmo != null)
            {
                hasAmmo.AmmoPickup(ammoAmount, gameObject);
            }
        }
    }
}

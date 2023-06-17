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
     * @brief Handles picking weapons when within trigger.
     */
    public class PickupGun : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        [SerializeField] private GunStats gun;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.instance.playerScript.PickupGun(gun);
                Destroy(gameObject);
            }
        }

    }
}

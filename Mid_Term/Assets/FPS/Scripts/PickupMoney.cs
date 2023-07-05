using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PickupMoney : MonoBehaviour
    {
        /**----------------------------------------------------------------
         * @brief
         */
        public int moneyAmount;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void OnTriggerEnter(Collider other)
        {
            IMoney hasMoney = other.GetComponent<IMoney>();

            if (hasMoney != null)
            {
                hasMoney.MoneyPickup(moneyAmount, gameObject);
            }
        }
    }
}

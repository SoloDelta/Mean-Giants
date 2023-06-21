using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
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

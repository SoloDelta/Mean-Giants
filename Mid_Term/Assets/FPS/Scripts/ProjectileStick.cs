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
    public class ProjectileStick : MonoBehaviour
    {
        private Rigidbody rb;

        private bool targetHit;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (targetHit)  //sticks to first object collided   
                return;
            else
                targetHit = true;


            rb.isKinematic = true; //sticks to surface

            transform.SetParent(collision.transform); //moves with object stuck to
            
        }
    }
}

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
     * @brief Represents a bullet in our game.
     */
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet")]
        [SerializeField] int damage;
        [SerializeField] int speed;
        [SerializeField] float destroyTime;

        [SerializeField] Rigidbody rb;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            Destroy(gameObject, destroyTime);
            rb.velocity = transform.forward * speed;
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void OnTriggerEnter(Collider other)
        {
            IDamage dam = other.GetComponent<IDamage>();

            if(dam != null)
            {
                dam.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}

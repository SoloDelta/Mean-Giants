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
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet")]
        [SerializeField] int damage;
        [SerializeField] int speed;
        [SerializeField] float destroyTime;

        [SerializeField] Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, destroyTime);
            rb.velocity = transform.forward * speed;
        }

        // Update is called once per frame
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

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
    public class Grenade : MonoBehaviour
    {
        public float delay = 3f;
        public float radius = 5f;
        public float force = 700f;

        public GameObject explosionEffect;

        float countdown;

        bool isExploded = false;

        void Start()
        {
            countdown = delay;
        }

        // Update is called once per frame
        void Update()
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0f && !isExploded)
            {
                Explode();
                isExploded = true;
            }
        }

        void Explode()
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(force, transform.position, radius);
                }
            }

            Destroy(gameObject);
        }
    }
}

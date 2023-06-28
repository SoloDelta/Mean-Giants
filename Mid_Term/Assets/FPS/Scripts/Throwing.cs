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
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class Throwing : MonoBehaviour
    {
        [Header("References")]
        public Transform cam;
        public Transform attackPoint;
        public GameObject objectToThrow;

        [Header("Settings")]
        [SerializeField] float destroyTime;
        public int totalThrows;
        public float throwCooldown;
        
        [Header("Throwing")]
        public KeyCode throwKey = KeyCode.Mouse0;
        public float throwForce;
        public float throwUpwardForce;

        public AudioSource aud;
        [SerializeField] AudioClip audKnife;
        [Range(0, 1)][SerializeField] float audKnifeVol;

        bool readyToThrow;

        private void Start()
        {
            
            readyToThrow = true; 
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && readyToThrow && totalThrows > 0)
            {
                Throw();
                aud.PlayOneShot(audKnife, audKnifeVol);
                StartCoroutine(DestroyeKnife());
            }
        }

        public IEnumerator DestroyeKnife()
        {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }
        private void Throw()
        {
            readyToThrow = false;

            GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation); //Int Object

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>(); //Ridigbody component

            Vector3 forceDirection = cam.transform.forward; //direction

            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
            {
                forceDirection = (hit.point - attackPoint.position).normalized;
            }

            Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce; // Force 

            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

            totalThrows--;

            Invoke(nameof(ResetThrow), throwCooldown);
        }

        private void ResetThrow()
        {
            readyToThrow = true;
        }
    }
}
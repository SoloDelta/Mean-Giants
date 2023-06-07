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
     * @brief
     */
    public class PlayerController : MonoBehaviour, IDamage
    {
        [Header("----- Components -----")]
        [SerializeField] CharacterController controller;

        [Header("----- Player Stats -----")]
        [SerializeField] int health;
        [Range(3, 8)][SerializeField] float playerSpeed;
        [Range(8, 25)][SerializeField] float jumpHeight;
        [Range(10, 50)][SerializeField] float gravityValue;
        [Range(1, 3)][SerializeField] int jumpMax;

        [Header("----- Gun Stats -----")]
        [Range(0.1f, 3)][SerializeField] float shootRate;
        [Range(1, 10)][SerializeField] int shootDamage;
        [Range(1, 1000)][SerializeField] int shootDistance;
        [SerializeField] GameObject hitEffect;

        private int jumpedTimes;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private Vector3 move;
        bool isShooting;
        int playerHpOrig;

        private void Start()
        {
            SpawnPlayer();
        }

        private void Update()
        {
            Movement();

            if (Input.GetButton("Shoot") && !isShooting)
            {
                StartCoroutine(shoot());
            }
        }

        void Movement()
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
                jumpedTimes = 0;
            }

            move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player
            if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
            {
                jumpedTimes++;
                playerVelocity.y = jumpHeight;
            }

            playerVelocity.y -= gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        IEnumerator shoot()
        {
            isShooting = true;
            RaycastHit hit;
          
            if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                Debug.Log(hit);
                IDamage damageable = hit.collider.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.TakeDamage(shootDamage);
                }
            }
            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                //GameManager.Instance.YouLose();
            }
        }

        public void UpdatePlayerHp()
        {
            //GameManager.Instance.playerHpBar.fillAmount = (float)health / playerHpOrig;
        }

        public void SpawnPlayer()
        {
            controller.enabled = false;
            //transform.position = GameManager.Instance.playerSpawnPos.transform.position;
            controller.enabled = true;
            health = playerHpOrig;
            UpdatePlayerHp();
        }
    }
}
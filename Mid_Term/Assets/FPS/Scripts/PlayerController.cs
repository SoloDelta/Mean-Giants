/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - Todd Zipp
 *  - Daniel I. Dorn <didorn@student.fullsail.edu>
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class PlayerController : MonoBehaviour, IDamage, IAmmo
    {
        [Header("----- Components -----")]
        [SerializeField] CharacterController controller;

        [Header("----- Player Stats -----")]
        [SerializeField] int health;
        [Range(3, 8)][SerializeField] float playerSpeed;
        [Range(8, 25)][SerializeField] float jumpHeight;
        [Range(10, 50)][SerializeField] float gravityValue;
        [Range(1, 3)][SerializeField] int jumpMax;
        [Range(2, 5)][SerializeField] int sprint;

        [Header("----- Gun Stats -----")]
        [SerializeField] List<GunStats> gunList = new List<GunStats>();
        [SerializeField] GameObject gunModel;
        [Range(0.1f, 3)][SerializeField] float shootRate;
        [Range(1, 10)][SerializeField] int shootDamage;
        [Range(1, 1000)][SerializeField] int shootDistance;
        [SerializeField] GameObject hitEffect;

        [Header("----- Weapon Componenets -----")]
        //public AudioSource gunshotSource;
        public AudioClip gClip;
        public ParticleSystem muzzleFlash;


        private int jumpedTimes;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private Vector3 move;
        bool isShooting;
        int playerHpOrig;
        int selectedGun;
        Coroutine lastRun;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {
            
            playerHpOrig = health;
            UpdatePlayerHp();
            SpawnPlayer();
        }

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            Movement();
            crouch();
            swapGun();

            if (Input.GetButton("Shoot") && !isShooting)
            {
                StartCoroutine(shoot());
            }

            if (playerHpOrig == health)
            {
                GameManager.instance.addCorner();
            }
            else
            {
                GameManager.instance.removeCorner();
            }
            showEnemyHP();
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
            if(Input.GetButtonDown("Sprint"))
            {
                playerSpeed += sprint;
            }
            if(Input.GetButtonUp("Sprint"))
            {
                playerSpeed -= sprint;
            }

            playerVelocity.y -= gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        public void crouch()
        {
            if (Input.GetButtonDown("Crouch"))
            {
                controller.height = controller.height / 2;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                controller.height = controller.height * 2;
            }
        }
        IEnumerator shoot()
        {
            if (gunList[selectedGun].curAmmo > 0)
            {
                muzzleFlash.Play();
                gunList[selectedGun].curAmmo--;
                //gunshotSource.PlayOneShot(gClip);   
                isShooting = true;
                RaycastHit hit;



                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
                {

                    IDamage damageable = hit.collider.GetComponent<IDamage>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(shootDamage);
                    }
                }
                yield return new WaitForSeconds(shootRate);
                isShooting = false;
            }
            
        }
        void showEnemyHP()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {

                if (hit.collider.CompareTag("Enemy"))
                {
                    if (lastRun != null)
                    {
                        StopCoroutine(lastRun);
                    }
                    hit.collider.GetComponent<EnemyAI>().wholeHealthBar.SetActive(true);
                    lastRun = StartCoroutine(turnOffEnemyHP(hit));

                }

            }
        }
        IEnumerator turnOffEnemyHP(RaycastHit _hit)
        {
            yield return new WaitForSeconds(1);
            if (_hit.collider != null)
            {
                _hit.collider.GetComponent<EnemyAI>().wholeHealthBar.SetActive(false);
            }

        }
        public void TakeDamage(int damage)
        {
            health -= damage;
            
            if (health <= 0)
            {
                GameManager.instance.YouLose();
            }
            UpdatePlayerHp();
            StartCoroutine(playerFlashDamage());
        }

        public void UpdatePlayerHp()
        {
            GameManager.instance.playerHpBar.fillAmount = (float) health / playerHpOrig;
        }

        public void SpawnPlayer()
        {
            controller.enabled = false;
            transform.position = GameManager.instance.playerSpawnPos.transform.position;
            controller.enabled = true;
            health = playerHpOrig;
            UpdatePlayerHp();
        }

        IEnumerator playerFlashDamage()
        {
            GameManager.instance.playerFlashUI.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.playerFlashUI.SetActive(false);
        }
       /* IEnumerator Flash()
        {
            GameManager.instance.bulletFlash.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.bulletFlash.SetActive(false);
        }*/

        public void PickupGun(GunStats gunstat)
        {
            gunList.Add(gunstat);

            shootDamage = gunstat.shootDamage;
            shootDistance = gunstat.shootDistance;
            shootRate = gunstat.shootRate;

            gunModel.GetComponent<MeshFilter>().mesh = gunstat.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().material = gunstat.model.GetComponent<MeshRenderer>().sharedMaterial;
            selectedGun = gunList.Count - 1;
        }

        public void swapGun()
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
            {
                selectedGun++;
                swapGunStats();
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                swapGunStats();
            }
        }

        void swapGunStats()
        {
            shootDamage = gunList[selectedGun].shootDamage;
            shootDistance = gunList[selectedGun].shootDistance;
            shootRate = gunList[selectedGun].shootRate;
            
            gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        }

        public void AmmoPickup(int amount, GameObject obj)
        {
            if(gunList.Count> 0)
            {
                int ammoDiffer = gunList[selectedGun].maxAmmo - gunList[selectedGun].curAmmo;
                gunList[selectedGun].curAmmo += amount;

                if (gunList[selectedGun].curAmmo > gunList[selectedGun].maxAmmo)
                {
                    gunList[selectedGun].curAmmo = gunList[selectedGun].maxAmmo;
                }

                PickupAmmo ammoPickup = obj.GetComponent<PickupAmmo>();
                ammoPickup.ammoAmount -= ammoDiffer;

                if(ammoPickup.ammoAmount <= 0)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
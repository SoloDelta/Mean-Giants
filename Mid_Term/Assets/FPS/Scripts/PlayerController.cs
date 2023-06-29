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

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class PlayerController : MonoBehaviour, IDamage, IHealth, IAmmo
    {
        [Header("----- Components -----")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private Gradient healthBarGradient;
        [SerializeField] private Animator anim;

        [Header("----- Player Stats -----")]
        [SerializeField] private int health;
        [Range(3, 8)][SerializeField] private float playerSpeed;
        [Range(8, 25)][SerializeField] private float jumpHeight;
        [Range(10, 50)][SerializeField] private float gravityValue;
        [Range(1, 3)][SerializeField] private int jumpMax;
        [Range(2, 5)][SerializeField] private int sprint;
        [SerializeField] private int stamina;

        [Header("----- Gun Stats -----")]
        [SerializeField] private List<GunStats> gunList = new List<GunStats>();
        [SerializeField] private GameObject gunModel;
        [Range(0.1f, 3)][SerializeField] private float shootRate;
        [Range(1, 10)][SerializeField] private int shootDamage;
        [Range(1, 1000)][SerializeField] private int shootDistance;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] float zoomIn;
        [Range(1, 15)][SerializeField] private int zoomInSpeed;
        [Range(1, 15)][SerializeField] private int zoomOutSpeed;

        [Header("----- Weapon Componenets -----")] 
        public ParticleSystem muzzleFlash;

        [Header("----- Audio -----")]
        public AudioSource aud;
        [SerializeField] AudioClip pickupClip;
        [SerializeField][Range(0, 1)] float pickupVol;
        [SerializeField] AudioClip gunShot;
        [SerializeField][Range(0, 1)] float shotVol;
        [SerializeField] AudioClip[] audJump;
        [Range(0, 1)][SerializeField] float audJumpVol;
        [SerializeField] AudioClip[] audDamage;
        [Range(0, 1)][SerializeField] float audDamageVol;
        [SerializeField] AudioClip[] audSteps;
        [Range(0, 1)][SerializeField] float audStepsVol;
        [SerializeField] AudioClip audCrouch;
        [Range(0, 1)][SerializeField] float audCrouchVol;
        [SerializeField] AudioClip[] audHurt;
        [Range(0, 1)][SerializeField] float audHurtVol;
        [SerializeField] AudioClip audReload;
        [Range(0, 1)][SerializeField] float audReloadVol;
        [SerializeField] AudioClip healthClip;
        [SerializeField][Range(0, 1)] float healthVol;
        [SerializeField] AudioClip emptyClipAud;
        [Range(0, 1)][SerializeField] float emptyClipVol;



        private int jumpedTimes;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private Vector3 move;
        private bool isShooting;
        private int playerHpOrig;
        private int playerStaminaOrig;
        private int selectedGun;
        private Coroutine lastRun;
        float zoomOrig;
        public bool isCrouching;
        bool isSprinting;
        float speedOrig;
        bool stepsPlaying;
        bool isReloading;
        public int ammoStorage;
        bool hasCellKey = false;

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Start()
        {  
            speedOrig = playerSpeed;
            playerHpOrig = health;
            playerStaminaOrig = stamina;
            UpdatePlayerHp();
            SpawnPlayer();
            UpdatePlayerStamina();
            zoomOrig = Camera.main.fieldOfView;
        }
        

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        void Update()
        {
            Sprint();
            zoomSights();

            if (GameManager.instance.activeMenu == null)
            {
                Movement();
                crouch();
                

                if (gunList.Count > 0)
                {
                    swapGun();

                    if (Input.GetButton("Shoot") && !isShooting)
                    {
                        StartCoroutine(shoot());
                    }
                }

                StartCoroutine(Reload());
                



                if (playerHpOrig == health)
                {
                    GameManager.instance.addCorner();
                }
                else
                {
                    GameManager.instance.removeCorner();
                }
                showEnemyHP();
                /////wrote this to collect the cube, not sure if this is the best place to call it but it works. - john
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
                {
                    if (hit.collider.CompareTag("Pickup"))
                    {
                        if (hit.distance < 3)
                        {
                            if (Input.GetButtonDown("Interact"))
                            {

                                StartCoroutine(collectedText(hit));
                                Destroy(hit.collider.gameObject);
                                Debug.Log("Cube Collected");
                            }

                        }

                    }
                }
            }
        }
        

        /**----------------------------------------------------------------
         * @brief
         */
        private void Movement()
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer)
            {
                if (!stepsPlaying && move.normalized.magnitude > 0.5f)
                {
                    StartCoroutine(PlaySteps());
                }
                if (playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                    jumpedTimes = 0;
                }

            }

            move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player
            if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
            {
                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
                jumpedTimes++;
                playerVelocity.y = jumpHeight;
            }
            if(move.normalized.magnitude <= 0)
            {
                anim.SetFloat("Speed", 0);
            }
            else if(!isSprinting && !isCrouching)
            {
                anim.SetFloat("Speed", 0.5f);
                
            }
            playerVelocity.y -= gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        IEnumerator PlaySteps()
        {
            stepsPlaying = true;
            aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

            if (!isSprinting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else if (isSprinting)
            {
                yield return new WaitForSeconds(0.3f);
            }
            stepsPlaying = false;
        }

        void Sprint()
        {
                if (Input.GetButtonDown("Sprint") && !isCrouching && stamina > 0)
                {
                    isSprinting = true;
                    playerSpeed *= sprint;
                    anim.SetFloat("Speed", 1);
                }
                else if (Input.GetButtonUp("Sprint"))
                {
                    isSprinting = false;
                    playerSpeed = speedOrig;
                }
                UpdatePlayerStamina();
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void crouch()
        {
            if (Input.GetButtonDown("Crouch"))
            {
                playerSpeed /= 2;
                controller.height = controller.height / 2;
                isCrouching = true;
                aud.PlayOneShot(audCrouch);
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                playerSpeed = speedOrig;
                controller.height = controller.height * 2;
                isCrouching = false;
                
            }
        }
        
        /**----------------------------------------------------------------
         * @brief
         */
        private IEnumerator shoot()
        {
         //   Debug.Log(gunList[selectedGun].curAmmo);
            if (gunList[selectedGun].curAmmo > 0)
            {
                muzzleFlash.Play();

                
                gunList[selectedGun].curAmmo--;   
                isShooting = true;
                aud.PlayOneShot(gunShot, shotVol);
                RaycastHit hit;
                updateAmmoUI();

                
                


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
            if (Input.GetButtonDown("Shoot") && gunList[selectedGun].curAmmo == 0 && !isReloading)
            {
                aud.PlayOneShot(emptyClipAud);
            }
        }

        


        /**----------------------------------------------------------------
         * @brief
         */
        private void showEnemyHP()
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

        /**----------------------------------------------------------------
         * @brief
         */
        private IEnumerator turnOffEnemyHP(RaycastHit _hit)
        {
            yield return new WaitForSeconds(1);
            if (_hit.collider != null)
            {
                _hit.collider.GetComponent<EnemyAI>().wholeHealthBar.SetActive(false);
            }

        }

        /**----------------------------------------------------------------
         * @brief
         */



    public void TakeDamage(int damage)
        {
            health -= damage;

            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

            if (health <= 0)
            {
                GameManager.instance.YouLose();
            }
            UpdatePlayerHp();
            StartCoroutine(playerFlashDamage());
        }

        /**----------------------------------------------------------------
         * @brief
         */

        public void UpdatePlayerStamina()
        {
            GameManager.instance.playerStaminaBar.fillAmount = (float)stamina / playerStaminaOrig;
            if(isSprinting)
            {
                stamina--;
            }
            else if(!isSprinting)
            {
                stamina++;
            }
            else if( stamina > playerStaminaOrig)
            {
                stamina = playerStaminaOrig;
            }
        }

        public void UpdatePlayerHp()
        {
            GameManager.instance.playerHpBar.fillAmount = (float) health / playerHpOrig;
            
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void SpawnPlayer()
        {
            controller.enabled = false;
            transform.position = GameManager.instance.playerSpawnPos.transform.position; 
            controller.enabled = true;
            health = playerHpOrig;
            UpdatePlayerHp();
            updateAmmoUI();
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private IEnumerator playerFlashDamage()
        {
            GameManager.instance.playerFlashUI.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.playerFlashUI.SetActive(false);
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void PickupGun(GunStats gunstat)
        {
            
            gunList.Add(gunstat);
            
            aud.PlayOneShot(pickupClip);
            shootDamage = gunstat.shootDamage;
            shootDistance = gunstat.shootDistance;
            shootRate = gunstat.shootRate;

            gunModel.GetComponent<MeshFilter>().mesh = gunstat.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().material = gunstat.model.GetComponent<MeshRenderer>().sharedMaterial;
            selectedGun = gunList.Count - 1;

            updateAmmoUI();
        }

        /**----------------------------------------------------------------
         * @brief
         */
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

        /**----------------------------------------------------------------
         * @brief
         */
        private void swapGunStats()
        {
            shootDamage = gunList[selectedGun].shootDamage;
            shootDistance = gunList[selectedGun].shootDistance;
            shootRate = gunList[selectedGun].shootRate;
            
            gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

            updateAmmoUI();
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void AmmoPickup(int amount, GameObject obj)
        {
            PickupAmmo ammoPickup = obj.GetComponent<PickupAmmo>();

            ammoStorage = ammoPickup.ammoAmount + ammoStorage;
            
            Destroy(obj);

            updateAmmoUI();
        }

        IEnumerator Reload()
        {
            if (Input.GetKeyDown(KeyCode.R) && ammoStorage != 0 && !isReloading)
            {
                if (gunList[selectedGun].curAmmo < gunList[selectedGun].maxAmmo) //Need to subtract from Ammo
                {
                    isReloading= true;
                    aud.PlayOneShot(audReload);
                    yield return new WaitForSeconds(1.5f);
                    if (gunList.Count > 0)
                    {
                        int ammoDiffer = gunList[selectedGun].maxAmmo - gunList[selectedGun].curAmmo;
                        gunList[selectedGun].curAmmo += ammoStorage;

                        if (gunList[selectedGun].curAmmo > gunList[selectedGun].maxAmmo)
                        {
                            gunList[selectedGun].curAmmo = gunList[selectedGun].maxAmmo;
                        }

                        ammoStorage -= ammoDiffer;
                    }
                    if (ammoStorage <= 0)
                    {
                        ammoStorage = 0;
                    }
                    updateAmmoUI();
                    isReloading = false;
                }
                

            }
        }


        

        public void healthPickup(int amount, GameObject obj)
        {
            if (health < playerHpOrig)
            {
                aud.PlayOneShot(healthClip, healthVol);
                health = playerHpOrig;
                UpdatePlayerHp();
                Destroy(obj);
            }
        }

        IEnumerator collectedText(RaycastHit hit)
        {
            GameManager.instance.itemCollectedText.text = hit.collider.gameObject.name + (" collected");
            GameManager.instance.itemCollectedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            GameManager.instance.itemCollectedText.gameObject.SetActive(false);
        }

        public void updateAmmoUI()
        {
            if(gunList.Count > 0)
            {
                GameManager.instance.ammoCurText.text = gunList[selectedGun].curAmmo.ToString("F0");
                GameManager.instance.ammoStorageText.text = ammoStorage.ToString("F0");
                //GameManager.instance.ammoMaxText.text = gunList[selectedGun].maxAmmo.ToString("F0");
            }
        }

        void zoomSights()
        {
            if (Input.GetButton("Zoom"))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomIn, Time.deltaTime * zoomInSpeed);
            }
            else
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomOrig, Time.deltaTime * zoomOutSpeed);
            }
        }
        void UiSwitch()
        {
            if (gunList[selectedGun].curAmmo == 30)
            {
                GameManager.instance.assaultRifle.gameObject.SetActive(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.tag == "PlayerCell")
            {
                if (other.GetComponent<CellDoor>().Moving == false)
                {
                    other.GetComponent<CellDoor>().Moving = true;
                }
            }
            if(other.tag == "CellDoor" && hasCellKey)
            {
                if(other.GetComponent<CellDoor>().Moving == false)
                {
                    other.GetComponent<CellDoor>().Moving = true;
                }
            }
        }
    }
}
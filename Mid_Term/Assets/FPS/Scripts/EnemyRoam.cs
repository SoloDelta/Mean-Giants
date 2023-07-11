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
using UnityEngine.AI;

namespace FPS
{
    public class EnemyRoam : MonoBehaviour, IDamage
    {
        [Header("-----Components-----")]
        [SerializeField] public NavMeshAgent agent; //the enemy's navmesh
        [SerializeField] private Animator anim; //the enemy's animator
        [SerializeField] private Transform headPosition;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private GameObject soundObject;

        [Header("----- UIComponents-----")]
        [SerializeField] private GameObject enemyUIParent; //the entirety of enemy UI. Healthbar and detection.
        [SerializeField] public GameObject wholeHealthBar; //the healthbar parent
        [SerializeField] private Transform HPBar; //the healthbar itself
        [SerializeField] private GameObject spottingUI; // question mark UI
        [SerializeField] private GameObject spottedUI; //exclamation point UI
        [SerializeField] private Transform qmarkTransform; //the red question mark that gets scaled with being spotted

        [Header("-----Audio-----")]
        [SerializeField] AudioSource audSource;
        [SerializeField] AudioClip hmSound;
        [SerializeField] float hmSoundVol;
        public AudioClip shootSound;
        [SerializeField] float shootSoundVol;
        public AudioClip deathSound;
        [SerializeField] float deathSoundVol;
        public AudioClip detectedSound;
        [SerializeField] float detectedSoundVol = 1f;

        [Header("-----Enemy Stats-----")]
        [SerializeField] public string currentState;
        [SerializeField] private int HP; //the health of the enemy
        [SerializeField] private float speed; //speed at which the enemy moves
        [SerializeField] private int playerFaceSpeed; //the speed at which the enemy rotates to the player when stopped
        [SerializeField] private int viewConeAngle; //the angle from which the enemy can see the player (calculated from middle)
        [SerializeField] private bool atStart;
        [SerializeField] private bool spotted = false;
        [SerializeField] private float spottingDistance;

        [Header("-----Roaming-----")]
        [SerializeField, Range(1, 10)] private float roamTimer; //how long the enemy waits before roaming
        [SerializeField, Range(10, 100)] private int roamDist; //how far away the enemy will roam

        [Header("-----Gun Stats-----")]
        [SerializeField] private float shootRate; //how fast the enemy shoots
        [SerializeField] private GameObject bullet; //the bullet the enemy shoots
        [SerializeField] private float burstRate; //how fast of a burst the enemy shoots
        [SerializeField] private bool isBurstShot; //if the enemy shoots regular or in burst
        [SerializeField] private bool isShotgun;
        [SerializeField] private float shotGunSpread = 2.5f;

        private Vector3 playerDirection; //the direction from the enemy to the player
        private bool playerInRange; //is the player within range of the enemy
        private float angleToPlayer; //the angle to the player
        private bool seesPlayer; //does the enemy see the player
        private bool destinationChosen; //bool used in roam to show if the enemy has found a spot to roam to
        private float stoppingDistanceOriginal; //the original stopping distance of the enemy
        private int enemyHPOriginal; //starting hp of the enemy
        private float percentSpotted; //float repressenting how much the player has been spotted                            
        private Coroutine searchCopy; //a variable to store the last coroutine of "losingPlayer"        
        private bool isShooting;
        public bool shouldStartSearching = false;
        Vector3 playerLastSeenAt;
        public Vector3 startingPos;
        // Start is called before the first frame update
        void Start()
        {
            anim.SetBool("Aiming", true);
            audSource = GetComponent<AudioSource>();
            agent.speed = speed;
            enemyHPOriginal = HP;
            //startingPos = transform.position;
            stoppingDistanceOriginal = agent.stoppingDistance;
            //GameManager.instance.UpdateObjective(1);
            agent.avoidancePriority = GameManager.instance.enemiesRemaining;
        }

        // Update is called once per frame
        void Update()
        {
            if(agent.isActiveAndEnabled)
            {
                OpenDoors();
                anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
                seesPlayer = canSeePlayer();
                rotateUI();
                if (!spotted)
                {
                    Spotting();
                }
                if (seesPlayer && spotted) //will fight the player if spotted and currently seen
                {
                    currentState = "Combat";
                    Combat();

                }
                else if(currentState == "Going to last seen position" && Vector3.Distance(transform.position, agent.destination) < 1)
                {
                    currentState = "lost player";
                    spotted = false;
                }
                else if (spotted && !seesPlayer) //goes the players last known location if the player has been spotted but isnt seen
                {
                    currentState = "Going to last seen position";
                    agent.stoppingDistance = 0;
                }
                else if (percentSpotted > 0.5) //stops to look at the player if spotting is over 50%
                {
                    currentState = "SpottingPlayer";
                    agent.isStopped = true;
                }
                else
                {
                    currentState = "Roam";
                    StartCoroutine(Roam());
                }
            }
        
        }

        void Combat()
        {
            agent.stoppingDistance = stoppingDistanceOriginal;
            agent.SetDestination(GameManager.instance.player.transform.position);
            playerLastSeenAt = GameManager.instance.player.transform.position;
 
           
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
            }
            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
        bool canSeePlayer()
        {
            if (playerInRange)
            {
                playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - headPosition.position;
                angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
                Debug.DrawRay(headPosition.position, playerDirection);
                RaycastHit hit;
                if (Physics.Raycast(headPosition.position, playerDirection, out hit))
                {
                    if (hit.collider.CompareTag("Player") && angleToPlayer <= viewConeAngle)
                    {
                        if (!spotted)
                        {
                            spottingUI.SetActive(true);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        private void Spotting() //starts detecting the player over time
        {

            if (seesPlayer && percentSpotted < 1) //spots the player at a rate dependent on whether the player is crouching and how far away the player is.
            {
                float spottingCrouchScale;
                if (GameManager.instance.playerScript.isCrouching) { spottingCrouchScale = 0.5f; }
                else { spottingCrouchScale = 1f; }
                float playerDistanceScale = (spottingDistance - Vector3.Distance(transform.position, GameManager.instance.player.transform.position)) / spottingDistance;
                percentSpotted += spottingCrouchScale * playerDistanceScale * Time.deltaTime;
                qmarkTransform.localScale = new Vector3((0.35f * percentSpotted), (0.35f * percentSpotted), (0.35f * percentSpotted));
                qmarkTransform.localPosition = new Vector3(qmarkTransform.localPosition.x, 2.5f * percentSpotted, qmarkTransform.localPosition.z);
            }
            else if (!seesPlayer && percentSpotted > 0)
            {
                percentSpotted -= 0.25f * Time.deltaTime;
                qmarkTransform.localScale = new Vector3((0.35f * percentSpotted), (0.35f * percentSpotted), (0.35f * percentSpotted));
                qmarkTransform.localPosition = new Vector3(qmarkTransform.localPosition.x, 2.5f * percentSpotted, qmarkTransform.localPosition.z);
            }
            if (percentSpotted >= 1)
            {
                agent.isStopped = false;
                spotted = true;
                audSource.PlayOneShot(detectedSound, detectedSoundVol);
                spottingUI.SetActive(false);
                StartCoroutine(spottedUIon());

            }
            if (percentSpotted <= 0)
            {
                spottingUI.SetActive(false);
            }
        }
        IEnumerator spottedUIon() //turns the UI on if the player gets spotted. turns it off after 3seconds
        {
            if (spottingUI.activeInHierarchy)
            {
                spottingUI.SetActive(false);
            }
            spottedUI.SetActive(true);
            Destroy(Instantiate(soundObject, this.transform.position, this.transform.rotation), 0.5f);

            yield return new WaitForSeconds(3.0f);
            spottedUI.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInRange = true;
            }
            if(other.gameObject.CompareTag("Sound"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                spotted = true;
                Debug.Log("Heard Noise");
            }
            


        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInRange = false;
            }

        }
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "CellDoor" || other.tag == "PlayerCell")
            {
                if (Vector3.Distance(transform.position, other.transform.position) < 4)
                {
                    if (other.GetComponent<CellDoor>().Moving == false)
                    {
                        other.GetComponent<CellDoor>().Moving = true;
                    }
                }
            }
        }
        public void TakeDamage(int dmg) //logic for taking damage. 
        {
            if (agent.isActiveAndEnabled)
            {
                HP -= dmg;
                if (HP < 0) { HP = 0; }
                audSource.PlayOneShot(hmSound, hmSoundVol);
                StartCoroutine(HitMarker());
                updateEnemyUI();
                if (HP <= 0) //if the enemy is dead, turns of lasersight, stops all active coroutines, stops animations, and turns off collision.
                {
                    GameManager.instance.hitMarkKill.gameObject.SetActive(false);
                    if (headPosition.parent.gameObject.GetComponent<Collider>())
                    {
                        headPosition.parent.gameObject.GetComponent<Collider>().enabled = false;
                        Debug.Log("Foundhead");
                    }
                    StopAllCoroutines();
                    spottedUI.SetActive(false);
                    spottingUI.SetActive(false);
                    wholeHealthBar.SetActive(false);

                    anim.SetBool("Aiming", false);
                    anim.SetBool("Death", true);
                    audSource.PlayOneShot(deathSound, deathSoundVol);
                    //GameManager.instance.UpdateObjective(-1);

                    StartCoroutine(HitMarker());

                    agent.enabled = false;

                    //GetComponent<CapsuleCollider>().enabled = false;
                    this.gameObject.layer = 13;
                }
                else //starts attacking the player and instantly spots them
                {
                    agent.SetDestination(GameManager.instance.player.transform.position);
                    if (!spotted)
                    {
                        StartCoroutine(spottedUIon());
                        audSource.PlayOneShot(detectedSound, detectedSoundVol);
                    }
                    spotted = true;
                }
            }


        }
        void rotateUI() //rotates the enemy's UI towards the player
        {
            if (wholeHealthBar.activeInHierarchy || spottedUI.activeInHierarchy || spottingUI.activeInHierarchy) //if ui is active, set its x rotation to that of the camera and set the y rotation to that of the player
            {
                Vector3 eulerAngleRots = new Vector3(Camera.main.transform.rotation.eulerAngles.x, GameManager.instance.player.transform.rotation.eulerAngles.y, 0.0f);
                Quaternion rotation = Quaternion.Euler(eulerAngleRots);
                enemyUIParent.transform.rotation = rotation;
            }
        }
        IEnumerator Roam() //roams fast
        {
            agent.stoppingDistance = 0;
            agent.isStopped = false;
            if (!destinationChosen && agent.remainingDistance < 1.5f)
            {
                destinationChosen = true;
                yield return new WaitForSeconds(roamTimer);
                destinationChosen = false;
                Vector3 randomPos = Random.insideUnitSphere * roamDist;
                randomPos += startingPos;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
            }
            else if (!destinationChosen && agent.velocity.magnitude < 1 && Vector3.Distance(transform.position, agent.destination) <= 2)
            {
                Debug.Log("Got Stuck, Finding new roam");
                Vector3 randomPos = Random.insideUnitSphere * roamDist;
                randomPos += startingPos;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
            }
        }
        private void updateEnemyUI() // Updates enemyHP bar
        {
            HPBar.transform.localScale = new Vector3((float)HP / enemyHPOriginal, HPBar.localScale.y, HPBar.localScale.y);
        }

        public void createBullet() //spawns the bullet at a position. needs work
        {
            playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
            Instantiate(bullet, shootPosition.position, Quaternion.LookRotation(playerDirection));
            audSource.PlayOneShot(shootSound, shootSoundVol);
            Debug.DrawRay(shootPosition.position, playerDirection);
        }
        void ShotgunBlast()
        {
            playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
            Quaternion lookrotation = Quaternion.LookRotation(playerDirection);
            //lookrotation.eulery
            Vector3 randAngles = new Vector3(Random.Range(-shotGunSpread, shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
            randAngles = new Vector3(Random.Range(-shotGunSpread, shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
            randAngles = new Vector3(Random.Range(-shotGunSpread, shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
            randAngles = new Vector3(Random.Range(-shotGunSpread, shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
            randAngles = new Vector3(Random.Range(-shotGunSpread, shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));



            audSource.PlayOneShot(shootSound, shootSoundVol);
            Debug.DrawRay(shootPosition.position, playerDirection);

        }
        IEnumerator HitMarker()
        {
            if (HP > 1)
            {
                GameManager.instance.hitMark.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                GameManager.instance.hitMark.gameObject.SetActive(false);
            }
            else if (HP <= 0)
            {
                GameManager.instance.hitMarkKill.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                GameManager.instance.hitMarkKill.gameObject.SetActive(false);
            }

        }
        private IEnumerator shootBurst() //burst that shoots 3 bullets 
        {
            createBullet();
            yield return new WaitForSeconds(burstRate);
            createBullet();
            yield return new WaitForSeconds(burstRate);
            createBullet();
        }
        private IEnumerator shoot() //coroutine to control shooting logic. Shoots a bullet, shoots a burst if needed
        {
            isShooting = true;

            anim.SetTrigger("Shoot");
            if (isBurstShot)
            {
                StartCoroutine(shootBurst());

            }
            else if (isShotgun)
            {
                ShotgunBlast();
            }
            else
            {
                createBullet();
            }

            yield return new WaitForSeconds(shootRate);

            isShooting = false;
        }
        void OpenDoors()
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            //int mask = 1 << LayerMask.NameToLayer(excludeName) | layerInteract.value;

            if (Physics.Raycast(transform.position, fwd, out hit, 5))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<DoorController>();
                    Debug.Log("Open Door");
                    if (raycastedObj != null) { raycastedObj.PlayAnimationEnemy(); }

                }
            }
        }
    }
}

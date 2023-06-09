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
using static Unity.VisualScripting.Member;

namespace FPS
{
    public class EnemyAIRefactor : MonoBehaviour, IDamage
    {
        #region Editor Vars
        [Header("-----Components-----")]
        [SerializeField] public NavMeshAgent agent; //the enemy's navmesh
        [SerializeField] private Animator anim; //the enemy's animator
        [SerializeField] private Transform headPosition;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private GameObject baseManager;
        [SerializeField] private BaseManager baseManagerScript;
        [SerializeField] private GameObject keyCard;
        [SerializeField] GameObject ammoPrefab;

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
        public AudioClip alarmNoise;
        [SerializeField] float alarmNoiseVol = 0.5f;
        public AudioClip detectedSound;
        [SerializeField] float detectedSoundVol = 1f;

        [Header("-----Enemy Stats-----")]
        [SerializeField] public string currentState; //This variable is just a visual representation of the current state of the enemy.
        [SerializeField] public int HP; //the health of the enemy
        [SerializeField] private float speed; //speed at which the enemy moves
        [SerializeField] private int playerFaceSpeed; //the speed at which the enemy rotates to the player when stopped
        [SerializeField] private int viewConeAngle; //the angle from which the enemy can see the player (calculated from middle)
        [SerializeField] private bool atStart; //a visual bool just to show if the AI is where it started.
        [SerializeField] private bool spotted = false;
        [SerializeField] public bool highAlert = false;
        [SerializeField] private float spottingDistance;
        [SerializeField] float currencyDrop;

        [Header("-----Pathfinding-----")]
        [SerializeField] private List<Vector3> patrolSpots = new List<Vector3>(); //the locations the enemy will follow when patroling
        [SerializeField] private List<int> patrolRotations = new List<int>(); //the rotations the enemy will face too at once reaching a patrol spot
        [SerializeField] float patrolStopTime;
        [SerializeField] float highAlertPatrolStopTime;
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
        #endregion

        #region Script Vars
        private int numOfPatrolSpots; //the number of spots the enemy patrols to
        private Vector3 playerDirection; //the direction from the enemy to the player
        private bool playerInRange; //is the player within range of the enemy
        private float angleToPlayer; //the angle to the player
        private bool seesPlayer; //does the enemy see the player
        private bool destinationChosen; //bool used in roam to show if the enemy has found a spot to roam to
        private Vector3 startingPos; //where the enemy was before roam started
        private float stoppingDistanceOriginal; //the original stopping distance of the enemy
        //private bool isPatrolling; //if the enemy patrols or roams
        private int currentPointIndex; //the index of the last patrol spot the enemy was at
        private int enemyHPOriginal; //starting hp of the enemy
        private float percentSpotted; //float repressenting how much the player has been spotted                            
        private Coroutine searchCopy; //a variable to store the last coroutine of "losingPlayer"        
        private float timeCount = 0.0f; //time variable for slerping after a patrol
        private bool isShooting;
        public bool shouldStartSearching = false;
        int maxHealth;
        ///newVars <summary>
        /// newVars
        /// </summary>
        bool tryAlert = true;
        public bool pullAlarm = false;
        bool patrolTurnAround = false;
        bool searching = false;
        Vector3 playerLastSeenAt;
        private DoorController raycastedObj;
        // Start is called before the first frame update

        #endregion

        #region Start
        void Start()
        {
            baseManager = transform.parent.transform.parent.gameObject;
            baseManagerScript = baseManager.GetComponent<BaseManager>();
            anim.SetBool("Aiming", true);
            audSource = GetComponent<AudioSource>();
            startingPos = transform.position;
            agent.speed = speed;
            enemyHPOriginal = HP;
            maxHealth = enemyHPOriginal;
            numOfPatrolSpots = patrolSpots.Count;
            stoppingDistanceOriginal = agent.stoppingDistance;

            if (patrolSpots.Count > 0) //if there are patrol spots set, ensures there are enough rotations to go w it. If there are no spots, add a spot and rotation
            {
                if (patrolSpots.Count - patrolRotations.Count != 0)
                {
                    while (patrolRotations.Count < patrolSpots.Count)
                    {
                        patrolRotations.Add(0);
                    }
                }
            }
            else
            {
                patrolSpots.Add(transform.position);
                patrolRotations.Add(0);
            }
            agent.destination = patrolSpots[0];
            //GameManager.instance.UpdateObjective(1);
            agent.avoidancePriority = GameManager.instance.enemiesRemaining;
        }
        #endregion

        #region Update
        void Update()
        {      
            if (agent.isActiveAndEnabled)
            {
                OpenDoors();
                anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
                seesPlayer = canSeePlayer();
                rotateUI();
                if (!spotted)
                {
                    Spotting();
                }
                StateMachine();
            }
        }
        #endregion

        #region FSM
        void StateMachine()
        {
            if (pullAlarm)
            {
                currentState = "Pulling Alarm";
                PullAlarm();
            }
            else if (seesPlayer && spotted) //will fight the player if spotted and currently seen
            {
                currentState = "Combat";
                Combat();

            }
            else if (searching) //only called while searching
            {
                spotted = true;
                currentState = "Search/Roaming";
                StartCoroutine(Roam());
            }
            else if ((!seesPlayer && Vector3.Distance(transform.position, playerLastSeenAt) <= 1.5) || shouldStartSearching) //starts searching if the enemy cant see the player and the enemy is at the players last known location
            {
                currentState = "Starting Search";
                agent.stoppingDistance = 0;

                if (searchCopy == null) { searchCopy = StartCoroutine(Search()); }
            }
            else if (spotted && !seesPlayer) //goes the players last known location if the player has been spotted but isnt seen
            {
                currentState = "Going to last seen position";
                if (agent.velocity.magnitude < 1 && Vector3.Distance(transform.position, agent.destination) <= 2)
                {
                    shouldStartSearching = true;
                }
                agent.stoppingDistance = 0;
            }
            else if (percentSpotted > 0.5) //stops to look at the player if spotting is over 50%
            {
                currentState = "SpottingPlayer";
                agent.isStopped = true;
            }
            else if (canSeeBody() && !baseManagerScript.highAlert) //something to make enemy sus from player
            {
                agent.stoppingDistance = 4;
                currentState = "Noticing dead body";
                //Path to a game object: Notice dead body
            } 
            else //if nothing else, do the patrol route
            {
                currentState = "Patrolling";
                Patrol();
            }

        }
        #endregion

        #region States
        public void PullAlarm() //finds the closest alarm, paths to it, and pulls the alarm to summon reinforcements
        {
            StopCoroutine(PatrolTurnAround());
            agent.stoppingDistance = 0;
            agent.isStopped = false;
            GameObject closestAlarm = baseManagerScript.alarms[0];
            for (int i = 0; i < baseManagerScript.alarms.Count; i++)
            {
                if (Vector3.Distance(baseManagerScript.alarms[i].transform.position, transform.position) < Vector3.Distance(closestAlarm.transform.position, transform.position))
                {
                    closestAlarm = baseManagerScript.alarms[i];
                }
            }
            agent.SetDestination(closestAlarm.transform.GetChild(1).transform.position);
            agent.speed = speed * 1.5f;
            if (Vector3.Distance(transform.position, closestAlarm.transform.GetChild(1).transform.position) < 1.5)
            {
                Vector3 alarmDirection = closestAlarm.transform.GetChild(1).transform.position - headPosition.position;
                Quaternion rot = Quaternion.LookRotation(new Vector3(alarmDirection.x, 0, alarmDirection.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
                StartCoroutine(PullingAlarm());
            }

        }
        void Combat()
        {
            agent.isStopped = false;
            searching = false;
            shouldStartSearching = false;
            spottingUI.SetActive(false);
            if (tryAlert)
            {
                baseManagerScript.PullAlarm(this.gameObject);
                baseManagerScript.AlertNearbyEnemies(this.gameObject);
                tryAlert = false;
                StartCoroutine(ReCallAlarmPull());
            }


            if (searchCopy != null) //if the enemy has started losing the player, stop losing the player.
            {
                StopCoroutine(searchCopy);
                searchCopy = null;
                spottingUI.SetActive(false);
                StartCoroutine(spottedUIon());
                audSource.PlayOneShot(detectedSound, detectedSoundVol);
                searching = false;
            }
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
        private IEnumerator Search()
        {
            if (!spotted)
            {
                StartCoroutine(ChangeStealthVals(true));
            }
            spotted = true;

            searching = true;
            percentSpotted = 1.2f;
            spottingUI.SetActive(true);
            yield return new WaitForSeconds(15);
            spottingUI.SetActive(false);
            searching = false;
            shouldStartSearching = false;
            spotted = false;
            StartCoroutine(ChangeStealthVals(false));
            percentSpotted = 0;
            StopCoroutine(Roam());
     
        }
        bool canSeeBody()
        {
            foreach (GameObject deadEnemy in baseManagerScript.enemies)
            {
                Vector3 deadDirection = deadEnemy.transform.position - headPosition.transform.position;
                
                if ((Vector3.Distance(deadEnemy.transform.position, this.gameObject.transform.position) < 50) && deadEnemy.layer == 13)
                {
                    
                    float angleToDead = Vector3.Angle(new Vector3(deadDirection.x, deadDirection.y, deadDirection.z), transform.forward);
                    
                    RaycastHit hit;
                    if (Physics.Raycast(headPosition.position, deadDirection, out hit))
                    {
                        
                        if (hit.collider.gameObject.layer == 13 && angleToDead <= viewConeAngle)
                        {
                         
                            agent.isStopped = false;
                            agent.stoppingDistance = 4;
                            agent.SetDestination(deadEnemy.transform.position);
                            if (Vector3.Distance(deadEnemy.transform.position, agent.transform.position) < 5)
                            {
                                if (tryAlert)
                                {
                                    baseManagerScript.PullAlarm(this.gameObject);
                                    tryAlert = false;
                                    StartCoroutine(ReCallAlarmPull());
                                }
                                shouldStartSearching = true;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        void Patrol() //follows a set patrol route, turning the set amount after reaching the spot
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(patrolSpots[currentPointIndex]);
            if (!patrolTurnAround) { agent.isStopped = false; }
            if (Vector3.Distance(agent.transform.position, patrolSpots[currentPointIndex]) < 0.1)
            {
                if (!patrolTurnAround)
                {
                    StartCoroutine(PatrolTurnAround());
                }
                Quaternion rotationAmount = Quaternion.Euler(0, patrolRotations[currentPointIndex], 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationAmount, timeCount);
                timeCount += Time.deltaTime / 10;
            }
        }
        #endregion
        
        #region Detection
        bool canSeePlayer()
        {
            if (playerInRange)
            {
                playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - headPosition.position;
                angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
 
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
                if (!spotted) { audSource.PlayOneShot(detectedSound, detectedSoundVol); }
                spotted = true;
                shouldStartSearching = false;
                spottingUI.SetActive(false);
                StartCoroutine(spottedUIon());
                StartCoroutine(ChangeStealthVals(true));
            }
            if (percentSpotted <= 0)
            {
                spottingUI.SetActive(false);
            }
            if(percentSpotted >= 0.5f && !seesPlayer)
            {
                agent.isStopped = false;
            }
        }
        IEnumerator spottedUIon() //turns the UI on if the player gets spotted. turns it off after 3seconds
        {
            if (spottingUI.activeInHierarchy)
            {
                spottingUI.SetActive(false);
            }
            spottedUI.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            spottedUI.SetActive(false);
        }


        IEnumerator ChangeStealthVals(bool _increase)
        {

            yield return new WaitForSeconds(1.5f);
            if(_increase && maxHealth != 2*enemyHPOriginal)
            {
                maxHealth *= 2;
                HP *= 2;
                viewConeAngle += 20;
            }
            else if(maxHealth != enemyHPOriginal && !_increase)
            {
                maxHealth /= 2;
                HP /= 2;
                viewConeAngle -= 20;
            }

        }
        #endregion

        #region Combat Supplements

        public void TakeDamage(int dmg) //logic for taking damage. 
        {
            if(agent.isActiveAndEnabled)
            {
                HP -= dmg;
                if (HP < 0) { HP = 0; }
                audSource.PlayOneShot(hmSound, hmSoundVol);
                StartCoroutine(HitMarker());
                updateEnemyUI();
                if (HP <= 0) //if the enemy is dead, turns of lasersight, stops all active coroutines, stops animations, and turns off collision.
                {
                    DropItem();
                    GameManager.instance.hitMarkKill.gameObject.SetActive(false);
                    if(headPosition.parent.gameObject.GetComponent<Collider>())
                    {
                        headPosition.parent.gameObject.GetComponent<Collider>().enabled = false;
                    
                    }
                    StopAllCoroutines();
                    spottedUI.SetActive(false);
                    spottingUI.SetActive(false);
                    wholeHealthBar.SetActive(false);

                    anim.SetBool("Aiming", false);
                    anim.SetBool("Use", false);
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
                        spottingUI.SetActive(false);
                        StartCoroutine(spottedUIon());
                        audSource.PlayOneShot(detectedSound, detectedSoundVol);
                    }
                    spotted = true;
                }
            }
            
            
        }

        void DropItem()
        {
            if(keyCard != null)
            {
                Instantiate(keyCard, transform.position, transform.rotation);
            }
            else
            {
                if(Random.Range(0,2) == 0)
                {
                    Instantiate(ammoPrefab,transform.position, transform.rotation);
                }
                else
                {
                    GameManager.instance.playerScript.playerCurrency += currencyDrop;
                }
            }
        }
        private void updateEnemyUI() // Updates enemyHP bar
        {
            HPBar.transform.localScale = new Vector3((float)HP / maxHealth, HPBar.localScale.y, HPBar.localScale.y);
        }

        public void createBullet() //spawns the bullet at a position. needs work
        {
            playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
            Instantiate(bullet, shootPosition.position, Quaternion.LookRotation(playerDirection));
            audSource.PlayOneShot(shootSound, shootSoundVol);
       
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
            randAngles = new Vector3(Random.Range(-shotGunSpread , shotGunSpread), Random.Range(-shotGunSpread, shotGunSpread), 0);
            Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));



            audSource.PlayOneShot(shootSound, shootSoundVol);
   

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
        #endregion

        #region Pathing Supplements
        public void SearchBase()
        {
            StartCoroutine(Search());
        }

        IEnumerator Roam() //roams fast
        {
            agent.stoppingDistance = 0;

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
           
                Vector3 randomPos = Random.insideUnitSphere * roamDist;
                randomPos += startingPos;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
            }
        }

        IEnumerator PatrolTurnAround()
        {
            patrolTurnAround = true;
            agent.isStopped = true;
            if (highAlert)
            {
                yield return new WaitForSeconds(highAlertPatrolStopTime);
            }
            else
            {
                yield return new WaitForSeconds(patrolStopTime);
            }
            agent.isStopped = false;
            patrolTurnAround = false;
            timeCount = 0;
            currentPointIndex++;
            if (currentPointIndex > numOfPatrolSpots - 1)
            {
                currentPointIndex = 0;
            }
        }

        IEnumerator PullingAlarm()
        {
            anim.SetBool("Aiming", false);
            anim.SetBool("Use", true);
            yield return new WaitForSeconds(4);
            pullAlarm = false;
            anim.SetBool("Use", false);
            anim.SetBool("Aiming", true);
            //playerLastSeenAt = transform.position;
            baseManagerScript.pullAlarm = false;
            audSource.PlayOneShot(alarmNoise, alarmNoiseVol);
            // baseManagerScript.isPullingAlarm = false;
            agent.speed = speed;
            baseManagerScript.highAlert = true;
            highAlert = true;
        }
        #endregion

        #region Misc

        void OpenDoors()
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            //int mask = 1 << LayerMask.NameToLayer(excludeName) | layerInteract.value;

            if (Physics.Raycast(transform.position, fwd, out hit, 5))
            {
                if (hit.collider.CompareTag("Door")){raycastedObj = hit.collider.gameObject.GetComponent<DoorController>();
  
                    if(raycastedObj != null) { raycastedObj.PlayAnimationEnemy(); }
                      
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInRange = true;
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
            if(other.tag == "CellDoor"  || other.tag == "PlayerCell")
            {
                if(Vector3.Distance(transform.position, other.transform.position) < 4)
                {
                    if (other.GetComponent<CellDoor>().Moving == false)
                    {
                        other.GetComponent<CellDoor>().Moving = true;
                    }
                }
            }
           
        }
        private void OnCollisionEnter(Collision collision)
        {
            
            if (collision.collider.gameObject.CompareTag("Enemy"))
            {
                
                if (collision.collider.gameObject.GetComponent<EnemyAIRefactor>().agent.isActiveAndEnabled)
                {

                    if (Vector3.Distance(agent.transform.position, agent.destination) > 
                        Vector3.Distance(collision.collider.gameObject.GetComponent<EnemyAIRefactor>().agent.transform.position, 
                        collision.collider.gameObject.GetComponent<EnemyAIRefactor>().agent.destination))
                    {
                   
                    }
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

        IEnumerator ReCallAlarmPull()
        {

            yield return new WaitForSeconds(15);
            if (!baseManagerScript.highAlert)
            {
     
                tryAlert = true;
            }
        }
        #endregion

    }
}

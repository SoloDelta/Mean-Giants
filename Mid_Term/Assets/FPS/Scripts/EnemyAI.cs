/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - John Price
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
    /**----------------------------------------------------------------
     * @brief
     */
    public class EnemyAI : MonoBehaviour, IDamage
    {
        [Header("-----Components-----")]
        [SerializeField] private Renderer model; //the model of the enemy
        [SerializeField] private NavMeshAgent agent; //the enemy's navmesh
        [SerializeField] private Animator anim; //the enemy's animator
        [SerializeField] private Transform headPosition; //the position of the players head. used for more direct raycasts
        [SerializeField] private Transform shootPosition; //the position of where the bullet comes out
        [SerializeField] private GameObject lineRenderer; //line renderer used for laser sight

        [Header("----- UIComponents-----")]
        [SerializeField] private GameObject enemyUIParent; //the entirety of enemy UI. Healthbar and detection.
        [SerializeField] public GameObject wholeHealthBar; //the healthbar parent
        [SerializeField] private Transform HPBar; //the healthbar itself
        [SerializeField] private GameObject spottingUI; // question mark UI
        [SerializeField] private GameObject spottedUI; //exclamation point UI
        [SerializeField] private Transform qmarkTransform; //the red question mark that gets scaled with being spotted

        [Header("-----Audio-----")]
        public AudioSource hmSource;
        public AudioClip hmClip;
        public AudioClip shootSound;
        [SerializeField]float shootSoundVol;
        public AudioClip deathSound;
        [SerializeField] float deathSoundVol;

        [Header("-----Enemy Stats-----")]
        [SerializeField] private int HP; //the health of the enemy
        [SerializeField] private float speed; //speed at which the enemy moves
        [SerializeField] private int playerFaceSpeed; //the speed at which the enemy rotates to the player when stopped
        [SerializeField] private int viewConeAngle; //the angle from which the enemy can see the player (calculated from middle)
        [SerializeField] private bool atStart; //a visual bool just to show if the AI is where it started.
        [SerializeField] private bool sawPlayerTemp = false;
        [SerializeField] private bool spotted = false;
        [SerializeField] private float spottingDistance;
        [Header("-----Roaming-----")]
        [SerializeField, Range(1, 10)] private float roamTimer; //how long the enemy waits before roaming
        [SerializeField, Range(10, 100)] private int roamDist; //how far away the enemy will roam

        [Header("-----Pathfinding-----")]
        [SerializeField] private List<Vector3> patrolSpots = new List<Vector3>(); //the locations the enemy will follow when patroling
        [SerializeField] private List<int> patrolRotations = new List<int>(); //the rotations the enemy will face too at once reaching a patrol spot

        [Header("-----Enemy Stats-----")]
        [SerializeField] private float shootRate; //how fast the enemy shoots
        [SerializeField] private GameObject bullet; //the bullet the enemy shoots
        [SerializeField] private float burstRate; //how fast of a burst the enemy shoots
        [SerializeField] private bool isBurstShot; //if the enemy shoots regular or in burst
        private int numOfPatrolSpots; //the number of spots the enemy patrols to
        private Vector3 playerDirection; //the direction from the enemy to the player
        private bool playerInRange; //is the player within range of the enemy
        private float angleToPlayer; //the angle to the player
        private bool isShooting; //is the enemy actively shooting
        private bool seesPlayer; //does the enemy see the player
        private bool destinationChosen; //bool used in roam to show if the enemy has found a spot to roam to
        private Vector3 startingPos; //where the enemy was before roam started
        private float stoppingDistanceOriginal; //the original stopping distance of the enemy
        private bool isPatrolling; //if the enemy patrols or roams
        private int currentPointIndex; //the index of the last patrol spot the enemy was at
        private int enemyHPOriginal; //starting hp of the enemy
        private float percentSpotted; //float repressenting how much the player has been spotted
        //has the enemy spotted the player
        private Coroutine losingPlayerCopy; //a variable to store the last coroutine of "losingPlayer"
         //did a raycast connect but not long enough for the player to be spotted
        private float timeCount = 0.0f; //time variable for slerping after a patrol
        private void Start()
        {
            //initializing variables
            anim.SetBool("Aiming", true);
            hmSource = GetComponent<AudioSource>();
            hmSource.clip = hmClip;
            startingPos = transform.position;
            agent.speed = speed;
            enemyHPOriginal = HP;
            numOfPatrolSpots = patrolSpots.Count;
            stoppingDistanceOriginal = agent.stoppingDistance;

            if (patrolSpots.Count > 0)
            {
                isPatrolling = true;
                if(patrolSpots.Count - patrolRotations.Count != 0)
                {
                    
                    while(patrolRotations.Count < patrolSpots.Count)
                    {
                        patrolRotations.Add(0);
                    }
                }
            }
            else
            {
                isPatrolling = false;
            }
          
        }

        private void Update()
        {
            
            
            
            if (agent.isActiveAndEnabled)
            {
                playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
                Debug.DrawRay(shootPosition.position, playerDirection);
                anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
                rotateUI();
                if(true) //removed playerinrange. Needs fix
                {
                    seesPlayer = canSeePlayer();
                    enemyNav();
                    
                }
                
            }
            
        }


        private void enemyNav() //Mega Function to determine how the enemy will navigate. 
        {
            if (!spotted) //if the player has not been spotted update percent spotted
            {
                spotting(Time.deltaTime);
            }
            if (seesPlayer) //if the enemy sees the player
            {
                
                if (!spotted) //if the player has not been spotted
                {
                    //agent.isStopped = true;
                    sawPlayerTemp = true;
                }
                else //if the player has been spotted
                {
                    if (losingPlayerCopy != null) //if the enemy has started losing the player, stop losing the player.
                    {
                        StopCoroutine(losingPlayerCopy);
                        losingPlayerCopy = null;
                        spottingUI.SetActive(false);
                        StartCoroutine(spottedUIon());
                    }

                    agent.stoppingDistance = stoppingDistanceOriginal;
                    agent.SetDestination(GameManager.instance.player.transform.position);
                    if (agent.remainingDistance <= agent.stoppingDistance) //ensures the enemy still tracks the player with rotation after reaching its stopping point
                    {
                        facePlayer();
                    }
                    if (!isShooting) //shoot
                    {
                        StartCoroutine(shoot());
                    }
                }
            }
            else //if the enemy does not see the player
            {
                
                if (spotted) //if the player has been spotted, go to the players last seen location and look around. 
                {
                    agent.stoppingDistance = 0;

                    if (agent.destination.x == transform.position.x && agent.destination.z == transform.position.z) 
                    {

                        if (losingPlayerCopy == null)
                        {
                            losingPlayerCopy = StartCoroutine(losingPlayer());

                        }
                        //Start look around routine
                        //
                        StartCoroutine(roam());
                    }
                }
                else //if the player hasnt been spotted, go back to original action, patroling or roaming
                {
                    if(sawPlayerTemp)
                    {
                        //Quaternion rotationAmount = transform.rotation * Quaternion.Euler(0, patrolRotations[currentPointIndex], 0);
                        //Debug.Log(rotationAmount);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, rotationAmount, 10.0f);

                        //timeCount = timeCount + Time.deltaTime;
                        
                        //MAKE NEW COROUTINE

                    }
                    if(isPatrolling)
                    {
                        patrolCirculation();
                    }
                    else
                    {
                        StartCoroutine(roam());
                    }
                    
                }
            }
        }

        private bool canSeePlayer() //raycasts to the player to see if the player can be seen. Need to update player in range
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
            
            //agent.stoppingDistance = 0;
            return false;
        }


        private void spotting(float _deltaTime) //starts detecting the player over time
        {
            if (seesPlayer && percentSpotted < 1) //spots the player at a rate dependent on whetehr the player is crouching and how far away the player is.
            {
                //anim.SetBool("Aiming", true);
                float spottingCrouchScale;
                agent.isStopped = true;
                if (GameManager.instance.playerScript.isCrouching) { spottingCrouchScale = 0.5f; }
                else { spottingCrouchScale = 1f; }
                float playerDistanceScale = (spottingDistance - Vector3.Distance(transform.position, GameManager.instance.player.transform.position)) / spottingDistance;
                percentSpotted += spottingCrouchScale * playerDistanceScale * _deltaTime;
                qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
            }
            else if (!seesPlayer && percentSpotted > 0)
            {
                agent.isStopped = false;
                percentSpotted -= 0.25f * _deltaTime;
                qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
            }
            if (percentSpotted >= 1)
            {
                agent.isStopped = false;
                spotted = true;
                spottingUI.SetActive(false);
                StartCoroutine(spottedUIon());
                Debug.Log(spotted);
            }
            if (percentSpotted <= 0)
            {
                agent.isStopped = false;
                spottingUI.SetActive(false);
            }
        }
        private void patrolCirculation() //controls the logic for pathing. if the enemy has a patrol route and doesnt see the player, he patrols. if he doesnt have a patrol he roams.
        {
            if (isPatrolling && !seesPlayer)
            {
                agent.stoppingDistance = 0;
                agent.SetDestination(patrolSpots[currentPointIndex]);

                if (new Vector3(transform.position.x, patrolSpots[currentPointIndex].y, transform.position.z) == patrolSpots[currentPointIndex]) //TRY AND CHECK IF PLAYER IS AT LEAST CLOSE TO POINT 
                {
                    
                    if(!agent.isStopped)
                    {
                        StartCoroutine(lookAround());
                    }
                    Debug.Log(agent.isStopped);
                    
                    
                        
                    Quaternion rotationAmount = Quaternion.Euler(0, patrolRotations[currentPointIndex], 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotationAmount, timeCount);
                    timeCount += Time.deltaTime / 10;
                    Debug.Log(timeCount);

                }
                else
                {
                   // agent.isStopped = false;
                }
            }
            else { StartCoroutine(roam()); }
        }
        

        private void rotateUI() //rotates the enemy's UI towards the player
        {
            if (wholeHealthBar.activeInHierarchy || spottedUI.activeInHierarchy || spottingUI.activeInHierarchy) //if ui is active, set its x rotation to that of the camera and set the y rotation to that of the player
            {
                Vector3 eulerAngleRots = new Vector3(Camera.main.transform.rotation.eulerAngles.x, GameManager.instance.player.transform.rotation.eulerAngles.y, 0.0f);
                Quaternion rotation = Quaternion.Euler(eulerAngleRots);
                enemyUIParent.transform.rotation = rotation;
            }
        }


        private void OnTriggerEnter(Collider other) //sets player in range if in range
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private void OnTriggerExit(Collider other) //sets player in range if in not range
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
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
        public void TakeDamage(int dmg) //logic for taking damage. 
        {
            HP -= dmg;
            hmSource.Play();
            StartCoroutine(HitMarker());
            

            updateEnemyUI();
            if (HP <= 0) //if the enemy is dead, turns of lasersight, stops all active coroutines, stops animations, and turns off collision.
            {
                
                GameManager.instance.hitMarkKill.gameObject.SetActive(false);
                if (lineRenderer != null)
                {
                    lineRenderer.SetActive(false);
                    
                    Debug.Log("LineRendererOff");
                }
                StopAllCoroutines();
                spottedUI.SetActive(false);
                spottingUI.SetActive(false);
                wholeHealthBar.SetActive(false);

                anim.SetBool("Aiming", false);
                anim.SetBool("Death", true);
                hmSource.PlayOneShot(deathSound, deathSoundVol);
                GameManager.instance.UpdateObjective(-1);

                StartCoroutine(HitMarker());

                agent.enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;
                
            }
            else //starts attacking the player and instantly spots them
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if(!spotted)
                {
                    StartCoroutine(spottedUIon());
                }
                spotted = true;
                StartCoroutine(flashColor());
                
            }
        }


        private void updateEnemyUI() // Updates enemyHP bar
        {
            HPBar.transform.localScale = new Vector3((float)HP / enemyHPOriginal, HPBar.localScale.y, HPBar.localScale.y);
        }


        private void facePlayer() //faces the player. Called when enemy is at stopping distance
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
        }
        

        public void createBullet() //spawns the bullet at a position. needs work
        {
            playerDirection =  new Vector3(0,1,0) + GameManager.instance.player.transform.position - shootPosition.position;
            Instantiate(bullet, shootPosition.position, Quaternion.LookRotation(playerDirection));
            hmSource.PlayOneShot(shootSound, shootSoundVol);
            Debug.DrawRay(shootPosition.position, playerDirection);
        }

        /**----------------------------------------------------------------
         * @brief Coroutine to make enemy shoot at the player.
         */
        private IEnumerator shoot() //coroutine to control shooting logic. Shoots a bullet, shoots a burst if needed
        {
            isShooting = true;
            
            anim.SetTrigger("Shoot");
            if (isBurstShot)
            {
                StartCoroutine(shootBurst());

            }
            else
            {
                createBullet();
            }
 
            yield return new WaitForSeconds(shootRate);
            
            isShooting = false;
        }

         
        private IEnumerator roam() //enemy chooses a random spot in roamDist and paths to it
        {
            if (!destinationChosen && agent.remainingDistance < 0.05f)
            {

                destinationChosen = true;
                agent.stoppingDistance = 0;

                yield return new WaitForSeconds(roamTimer);

                destinationChosen = false;
                Vector3 randomPos = Random.insideUnitCircle * roamDist;

                randomPos += startingPos;


                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
            }
        }


        private IEnumerator spottedUIon() //turns the UI on if the player gets spotted. turns it off after 3seconds
        {
            if (spottingUI.activeInHierarchy)
            {
                spottingUI.SetActive(false);
            }
            spottedUI.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            spottedUI.SetActive(false);
        }

        /**----------------------------------------------------------------
         * @brief 
         */
        private IEnumerator losingPlayer() //starts losing the player if LOS is lost. after a time the player is lost and enemy returns to defautl. Currently set to 5 seconds for testing
        {
            roamDist = 10;
            //make some function for looking around after stopping
            Debug.Log("Losing Player");
            if (spottedUI.activeInHierarchy)
            {
                spottedUI.SetActive(false);
            }
            qmarkTransform.localScale = new Vector3(4, qmarkTransform.localScale.y, qmarkTransform.localScale.z);
            spottingUI.SetActive(true);
            yield return new WaitForSeconds(5.0f);
            spotted = false;
            Debug.Log("PlayerLost");
            percentSpotted = 0;
            //anim.SetBool("Aiming", false);
        }

 
        private IEnumerator flashColor() //make the enemy model flash when taking damage
        {

            Color originalColor = model.material.color;
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = originalColor;
        }
        
      
        private IEnumerator shootBurst() //burst that shoots 3 bullets 
        {
            createBullet();
            yield return new WaitForSeconds(burstRate);
            createBullet();
            yield return new WaitForSeconds(burstRate);
            createBullet();
        }
        
    
        private IEnumerator lookAround() //WIP: Has the enemy look around a certain degree after reaching patrol spots
        {
            //if spotted look 45 left, 45 right, 180, 45 left 45 right
            //if patrolling rotate player based on index in int list
 
            //Quaternion rotationAmount = transform.rotation * Quaternion.Euler(0, patrolRotations[currentPointIndex], 0);
            //transform.rotation *= rotationAmount;
         
            agent.isStopped = true;
            //transform.rotation.SetFromToRotation(transform.rotation.eulerAngles, rotationAmount.eulerAngles);
            //transform.rotation.SetLookRotation(rotationAmount.eulerAngles);

            //transform.rotation = Quaternion.Slerp(transform.rotation, rotationAmount, 0.1f);
            
            yield return new WaitForSeconds(3.0f);
            Debug.Log("Rotated");
            agent.isStopped = false;
            sawPlayerTemp = false;
            currentPointIndex++;
            if (currentPointIndex > numOfPatrolSpots - 1)
            {
                currentPointIndex = 0;
            }
            timeCount = 0;
        }
    }
}

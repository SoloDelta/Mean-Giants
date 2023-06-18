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
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.ComponentModel;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class EnemyAI : MonoBehaviour, IDamage
    {
        [Header("-----Components-----")]
        [SerializeField] private Renderer model;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator anim;
        [SerializeField] private Transform headPosition;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private GameObject lineRenderer;

        [Header("----- UIComponents-----")]
        [SerializeField] private GameObject enemyUIParent;
        [SerializeField] public GameObject wholeHealthBar;
        [SerializeField] private Transform HPBar;
        [SerializeField] private GameObject spottingUI;
        [SerializeField] private GameObject spottedUI;
        [SerializeField] private Transform qmarkTransform;

        [Header("-----Enemy Stats-----")]
        [SerializeField] private int HP;
        [SerializeField] private float speed;
        [SerializeField] private int playerFaceSpeed;
        [SerializeField] private int viewConeAngle;
        [SerializeField] private bool atStart;

        [Header("-----Roaming-----")]
        [SerializeField, Range(1, 10)] private float roamTimer;
        [SerializeField, Range(10, 100)] private int roamDist;

        [Header("-----Pathfinding-----")]
        [SerializeField] private List<Vector3> patrolSpots = new List<Vector3>();
        [SerializeField] private List<int> patrolRotations = new List<int>();

        [Header("-----Enemy Stats-----")]
        [SerializeField] private float shootRate;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float burstRate;
        [SerializeField] private bool isBurstShot;
        private int numOfPatrolSpots;
        private Vector3 playerDirection;
        private bool playerInRange;
        private float angleToPlayer;
        private bool isShooting;
        private bool seesPlayer;
        private bool destinationChosen;
        private Vector3 startingPos;
        private float stoppingDistanceOriginal;
        private bool isPatrolling;
        private int currentPointIndex;
        private GameObject player;
        private int enemyHPOriginal;
        private float percentSpotted;
        private bool spotted = false;
        private Coroutine losingPlayerCopy;
        private Coroutine lookAroundCopy;
        private bool sawPlayerTemp = false;
        private float timeCount = 0.0f;
        private void Start()
        {
            GameManager.instance.UpdateObjective(1);
            //player = GameObject.FindGameObjectWithTag("Player");
            player = GameManager.instance.player;
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

        /**----------------------------------------------------------------
         * @brief MonoBehaviour override.
         */
        private void Update()
        {
            
            if (agent.isActiveAndEnabled)
            {
                anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
                rotateUI();
                if(true) //removed playerinrange
                {
                    seesPlayer = canSeePlayer();
                    enemyNav();
                    
                }
                
            }
            
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private void enemyNav()
        {
            if (!spotted) //if the player has not been spotted update percent spotted
            {
                spotting(Time.deltaTime);
            }
            if (seesPlayer) //if the enemy sees the player
            {
                
                if (!spotted) //if the player has not been spotted
                {
                    agent.isStopped = true;
                    sawPlayerTemp = true;
                }
                else
                {
                    if (losingPlayerCopy != null)
                    {
                        StopCoroutine(losingPlayerCopy);
                        losingPlayerCopy = null;
                        spottingUI.SetActive(false);
                        StartCoroutine(spottedUIon());
                    }
                    agent.isStopped = false;
                    agent.stoppingDistance = stoppingDistanceOriginal;
                    agent.SetDestination(GameManager.instance.player.transform.position);
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        facePlayer();
                    }
                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
                    }
                }
            }
            else
            {
                
                if (spotted)
                {
                    agent.isStopped = false;
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
                else
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
                        agent.isStopped = false;
                        StartCoroutine(roam());
                    }
                    
                }
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private bool canSeePlayer()
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

        /**----------------------------------------------------------------
         * @brief
         */
        private void spotting(float _deltaTime)
        {
            if (seesPlayer && percentSpotted < 1)
            {
                percentSpotted += 0.5f * _deltaTime;
                qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
                //Debug.Log(qmarkTransform.localScale.x);
                //Debug.Log(percentSpotted);

            }
            else if (!seesPlayer && percentSpotted > 0)
            {
                percentSpotted -= 0.25f * _deltaTime;
                qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
            }
            if (percentSpotted >= 1)
            {
                spotted = true;
                spottingUI.SetActive(false);
                StartCoroutine(spottedUIon());
                Debug.Log(spotted);
            }
            if (percentSpotted <= 0)
            {
                spottingUI.SetActive(false);
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
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
        
        /**----------------------------------------------------------------
         * @brief
         */
        private void rotateUI()
        {
            if (wholeHealthBar.activeInHierarchy || spottedUI.activeInHierarchy || spottingUI.activeInHierarchy) //if the health bar is active, set its x rotation to that of the camera and set the y rotation to that of the player
            {
                Vector3 eulerAngleRots = new Vector3(Camera.main.transform.rotation.eulerAngles.x, GameManager.instance.player.transform.rotation.eulerAngles.y, 0.0f);
                Quaternion rotation = Quaternion.Euler(eulerAngleRots);
                enemyUIParent.transform.rotation = rotation;
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }

        /**----------------------------------------------------------------
         * @brief
         */
        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            
            updateEnemyUI();
            if (HP <= 0)
            {
                if (lineRenderer != null)
                {
                    lineRenderer.SetActive(false);
                    
                    Debug.Log("LineRendererOff");
                }
                StopAllCoroutines();
                spottedUI.SetActive(false);
                anim.SetBool("Death", true);
                GameManager.instance.UpdateObjective(-1);
                
                agent.enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;
                
            }
            else
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

        /**----------------------------------------------------------------
         * @brief
         */
        private void updateEnemyUI()
        {
            HPBar.transform.localScale = new Vector3((float)HP / enemyHPOriginal, HPBar.localScale.y, HPBar.localScale.y);
        }

        /**----------------------------------------------------------------
         * @brief
         */
        private void facePlayer() //faces the player. Called when enemy is at stopping distance
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
        }
        
        /**----------------------------------------------------------------
         * @brief
         */
        public void createBullet()
        {
            Instantiate(bullet, shootPosition.position, transform.rotation); //no shooting at the player, need to be changed
        }

        /**----------------------------------------------------------------
         * @brief Coroutine to make enemy shoot at the player.
         */
        private IEnumerator shoot()
        {
            isShooting = true;
            if (isBurstShot)
            {
                StartCoroutine(shootBurst());

            }
            else
            {
                Instantiate(bullet, shootPosition.position, transform.rotation);
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

        /**----------------------------------------------------------------
         * @brief Coroutine to make enemy roam around the nav mesh.
         */        
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

        /**----------------------------------------------------------------
         * @brief
         */
        private IEnumerator spottedUIon()
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
        private IEnumerator losingPlayer()
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
        }

        /**----------------------------------------------------------------
         * @brief Coroutine to make the enemy flash red when damaged.
         */
        private IEnumerator flashColor()
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = Color.white;
        }
        
        /**----------------------------------------------------------------
         * @brief Corouting to shoot burst fire at player.
         */
        private IEnumerator shootBurst()
        {
            Instantiate(bullet, shootPosition.position, transform.rotation);
            yield return new WaitForSeconds(burstRate);
            Instantiate(bullet, shootPosition.position, transform.rotation);
            yield return new WaitForSeconds(burstRate);
            Instantiate(bullet, shootPosition.position, transform.rotation);
        }
        
        /**----------------------------------------------------------------
         * @brief Coroutine to make enemy look around for player.
         */
        private IEnumerator lookAround()
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

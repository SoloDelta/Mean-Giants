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
        [SerializeField] Renderer model;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Animator anim;
        [SerializeField] Transform headPosition;
        [SerializeField] Transform shootPosition;
        [SerializeField] GameObject lineRenderer;

        [Header("----- UIComponents-----")]
        [SerializeField] GameObject enemyUIParent;
        [SerializeField] public GameObject wholeHealthBar;
        [SerializeField] Transform HPBar;
        [SerializeField] GameObject spottingUI;
        [SerializeField] GameObject spottedUI;
        [SerializeField] Transform qmarkTransform;

        [Header("-----Enemy Stats-----")]
        [SerializeField] int HP;
        [SerializeField] float speed;
        [SerializeField] int playerFaceSpeed;
        [SerializeField] int viewConeAngle;
        [SerializeField] bool atStart;

        [Header("-----Roaming-----")]
        [SerializeField, Range(1, 10)] float roamTimer;
        [SerializeField, Range(10, 100)] int roamDist;



        [Header("-----Pathfinding-----")]
        [SerializeField] List<Vector3> patrolSpots = new List<Vector3>();



        [Header("-----Enemy Stats-----")]
        [SerializeField] float shootRate;
        [SerializeField] GameObject bullet;
        [SerializeField] float burstRate;
        [SerializeField] bool isBurstShot;

        int numOfPatrolSpots;
        Vector3 playerDirection;
        bool playerInRange;
        float angleToPlayer;
        bool isShooting;
        bool seesPlayer;
        bool destinationChosen;
        Vector3 startingPos;
        float stoppingDistanceOriginal;
        bool isPatrolling;
        int currentPointIndex;
        GameObject player;

        int enemyHPOriginal;
        //bool seesPlayer;
        float percentSpotted;
        bool spotted = false;
        Coroutine losingPlayerCopy;


        void Start()
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
            }
            else
            {
                isPatrolling = false;
            }
          
        }

        void Update()
        {
            if (agent.isActiveAndEnabled)
            {
                anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
                rotateUI();
                if(true) //removed playerinrange
                {
                    seesPlayer = canSeePlayer();
                    enemyNav();
                    patrolCirculation();
                }
                
            }
            
        }
        ////////////
        ///Enemy Logic  
        ////////////
        void enemyNav()
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
                agent.isStopped = false;
                if (spotted)
                {
                    agent.stoppingDistance = 0;

                    if (agent.destination.x == transform.position.x && agent.destination.z == transform.position.z)
                    {
                        Debug.Log("At last spot");
                        if (losingPlayerCopy == null)
                        {
                            losingPlayerCopy = StartCoroutine(losingPlayer());
                            Debug.Log("starte losing plyer");
                        }
                        //Start look around routine
                        //
                        StartCoroutine(roam());
                    }
                }
                else
                {
                    StartCoroutine(roam());
                }
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
            
            //agent.stoppingDistance = 0;
            return false;
        }
        void spotting(float _deltaTime)
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
        void patrolCirculation() //controls the logic for pathing. if the enemy has a patrol route and doesnt see the player, he patrols. if he doesnt have a patrol he roams.
        {
            if (isPatrolling && !seesPlayer)
            {
                agent.SetDestination(patrolSpots[currentPointIndex]);
                if (new Vector3(transform.position.x, patrolSpots[currentPointIndex].y, transform.position.z) == patrolSpots[currentPointIndex])
                {
                    currentPointIndex++;
                    if (currentPointIndex > numOfPatrolSpots - 1)
                    {
                        currentPointIndex = 0;
                    }
                }
            }
            else { StartCoroutine(roam()); }
        }

        /* ///RETIRED 
        bool isFollowingPlayer() //checks to see if the enemy can see the play then tracks the player and shoots at the player
        {
            if (playerInRange)
            {
                player = GameManager.instance.player;
                playerDirection = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z) - headPosition.transform.position;
                angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
                Debug.DrawRay(headPosition.position, playerDirection);

                RaycastHit hit;
                if (Physics.Raycast(headPosition.position, playerDirection, out hit))
                {
                    if (hit.collider.CompareTag("Player") && angleToPlayer <= viewConeAngle)
                    {
                        spottingUI.SetActive(true);
                        agent.stoppingDistance = stoppingDistanceOriginal;
                        agent.SetDestination(player.transform.position);


                        return true;
                    }
                }
            }
            agent.stoppingDistance = 0;
            return false;
        }
        */
        ////////////
        //Misc  
        ////////////
        void rotateUI()
        {
            if (wholeHealthBar.activeInHierarchy || spottedUI.activeInHierarchy || spottingUI.activeInHierarchy) //if the health bar is active, set its x rotation to that of the camera and set the y rotation to that of the player
            {
                Vector3 eulerAngleRots = new Vector3(Camera.main.transform.rotation.eulerAngles.x, GameManager.instance.player.transform.rotation.eulerAngles.y, 0.0f);
                Quaternion rotation = Quaternion.Euler(eulerAngleRots);
                enemyUIParent.transform.rotation = rotation;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                Debug.Log("THIS IS WHYY");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }
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
        void updateEnemyUI()
        {
            HPBar.transform.localScale = new Vector3((float)HP / enemyHPOriginal, HPBar.localScale.y, HPBar.localScale.y);
        }
        void facePlayer() //faces the player. Called when enemy is at stopping distance
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
        }
        public void createBullet()
        {
            Instantiate(bullet, shootPosition.position, transform.rotation); //no shooting at the player, need to be changed
        }
        ////////////////////////////
        ////IENUMERATORS
        ////////////////////////////
        IEnumerator shoot()
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
        IEnumerator roam() //enemy chooses a random spot in roamDist and paths to it
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
        IEnumerator spottedUIon()
        {
            if (spottingUI.activeInHierarchy)
            {
                spottingUI.SetActive(false);
            }
            spottedUI.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            spottedUI.SetActive(false);
        }
        IEnumerator losingPlayer()
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
        IEnumerator flashColor()
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = Color.white;
        }

        
       
       
       
        IEnumerator shootBurst()
        {
            Debug.Log("BRRAPP");
            Instantiate(bullet, shootPosition.position, transform.rotation);
            yield return new WaitForSeconds(burstRate);
            Instantiate(bullet, shootPosition.position, transform.rotation);
            yield return new WaitForSeconds(burstRate);
            Instantiate(bullet, shootPosition.position, transform.rotation);
        }
        

        

    }
}

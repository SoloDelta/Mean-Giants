using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//Project: Mean_Giants Midterm
public class enemyAI : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    GameObject player;

    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewConeAngle;
    
    [SerializeField] bool atStart;



    [Header("-----Pathfinding-----")]
    [SerializeField] List<Vector3> patrolSpotss = new List<Vector3>();
    [SerializeField] int currentPointIndex;


    [Header("-----Enemy Stats-----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;

    int numOfPatrolSpots;
    Vector3 playerDirection;
    public bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    bool seesPlayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        numOfPatrolSpots = patrolSpotss.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
        seesPlayer = isFollowingPlayer();
        patrolCirculation();
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashColor());
        if(HP <= 0)
        {
            //TODO: decrement enemies remaining in GM
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void patrolCirculation()
    {
        if(!seesPlayer)
        {
            agent.SetDestination(patrolSpotss[currentPointIndex]);
            if (new Vector3(transform.position.x, patrolSpotss[currentPointIndex].y, transform.position.z) == patrolSpotss[currentPointIndex])
            {
                currentPointIndex++;
                if (currentPointIndex > numOfPatrolSpots - 1)
                {
                    currentPointIndex = 0;
                }
            }
        }
        
    }
   

    bool isFollowingPlayer() //checks to see if the enemy can see the play then tracks the player
    {
        if(playerInRange)
        {
            //Debug.Log(player.transform.position);
            playerDirection = player.transform.position - headPosition.transform.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
            Debug.DrawRay(headPosition.position, playerDirection);

            RaycastHit hit;
            if (Physics.Raycast(headPosition.position, playerDirection, out hit)) //if raycast successfully hits
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= viewConeAngle) //and it hit the player
                {
                    agent.SetDestination(player.transform.position);
                    //Debug.Log("Chasing Player");
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        //implement code for facing the player when at stopping distance
                    }
                    if(!isShooting)
                    {
                        StartCoroutine(shoot());
                        Debug.Log("Bang");
                    }
                    //if not shooting, start coroutine for shooting
                    return true;
                }
            }
        }
        
        return false;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPosition.position, transform.rotation); 
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

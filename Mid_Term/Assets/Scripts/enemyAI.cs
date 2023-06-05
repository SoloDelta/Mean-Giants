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
    GameObject player;

    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewConeAngle;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] bool atStart;


    Vector3 playerDirection;
    public bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    bool seesPlayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        moveOnPatrol();
        seesPlayer = isFollowingPlayer();
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

    void moveOnPatrol()
    {
        //NOTE: This code will do for now, but setting the y position will cause issues on multi storied scenes.
        if(atStart)
        {
            agent.SetDestination(endPosition);
            if(new Vector3(transform.position.x, startPosition.y, transform.position.z) == endPosition)
            {
                atStart = !atStart;
              
            }
        }
        else
        {
            agent.SetDestination(startPosition);
            if (new Vector3(transform.position.x, startPosition.y, transform.position.z) == startPosition)
            {
                atStart = !atStart;
                
            }
        }
        

    }

    bool isFollowingPlayer() //checks to see if the enemy can see the play then tracks the player
    {

        //Debug.Log(player.transform.position);
        playerDirection = player.transform.position - headPosition.transform.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit)) //if raycast successfully hits
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewConeAngle && playerInRange) //and it hit the player
            {
                agent.SetDestination(player.transform.position);
                Debug.Log("Chasing Player");
                if(agent.remainingDistance <= agent.stoppingDistance) 
                {
                    //implement code for facing the player when at stopping distance
                }

                //if not shooting, start coroutine for shooting
                return true;
            }
        }
        return false;
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

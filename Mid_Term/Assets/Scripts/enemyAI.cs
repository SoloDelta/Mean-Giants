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


    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewConeAngle;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] bool atStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveOnPatrol();
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
                Debug.Log("Swap");
            }
        }
        else
        {
            agent.SetDestination(startPosition);
            if (new Vector3(transform.position.x, startPosition.y, transform.position.z) == startPosition)
            {
                atStart = !atStart;
                Debug.Log("Swap");
            }
        }
        

    }
    //TODO Add code for chasing player once game manager is  set up
}

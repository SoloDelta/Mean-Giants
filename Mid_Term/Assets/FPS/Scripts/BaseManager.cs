using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [SerializeField] GameObject enemiesParent;
    [SerializeField] GameObject alarmsParent;
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> alarms = new List<GameObject>();
    [SerializeField] public bool pullAlarm;
    [SerializeField] public bool isPullingAlarm;
    [SerializeField] public bool highAlert;
    [SerializeField] GameObject heavy;
    [SerializeField] GameObject shotgun;
    [SerializeField] GameObject reinformentSpawnPos;
    [SerializeField] int numOfReinforcements;
    [SerializeField] int alertEnemiesRange; //this number decides how far away other enemies can be to hear alerts
    bool reinforced = false;
    bool playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in enemiesParent.transform)
        {
            if(child.gameObject.GetComponent<EnemyAIRefactor>() != null)
            {
                enemies.Add(child.gameObject);
                Debug.Log(child.gameObject);
            }
         
        }
        foreach (Transform child in alarmsParent.transform)
        {
            if (child.gameObject.CompareTag("AlarmParent"))
            {
                alarms.Add(child.gameObject);
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        //if(pullAlarm)
        //{
        //    PullAlarm();
        //}
        if (highAlert)
        {
            if(!reinforced)
            {
                reinforced = true;
                StartCoroutine(Reinforcements());
                
                foreach (GameObject enemy in enemies)
                {
                    if(enemy.GetComponent<EnemyAIRefactor>().agent.isActiveAndEnabled)
                    {
                        if(enemy.GetComponent<EnemyAIRefactor>().currentState != "Combat")
                        {
                            enemy.GetComponent<EnemyAIRefactor>().SearchBase();
                            enemy.GetComponent<EnemyAIRefactor>().highAlert = true;
                        }
                        
                    }
                    
                }
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            enemiesParent.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            enemiesParent.SetActive(false);
        }
    }

    public void PullAlarm(GameObject _callingEnemy = null)
    {
        if(!isPullingAlarm)
        {
            isPullingAlarm = true;
            Debug.Log("Called");
            foreach (GameObject alarm in alarms)
            {
                Debug.Log("Alarm");
                
                FindClosestEnemy(alarm, _callingEnemy).GetComponent<EnemyAIRefactor>().pullAlarm = true;
            }
        }
       
    }

    IEnumerator Reinforcements()
    {
        for(int i = 0; i < numOfReinforcements; i++) 
        {
            if(Random.Range(0,2) == 0) { Instantiate(heavy, reinformentSpawnPos.transform); }
            else { Instantiate(shotgun, reinformentSpawnPos.transform); }
            yield return new WaitForSeconds(1);
        }
    }
    public GameObject FindClosestEnemy(GameObject _alarm, GameObject _callingEnemy)
    {
        /////
        ///This function searches all of the enemies in the base and finds the best enemy to pull the alarm. If this was called by an enemy, it sends the enemy closest to the alarm that is in callingEnemy's range. Otherwise it just sends the closest enemy
        GameObject closestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            if(enemy.GetComponent<EnemyAIRefactor>().agent.isActiveAndEnabled)
            {
                
                if (_callingEnemy != enemy)
                {
                    
                    if(_callingEnemy == null)
                    {
                        if (closestEnemy == null)
                        {
                            closestEnemy = enemy;
                        }
                        if (Vector3.Distance(_alarm.transform.position, closestEnemy.transform.position) > Vector3.Distance(_alarm.transform.position, enemy.transform.position))
                        {
                            closestEnemy = enemy;
                        }
                        
                    }
                    else if (_callingEnemy != null)
                    {
                        Debug.Log("calling enemy not null");
                        if (alertEnemiesRange >= Vector3.Distance(_callingEnemy.transform.position, enemy.transform.position))
                        {
                            if(closestEnemy == null) { closestEnemy = enemy; }
                            if (Vector3.Distance(_alarm.transform.position, closestEnemy.transform.position) > Vector3.Distance(_alarm.transform.position, enemy.transform.position))
                            {
                                closestEnemy = enemy;
                                
                            }
                        }
                    }
               
                    Debug.Log("Enemy");
                }
            }
        }
        return closestEnemy;
    }
}

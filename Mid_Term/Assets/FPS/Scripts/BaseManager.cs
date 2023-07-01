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
        if(pullAlarm)
        {
            PullAlarm();
        }
        if (highAlert)
        {
            if(!reinforced)
            {
                reinforced = true;
                StartCoroutine(Reinforcements());
                
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<EnemyAIRefactor>().SearchBase();
                    enemy.GetComponent<EnemyAIRefactor>().highAlert = true;
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

    void PullAlarm()
    {
        if(!isPullingAlarm)
        {
            isPullingAlarm = true;
            Debug.Log("Called");
            foreach (GameObject alarm in alarms)
            {
                Debug.Log("Alarm");
                GameObject closestEnemy = enemies[0];
                foreach (GameObject enemy in enemies)
                {
                    Debug.Log("Enemy");
                    if (Vector3.Distance(alarm.transform.position, closestEnemy.transform.position) > Vector3.Distance(alarm.transform.position, enemy.transform.position))
                    {
                        closestEnemy = enemy;
                    }
                }
                closestEnemy.GetComponent<EnemyAIRefactor>().pullAlarm = true;
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
}

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

namespace FPS
{
    public class BaseManager : MonoBehaviour
    {
        #region Vars
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
        #endregion

        #region Start
        void Start()
        {
            foreach (Transform child in enemiesParent.transform)
            {
                if(child.gameObject.GetComponent<EnemyAIRefactor>() != null)
                {
                    enemies.Add(child.gameObject);
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
        #endregion

        #region Update
        void Update() //If the base is on high alert, call reinforcements (if not done) and for every active enemy alert them and have them search the base
        {
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
        #endregion

        #region Triggers
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                enemiesParent.SetActive(true);

                foreach (GameObject enemy in enemies)
                {
                    if(enemy.layer == 13)
                    {

                        enemy.SetActive(false);
                    }
                    else
                    {
                        enemy.SetActive(true);
                    }
                }
                
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                enemiesParent.SetActive(false);
            }
        }
        #endregion

        #region Alarm Stuff
        public void PullAlarm(GameObject _callingEnemy = null) //for every alarm, find the closest enemy and have them pull the alarm
        {
            if(!isPullingAlarm)
            {
                isPullingAlarm = true;
                foreach (GameObject alarm in alarms)
                {
                    GameObject closestEnemy = FindClosestEnemy(alarm, _callingEnemy);
                    if(closestEnemy != null)
                    {
                        closestEnemy.GetComponent<EnemyAIRefactor>().pullAlarm = true;
                    }
                    
                }
            }
        
        }
        public GameObject FindClosestEnemy(GameObject _alarm, GameObject _callingEnemy)
        {
            /////
            ///This function searches all of the enemies in the base and finds the best enemy to pull the alarm. 
            ///If this was called by an enemy, it sends the enemy closest to the alarm that is in callingEnemy's range. Otherwise it just sends the closest enemy
            GameObject closestEnemy = null;
            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<EnemyAIRefactor>().agent.isActiveAndEnabled)
                {

                    if (_callingEnemy != enemy)
                    {

                        if (_callingEnemy == null)
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
                            if (alertEnemiesRange >= Vector3.Distance(_callingEnemy.transform.position, enemy.transform.position))
                            {
                                if (closestEnemy == null) { closestEnemy = enemy; }
                                if (Vector3.Distance(_alarm.transform.position, closestEnemy.transform.position) > Vector3.Distance(_alarm.transform.position, enemy.transform.position))
                                {
                                    closestEnemy = enemy;

                                }
                            }
                        }
                    }
                }
            }
            return closestEnemy;
        }
        IEnumerator Reinforcements() //instantiates random enemies when the alarm is pulled
        {
            for(int i = 0; i < numOfReinforcements; i++) 
            {
                if(Random.Range(0,2) == 0) { Instantiate(heavy, reinformentSpawnPos.transform); }
                else { Instantiate(shotgun, reinformentSpawnPos.transform.position, transform.rotation).GetComponent<EnemyRoam>().startingPos = this.transform.position; }
                yield return new WaitForSeconds(1);
            }
        }
        #endregion
        public void AlertNearbyEnemies(GameObject _callingEnemy)
        {
            foreach(GameObject enemy in enemies)
            {
                if (enemy.GetComponent<EnemyAIRefactor>().agent.isActiveAndEnabled)
                {
                    if (enemy != _callingEnemy)
                    {
                        if (Vector3.Distance(enemy.transform.position, _callingEnemy.transform.position) < alertEnemiesRange)
                        {
                            Debug.Log("enem alerted");
                            enemy.GetComponent<EnemyAIRefactor>().shouldStartSearching = true;
                            enemy.GetComponent<EnemyAIRefactor>().agent.SetDestination(GameManager.instance.player.transform.position);
                        }
                    }
                }
                
            }

        }
    }
}

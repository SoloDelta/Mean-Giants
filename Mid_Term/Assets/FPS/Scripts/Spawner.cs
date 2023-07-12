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
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief
     */
    public class Spawner : MonoBehaviour
    {
        [SerializeField] GameObject objectToSpawn;
        [SerializeField] Transform[] spawnPos;
        [SerializeField] float timeBetweenSpawns;
        [SerializeField] int numberToSpawn;


        int numberSpawn;
        bool playerInRange;
        bool isSpawning;


        // Start is called before the first frame update
        void Start()
        {
            //GameManager.instance.UpdateObjective(numberToSpawn); 
        }

        // Update is called once per frame
        void Update()
        {
            if (playerInRange && !isSpawning && numberSpawn < numberToSpawn)
            {
                StartCoroutine(Spawn());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        IEnumerator Spawn()
        {
            isSpawning = true;
            for(int i = 0; i < spawnPos.Length; i++)
            {
                Instantiate(objectToSpawn , spawnPos[i].position, transform.rotation);
            }
            numberSpawn++;
            yield return new WaitForSeconds(timeBetweenSpawns);
            isSpawning = false;
        }
    }
}
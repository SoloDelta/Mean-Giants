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
    public class PrisonToHqNpcSpawn : MonoBehaviour
    {
        [SerializeField] GameObject objectToSpawn;
        [SerializeField] Transform spawnPos;
        [SerializeField] int numberToSpawn;
        [SerializeField] float timeBetweenSpawn;

        int numberSpawn;
        bool playerInRange;
        bool isSpawning;

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
            GameObject npc = Instantiate(objectToSpawn, spawnPos.position, Quaternion.identity);
            npc.transform.LookAt(Camera.main.transform);
            numberSpawn++;
            yield return new WaitForSeconds(timeBetweenSpawn);
            isSpawning = false;
        }
    }
}

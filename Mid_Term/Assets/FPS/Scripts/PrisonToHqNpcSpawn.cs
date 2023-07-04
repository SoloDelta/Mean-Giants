using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Instantiate(objectToSpawn, spawnPos.position, Quaternion.identity);
            objectToSpawn.transform.LookAt(Camera.main.transform);
            numberSpawn++;
            yield return new WaitForSeconds(timeBetweenSpawn);
            isSpawning = false;
        }
}


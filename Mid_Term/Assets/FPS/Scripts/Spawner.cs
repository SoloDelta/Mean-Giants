using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //GameManager.instance.UpdateObjective(numberToSpawn); I am removing this line because some enemies cant be spawned in, so they need the updateobjective in their respective start - john
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
        Instantiate(objectToSpawn, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
        numberSpawn++;
        yield return new WaitForSeconds(timeBetweenSpawns);
        isSpawning = false;
    }


}

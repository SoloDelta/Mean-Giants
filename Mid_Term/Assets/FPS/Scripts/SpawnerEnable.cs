using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class SpawnerEnable : MonoBehaviour
    {
        public List<GameObject> enemiesInRange = new List<GameObject>();
        bool initialQueryDone = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (!initialQueryDone)
                {
                    if (!enemiesInRange.Contains(other.gameObject))
                    {
                        Debug.Log("Query");
                        enemiesInRange.Add(other.gameObject);
                        other.gameObject.SetActive(false);
                    }
                }
            }
            

            if(other.CompareTag("Player"))
            {
                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    enemiesInRange[i].SetActive(true);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    enemiesInRange[i].SetActive(false);
                }
            }
        }
    }
}

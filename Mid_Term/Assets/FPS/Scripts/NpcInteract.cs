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
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FPS
{
    public class NpcInteract : MonoBehaviour
    {
        bool isOn = false;
        [SerializeField] KeyStorage key;
        [SerializeField] Canvas[] npcText;
        [SerializeField] string[] updateObjective;

        [Header("--- Boundry Collider")]
        public Collider prisonCollider;
        public Collider villageCollider;
        public Mesh changeTo;
        public MeshFilter prisonMeshFilter;
        public MeshFilter villageMeshFilter;

        public bool hasPrisonObjective = false;
        public bool hasVillageObjective = false;
        public bool firstMissionCompleted = false;
        public bool secondMissionCompleted = false;
        bool thirdMissionCompleted = false;
        PrisonToHqNpcSpawn spawned;

        private void Update()
        {
     
        }

        private void Start()
        {
            GameManager.instance.objectiveText.text = updateObjective[0];
        }



        public void Interact()
        {
            if(!firstMissionCompleted)
            {
                StartCoroutine(MissionOne());
            }
            if(firstMissionCompleted)
            {
                StartCoroutine(MissionTwo());
            }

        }





        public void TurnOffMessage()
        {
            for(int i = 0; i < npcText.Length; i++)
            {
                npcText[i].enabled = false;
            }
        }


        public bool PlayerHasPrisonKey()
        {
            if (key.HasPrisonKey)
            {
                Debug.Log("Cell Key True");
                return true;
            }
            else
            {
                Debug.Log("Cell Key False");
                return false;
            }
        }

        private void MissionColliderDisable()
        {
            if (hasPrisonObjective)
            {
                prisonMeshFilter.mesh = changeTo;
                prisonCollider.enabled = false;
            }

            else if (hasVillageObjective)
            {
                villageMeshFilter.mesh = changeTo;
                villageCollider.enabled = false;
            }
        }

        private IEnumerator MissionOne()
        {
            if (PlayerHasPrisonKey())
            {
                    hasPrisonObjective = true;
                    MissionColliderDisable();
                    npcText[1].enabled = true;
                    GameManager.instance.objectiveText.text = updateObjective[1];
                    yield return new WaitForSeconds(15);
                    npcText[1].enabled = false;
                    firstMissionCompleted = true;
             }
             else
             {
                    npcText[0].enabled = true;
                    yield return new WaitForSeconds(15);
                    npcText[0].enabled = false;
                    GameManager.instance.objectiveText.text = updateObjective[0];
                
             }

        }

        private IEnumerator MissionTwo()
        {
            npcText[2].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[2].enabled = false;
            GameManager.instance.objectiveText.text = updateObjective[2];

        }

        private void MissionThree()
        {
            //steal item from base one
        }

        private void MissionFour()
        {
            // kill enemy mission
        }

        private void MissionFive()
        {
            //steal item from base two 
        }

        private void MissionSix()
        {
            //steal item 
        }

        private void MissionSeven()
        {
            //kill enemies
        }


    }
}
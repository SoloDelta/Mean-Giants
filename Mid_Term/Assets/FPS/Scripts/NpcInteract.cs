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
        bool fourthMissionCompleted = false;
        bool fifthMissionCompleted = false;
        bool sixthMissionCompleted = false;
        bool seventhMissionCompleted = false;
        PrisonToHqNpcSpawn spawned;

        private void Update()
        {
            Objectives();
        }

        private void Start()
        {
           
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

        public void Objectives()
        {
            if(!firstMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[0];
            }
            if(firstMissionCompleted && !secondMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[1];
            }
            if(secondMissionCompleted && !thirdMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[2];
            }
            if(thirdMissionCompleted && !fourthMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[3];
            }
            if(fourthMissionCompleted && !fifthMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[4];
            }
            if(fifthMissionCompleted && !sixthMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[5];
            }
            if(sixthMissionCompleted && !seventhMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[6];
            }
            if(seventhMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[7];
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
                    yield return new WaitForSeconds(15);
                    npcText[1].enabled = false;
                    firstMissionCompleted = true;
             }
             else
             {
                    npcText[0].enabled = true;
                    yield return new WaitForSeconds(15);
                    npcText[0].enabled = false;
             }

        }

        private IEnumerator MissionTwo()
        {
            npcText[2].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[2].enabled = false;

        }

        private IEnumerator MissionThree()
        {
            //steal item from base one
            npcText[3].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[3].enabled = false;
        }

        private IEnumerator MissionFour()
        {
            // kill enemy mission
            npcText[4].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[4].enabled = false;
        }

        private IEnumerator MissionFive()
        {
            //steal item from base two 
            npcText[5].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[5].enabled = false;
        }

        private IEnumerator MissionSix()
        {
            //steal item
            npcText[6].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[6].enabled = false;
        }

        private IEnumerator MissionSeven()
        {
            //kill enemies
            npcText[7].enabled = true;
            yield return new WaitForSeconds(15);
            npcText[7].enabled = false;
        }


    }
}
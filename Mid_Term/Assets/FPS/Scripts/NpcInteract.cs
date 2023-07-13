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
        PlayerController player;

        [Header("--- Boundry Collider")]
        public Collider prisonCollider;
        public Collider villageCollider;
        public Collider phantomCollider;
        public Mesh changeTo;
        public MeshFilter prisonMeshFilter;
        public MeshFilter villageMeshFilter;
        public MeshFilter phantomMeshFilter;


        public bool hasPrisonObjective;
        public bool hasVillageObjective;
        public bool firstMissionCompleted;
        public bool secondMissionCompleted;
        public bool secondMissionStarted;
        public bool thirdMissionCompleted;
        public bool thirdMissionStarted;
        public bool fourthMissionCompleted;
        public bool fifthMissionCompleted;
        public bool sixthMissionCompleted;
        public bool seventhMissionCompleted;
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
            else if(firstMissionCompleted)
            {
                StartCoroutine(MissionTwo());

            }
            else if(secondMissionCompleted)
            {
                StartCoroutine(MissionThree());
            }

        }

        public void Objectives()
        {
            if(!firstMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[0];
            }
            if(firstMissionCompleted)
            {
                GameManager.instance.objectiveText.text = updateObjective[1];
                secondMissionStarted = true;
            }
            if(secondMissionStarted)
            {
                GameManager.instance.objectiveText.text = updateObjective[2];
                GameManager.instance.enemiesToKill.enabled = true;
                if (GameManager.instance.enemiesRemaining <= 0)
                {
                    secondMissionCompleted = true;
                }

            }
            if(secondMissionCompleted)
            {
                GameManager.instance.enemiesToKill.enabled = false;
                GameManager.instance.objectiveText.text = updateObjective[3];
            }
            if(thirdMissionStarted)
            {
                GameManager.instance.objectiveText.text = updateObjective[4];
                if(player.stoleFile)
                {
                    thirdMissionCompleted = true;
                }
            }
            if(thirdMissionCompleted)
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
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator MissionOne()
        {
            if (PlayerHasPrisonKey())
            {
                    hasPrisonObjective = true;
                    prisonMeshFilter.mesh = changeTo;
                    prisonCollider.enabled = false;
                    npcText[1].enabled = true;
                    firstMissionCompleted = true;
                    yield return new WaitForSeconds(10);
                    npcText[1].enabled = false;
             }
             else
             {
                    npcText[0].enabled = true;
                    yield return new WaitForSeconds(10);
                    npcText[0].enabled = false;
             }

        }

        private IEnumerator MissionTwo()
        {
            //kill enemies
            npcText[2].enabled = true;
            villageMeshFilter.mesh = changeTo;
            villageCollider.enabled = false;
            yield return new WaitForSeconds(15);
            npcText[2].enabled = false;



        }

        private IEnumerator MissionThree()
        {
            //steal item from base one
            npcText[3].enabled = true;
            thirdMissionStarted = true;
            phantomMeshFilter.mesh = changeTo;
            phantomCollider.enabled = false;
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
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
//using UnityEditor.VersionControl;
using UnityEngine;

namespace FPS
{
    public class NpcInteract : MonoBehaviour
    {
        bool isOn = false;
        [SerializeField] KeyStorage key;
        [SerializeField] float deletePrisonNpcTimer;
        [SerializeField] Canvas[] npcText;

        [Header("--- Boundry Collider")]
        public Collider prisonCollider;
        public Collider villageCollider;

        public bool hasPrisonObjective = false;
        bool hasVillageObjective = false;
        bool firstMissionCompleted = false;
        bool secondMissionCompleted = false;

        private void Update()
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
                prisonCollider.enabled = false;
            }

            else if (hasVillageObjective)
            {
                villageCollider.enabled = false;
            }
        }

        private IEnumerator MissionOne()
        {
            if (!isOn)
            {
                if (PlayerHasPrisonKey())
                {
                    hasPrisonObjective = true;
                    MissionColliderDisable();
                    npcText[1].enabled = true;
                    yield return new WaitForSeconds(8);
                    npcText[1].enabled = false;
                    firstMissionCompleted = true;
                }
                else
                {
                    npcText[0].enabled = true;
                    yield return new WaitForSeconds(8);
                    npcText[0].enabled = false;
                }
                isOn = true;
            }
            else
            {
                TurnOffMessage();
                isOn = false;
            }
        }

        private IEnumerator MissionTwo()
        {
            npcText[2].enabled = true;
            yield return new WaitForSeconds(8);
            npcText[2].enabled = false;

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
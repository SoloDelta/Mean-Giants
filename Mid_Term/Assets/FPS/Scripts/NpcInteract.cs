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
        [SerializeField]KeyStorage key;
        [SerializeField] float deletePrisonNpcTimer;

        [Header("--- Boundry Collider")]
        public Collider prisonCollider;
        public Collider villageCollider;

        public bool hasPrisonObjective = false;
        bool hasVillageObjective = false;
        bool deletePrisonNpc = false;

        private void Update()
        {
            
        }



        public void Interact()
        {
            if(!isOn)
            {
                if(!deletePrisonNpc)
                {
                    StartCoroutine(TurnOnMessage());
                    isOn = true;
                }
                
            }
            else
            {
                TurnOffMessage();
                isOn = false;
            }
            
        }


        public IEnumerator TurnOnMessage()
        {

            if (PlayerHasPrisonKey())
            {
                hasPrisonObjective = true;
                MissionColliderDisable();
                GameManager.instance.npcPrisonText[1].enabled = true;
                yield return new WaitForSeconds(5);
                GameManager.instance.npcPrisonText[1].enabled = false;
                yield return new WaitForSeconds(deletePrisonNpcTimer);
                Destroy(gameObject);
                deletePrisonNpc = true;
            }
            else
            {
                GameManager.instance.npcPrisonText[0].enabled = true;
                yield return new WaitForSeconds(5);
                GameManager.instance.npcPrisonText[0].enabled = false;
            }
        }

        public void TurnOffMessage()
        {
            if (PlayerHasPrisonKey())
            {
                GameManager.instance.npcPrisonText[1].enabled = false;
            }
            else
            {
                GameManager.instance.npcPrisonText[0].enabled = false;
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
}
}
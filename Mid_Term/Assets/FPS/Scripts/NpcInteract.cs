using FPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class NpcInteract : MonoBehaviour
{
    bool isOn = false;
    [SerializeField]KeyStorage key;

    public void Interact()
    {
        if(!isOn)
        {
            TurnOnMessage();
            isOn = true;
        }
        else
        {
            TurnOffMessage();
            isOn = false;
        }
        
    }


    public void TurnOnMessage()
    {

        if (PlayerHasPrisonKey())
        {
            GameManager.instance.npcPrisonText[1].enabled = true;
        }
        else
        {
            GameManager.instance.npcPrisonText[0].enabled = true;
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
}

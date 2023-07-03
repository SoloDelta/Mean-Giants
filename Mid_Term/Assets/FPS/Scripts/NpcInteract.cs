using FPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class NpcInteract : MonoBehaviour
{
    [SerializeField]public TextMeshProUGUI NpcTextMesh;
    bool isOn = false;

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
        GameManager.instance.npcPrisonText.enabled = true;
    }

    public void TurnOffMessage()
    {
        GameManager.instance.npcPrisonText.enabled = false;
    }
}

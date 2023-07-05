using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChange : MonoBehaviour
{
    public GameObject firstScreenCanvas;
    public GameObject secondScreenCanvas;
    public void FlipScreens()
    {
        firstScreenCanvas.SetActive(!firstScreenCanvas.activeSelf);
        secondScreenCanvas.SetActive(!secondScreenCanvas.activeSelf);
    }
}

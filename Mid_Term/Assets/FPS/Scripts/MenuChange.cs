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
using UnityEngine;

namespace FPS
{
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
}

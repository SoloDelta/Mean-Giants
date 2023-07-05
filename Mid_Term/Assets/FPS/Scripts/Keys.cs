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
    public enum Key
    {
        prisonKey,
        compoundKey,
        prisonCellKey,
    }

    public class Keys : MonoBehaviour
    {
        public GameObject model;
        public Key key;
    }
}

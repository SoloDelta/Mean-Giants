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
    public class KeyStorage : MonoBehaviour
    {
        public bool _hasPrisonKey;
        public bool _hasCompoundKey;

        public bool HasPrisonKey
        {
            get { return _hasPrisonKey; }
            set { _hasPrisonKey = value; }
        }

        public bool HasCompoundKey
        {
            get { return _hasCompoundKey; }
            set { _hasCompoundKey = value; }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

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
    /**----------------------------------------------------------------
     * @brief Data class to set different gun stats.
     */
    [CreateAssetMenu] public class GunStats : ScriptableObject
    {
        public int shootDistance;
        public int shootDamage;
        public float shootRate;
        public int maxAmmo;
        public int curAmmo;

        public GameObject model;
        public ParticleSystem hitEffect;
    }
}

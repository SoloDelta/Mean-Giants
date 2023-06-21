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
        public weaponClasses weaponClass;

        public GameObject model;
        public ParticleSystem hitEffect;

        public AudioSource gSource;
        [SerializeField][Range(0, 1)] AudioClip gClip;

        public void ShootSounds()
        {
            gSource.PlayOneShot(gClip);
        }
    }

  

}

public enum weaponClasses
{
    Heavy, Smg, AssualtRifle, Pistol
}


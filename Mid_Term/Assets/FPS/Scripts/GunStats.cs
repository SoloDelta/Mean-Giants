/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using UnityEngine;

namespace FPS
{
    /**----------------------------------------------------------------
     * @brief 
     */
    public enum WeaponClasses
    {
        Heavy, Smg, AssualtRifle, Pistol
    }

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
        public WeaponClasses weaponClass;

        public GameObject model;
        public ParticleSystem hitEffect;
    }
}

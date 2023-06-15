using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public int shootDistance;
    public int shootDamage;
    public float shootRate;
    public int maxAmmo;
    public int curAmmo;

    public GameObject model;
    public ParticleSystem hitEffect;
}

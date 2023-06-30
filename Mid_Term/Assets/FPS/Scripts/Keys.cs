using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Key
{
    prisonKey,
    compoundKey,
    prisonCellKey,
}

public class Keys : ScriptableObject
{
    public GameObject model;
    public Key key;
}

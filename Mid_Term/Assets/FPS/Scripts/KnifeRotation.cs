using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeRotation : MonoBehaviour
{
    [Range(-100, 100)][SerializeField] private int x;
    [Range(-100, 100)][SerializeField] private int y;
    [Range(-100, 100)][SerializeField] private int z;
    void Update()
    {
        transform.Rotate(x,y,z);
    }
}

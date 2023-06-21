using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeRotation : MonoBehaviour
{
    [Range(-40, 40)][SerializeField] private int x;
    [Range(-40, 40)][SerializeField] private int y;
    [Range(-40, 40)][SerializeField] private int z;
    void Update()
    {
        transform.Rotate(x,y,z);
    }
}

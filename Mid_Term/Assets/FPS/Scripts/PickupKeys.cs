using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupKeys : MonoBehaviour
{
    [SerializeField] private Keys key;
    private readonly Key useableKey;

    public void OnTriggerEnter(Collider other)
    {
        IKey hasKey = other.GetComponent<IKey>();

        if(hasKey != null)
        {
            hasKey.PickupKey(useableKey, gameObject);
        }
    }
}

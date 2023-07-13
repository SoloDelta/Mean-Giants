using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObjective : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        IObjective objective = other.GetComponent<IObjective>();

        if(objective != null )
        {
            objective.ObjectivePickup(gameObject); 
        }
    }
}

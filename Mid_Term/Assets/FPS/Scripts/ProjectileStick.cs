using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStick : MonoBehaviour
{
    private Rigidbody rb;

    private bool targetHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)  //sticks to first object collided   
            return;
        else
            targetHit = true;


        rb.isKinematic = true; //sticks to surface

        transform.SetParent(collision.transform); //moves with object stuck to
        
    }


}

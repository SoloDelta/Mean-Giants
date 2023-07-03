using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField][Range(0, 15)] float interactRange;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
           Collider[] colliderArray =  Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out NpcInteract interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }
}

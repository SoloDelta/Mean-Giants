using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [Range(1, 50)][SerializeField] private int rayLength;
    [SerializeField] private LayerMask layerInteract;
    [SerializeField] private string excludeName = null;

    private DoorController raycastedObj;

    [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse1;

    [SerializeField] private Image Reticle = null;
    private bool isReticleActive;
    private bool doOnce;

    private const string interactableTag = "Door";

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(excludeName) | layerInteract.value;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                if (!doOnce)
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<DoorController>();
                    ReticleChange(true);
                }

                isReticleActive = true;
                doOnce = true;

                if (Input.GetKeyDown(openDoorKey))
                {
                    raycastedObj.PlayAnimation();
                }
            }
        }

        else
        {
            if (isReticleActive)
            {
                ReticleChange(false);
                doOnce = false;
            }
        }
    }

    void ReticleChange(bool on)
    {
        if (on && !doOnce)
        {
            Reticle.color = Color.white;
        }

        else
        {
            Reticle.color = Color.red;
            isReticleActive = false;
        }
    }
}

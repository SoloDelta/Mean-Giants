using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator doorAnim;

    [SerializeField] private string openAnimation;
    [SerializeField] private string closeAnimation;

    private bool doorOpen = false;

    private void Awake()
    {
        doorAnim = gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        OpenDoor();
    }

    private void OpenDoor()
    {
        if (!doorOpen)
        {
            doorAnim.Play(openAnimation, 0, 0.0f);
            doorOpen = true;
        }
        else
        {
            doorAnim.Play(closeAnimation, 0, 0.0f);
            doorOpen = false;
        }
    }
}

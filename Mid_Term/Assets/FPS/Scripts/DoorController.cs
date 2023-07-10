/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - 
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class DoorController : MonoBehaviour
    {
        private Animator doorAnim;
        [Header("--- Animation ---")]
        [SerializeField] private string openAnimation;
        [SerializeField] private string closeAnimation;
        [Header("--- Storage ---")]
        [SerializeField] KeyStorage _keyStorage = null;
        public AudioMixer audioMixer;
        public bool prisonDoor;
        public bool PrisonCell;
        public bool CompoundDoor;


        private bool doorOpen = false;

        private void Awake()
        {
            doorAnim = gameObject.GetComponent<Animator>();
            _keyStorage = FindObjectOfType<KeyStorage>();
        }

        public void PlayAnimation()
        {
            if (_keyStorage._hasPrisonKey)
            {
                if (prisonDoor)
                {
                    OpenDoor();
                }
            }
            else
            {
                if (CompoundDoor)
                {
                    OpenDoor();
                }
            }
        }

        private void OpenDoor()
        {
            if (!doorOpen)
            {
                doorAnim.Play(openAnimation, 0, 0.0f);
                audioMixer.DoorSound();
                doorOpen = true;
            }
            else
            {
                doorAnim.Play(closeAnimation, 0, 0.0f);
                audioMixer.DoorSound();
                doorOpen = false;
            }
        }
    }
}

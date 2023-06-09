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
using System.Text.RegularExpressions;
using UnityEngine;

namespace FPS
{

    public class AudioMixer : MonoBehaviour
    {
        public AudioSource soundSource;
        public AudioSource aud;


        [SerializeField] AudioClip pickupClip;
        [SerializeField][Range(0, 1)] float pickupVol;
        [SerializeField] AudioClip gunShot;
        [SerializeField][Range(0, 1)] float shotVol;
        [SerializeField] AudioClip[] audJump;
        [Range(0, 1)][SerializeField] float audJumpVol;
        [SerializeField] AudioClip[] audDamage;
        [Range(0, 1)][SerializeField] float audDamageVol;
        [SerializeField] AudioClip[] audSteps;
        [Range(0, 1)][SerializeField] float audStepsVol;
        [SerializeField] AudioClip audCrouch;
        [Range(0, 1)][SerializeField] float audCrouchVol;
        [SerializeField] AudioClip[] audHurt;
        [Range(0, 1)][SerializeField] float audHurtVol;
        [SerializeField] AudioClip audReload;
        [Range(0, 1)][SerializeField] float audReloadVol;
        [SerializeField] AudioClip healthClip;
        [SerializeField][Range(0, 1)] float healthVol;
        [SerializeField] AudioClip emptyClipAud;
        [Range(0, 1)][SerializeField] float emptyClipVol;
        [SerializeField] AudioClip DoorAud;
        [Range(0, 1)][SerializeField] float DoorVol;
        [SerializeField] AudioClip knifeAud;
        [Range(0, 1)][SerializeField] float knifeVol;
        [SerializeField] AudioClip knifeThrowAud;
        [Range(0, 1)][SerializeField] float knifeThrowVol;

        //float[] sfxGroup = new float[9];


        /* void Start()
         {
             sfxGroup[0] = pickupVol;
             sfxGroup[1] = shotVol;
             sfxGroup[2] = audJumpVol;
             sfxGroup[3] = audDamageVol;
             sfxGroup[4] = audCrouchVol;
             sfxGroup[5] = audHurtVol;
             sfxGroup[6] = audReloadVol;
             sfxGroup[7] = healthVol;
             sfxGroup[8] = emptyClipVol;
             sfxGroup[9] = audStepsVol;

         }*/



        public void JumpSound()
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }

        public void StepsSound()
        {
            aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        }

        public void CrouchSound()
        {
            aud.PlayOneShot(audCrouch, audCrouchVol);
        }

        public void ShootSound()
        {
            aud.PlayOneShot(gunShot, shotVol);
        }

        public void EmptyClipSound()
        {
            aud.PlayOneShot(emptyClipAud, emptyClipVol);
        }
        public void HurtSound()
        {
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        }

        public void ReloadSound()
        {
            aud.PlayOneShot(audReload, audReloadVol);
        }

        public void PickupClipSound()
        {
            aud.PlayOneShot(pickupClip, pickupVol);
        }

        public void HealthPickupSound()
        {
            aud.PlayOneShot(healthClip, healthVol);
        }

        public void DoorSound()
        {
            aud.PlayOneShot(DoorAud, DoorVol);
        }
        public void KnifeSlash()
        {
            aud.PlayOneShot(knifeAud, knifeVol);
        }
        public void KnifeThrow()
        {
            aud.PlayOneShot(knifeThrowAud, knifeThrowVol);
        }

    }
}

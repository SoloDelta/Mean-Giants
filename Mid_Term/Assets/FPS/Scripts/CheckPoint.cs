/**
 * Copyright (c) 2023 - 2023, The Mean Giants, All Rights Reserved.
 *
 * Authors
 *  - John Price
 */

//-----------------------------------------------------------------
// Using Namespaces
//-----------------------------------------------------------------
using System.Collections;
using UnityEngine;

namespace FPS
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] Renderer model;
        [SerializeField] Color colorOrig;
        public AudioSource aud;
        [SerializeField] AudioClip checkPointAudio;
        [SerializeField][Range(0,1)] float checkPointVol;

        private void Start()
        {
            colorOrig = model.materials[0].color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
            {
                GameManager.instance.playerSpawnPos.transform.position = transform.position;
                StartCoroutine(playerColor());
                aud.PlayOneShot(checkPointAudio, checkPointVol);

            }
        }

        private IEnumerator playerColor()
        {
            model.material.color = Color.blue;
            GameManager.instance.checkPointPopup.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            model.material.color = colorOrig;
            GameManager.instance.checkPointPopup.SetActive(false);
        }
    }
}
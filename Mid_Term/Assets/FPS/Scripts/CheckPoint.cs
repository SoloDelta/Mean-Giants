using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] Color colorOrig;

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

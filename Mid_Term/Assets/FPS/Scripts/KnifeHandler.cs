using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnifeHandler : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject Knife;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;



    private void Start()
    {
        audioMixer = FindObjectOfType<AudioMixer>();
    }

    private void Update()
    {

        if (GameManager.instance.activeMenu == null)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (CanAttack)
                {
                    StartCoroutine(GunHide());
                    KnifeAttack();
                    audioMixer.KnifeSlash();
                }
            }
        }

    }

    public void KnifeAttack()
    {
        CanAttack = false;
        Animator anim = Knife.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCD());

    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }

    IEnumerator GunHide()
    {
        GameManager.instance.gunPos.SetActive(false);
        yield return new WaitForSeconds(.5f);
        GameManager.instance.gunPos.SetActive(true);


    }
}

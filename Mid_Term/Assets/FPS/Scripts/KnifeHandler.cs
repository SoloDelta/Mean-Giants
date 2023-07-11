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
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (CanAttack)
            {
                KnifeAttack();
                audioMixer.KnifeSlash();
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
}
